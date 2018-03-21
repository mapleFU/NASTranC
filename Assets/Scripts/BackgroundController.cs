/*
 * 基本的背景对象
 * 比如地铁站通道 楼梯等
 * 没有任何障碍物，是正常的通路
 */

// https://www.mathworks.com/help/matlab/matlab_external/call-matlab-function-from-c-client.html?requestedDomain=www.mathworks.com

using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using System.IO;
using System.Threading;


using Ceil = UnityEngine.Vector2;
using System.Linq;

namespace SimuUtils {

    // 用于存放子对象，处理子元素的类
    // TODO: 是否考虑加入事件
    public class ChildObjects
    {
        private static int n = 0;
        private int num;
        public override string ToString()
        {
            return "ChildObjects " + n;
        }

        private static readonly object syncLock2 = new object();
        public ChildObjects() {
            lock (syncLock2) {
                num = n;
                ++n;
            }
        }

        // 对应的background, 需要对象来赋值产生
        public GameObject backGround;

        private ArrayList _dests = new ArrayList();
        private ArrayList _blocks = new ArrayList();
        private ArrayList _humans = new ArrayList();
        private ArrayList _lifts = new ArrayList();

        public ArrayList dests {
            get { return _dests; }
            set { _dests = value; }
        }
        public ArrayList blocks {
            get { return _blocks; }
        }
        public ArrayList humans {
            get { return _humans; }
        }
        public ArrayList lifts {
            get { return _lifts; }
        }


    }

    public class BackgroundController : MonoBehaviour {
        const int GRID_SIZE = 1;        // 元胞点的大小

        // 装有所有的子对象
        public ChildObjects childObjects;

        public int myLayer;
        private float height;
        private float width;
        public float xmin, ymin;
        // 双亲的长度
        private float parent_x;
        private float parent_y;

        // from 0 to start
        public static int layer_num = 0;
        /*
		 * 势能场相关的力
		 */

        // 在初始化的时候设置
        public string FILE_NAME; // 本层的势能场所在的文件名称
        public float grid_size; // 格子中每个点的大小
        public int x, y;    // x y坐标点的数目

        //		// 势能场数组
        //		private ArrayList apfs;
        //		/*
        //		 *  得到第n个apf
        //		 */ 
        //		public float[,] get_apf_n(int n) {
        //			if (n >= apfs.Count || n <= 0) {
        //				return null;
        //			} else {
        //				return apfs[n];
        //			}
        //		}

        public float[,] APF01;
        public float[,] APF02;
        public float[,] APF03;
        public float[,] APF04;
        public List<float[,]> APF05;
        public List<float[,]> APF06;
        public float[,] APF11;
        public float[,] APF12;
        public float[,] APF13;
        public float[,] APF14;
        public List<float[,]> APF15;
        public List<float[,]> APF16;
        /*
		 * 本层是否是楼梯
		 * 是则返回true
		 * 否则返回false
		 */
        public bool on_stair_or_not() {
            // 有父亲的话则为stair。
            return transform.parent != null;
        }


        // 给同层传递火情

        bool has_disaster;  // 若本层拥有局部灾情
        void LateUpdate() {
            // TODO: 填充获得险情的场景


            //  
            var human_lists = childObjects.humans;
            int size = human_lists.Count;
            for (int i = 1; i < size; ++i) {
                HumanController h = human_lists[i] as HumanController;
                if (h.in_disaster) {
                    for (int j = 0; j < size; ++j) {
                        HumanController humanj = human_lists[j] as HumanController;
                        if (Vector2.Distance(h.transform.position, humanj.transform.position)
                            < HumanController.MAX_DISASTER_BROADCAST) {
                            // 应急模式
                            humanj.to_disaster_mode();
                        }

                    }
                }

            }
        }

        //		public const string MAL_PATH = "/Users/fuasahi/Desktop/CreateAPF2.m";

        /*
		 * 坐标转化
		 */
        static Ceil V2_TO_CEIL(Vector2 position)
        {
            return new Vector2((float)Math.Truncate(position.x), (float)Math.Truncate(position.y));
        }

        public void add_person(GameObject person)
        {
            HumanController person_script = person.GetComponent<HumanController>();
            childObjects.humans.Add(person_script);
        }

        public void pop_person(GameObject person)
        {
            HumanController person_script = person.GetComponent<HumanController>();
            childObjects.humans.Remove(person_script);
        }

        // 最早应该做的事情
        public static int uid_z = 0;
        void Awake()
        {
            childObjects = new ChildObjects();

            //			var pos = this.transform.position;
            //			pos.z = uid_z;
            //			transform.position = pos;
            //			++uid_z;
        }

        // Use this for initialization
        private static readonly object syncLock = new object();
        public virtual void Start() {
            var bounds = GetComponent<BoxCollider2D>();

            // 获得长与宽
            width = bounds.size.x * transform.localScale.x;
            height = bounds.size.y * transform.localScale.y;
            xmin = transform.position.x - width / 2;    // x 最小的坐标
            ymin = transform.position.y - height / 2;   // y 最小的坐标
                                                        //			Debug.Log("width= " + width + ", height= " + height + ",xmin = " + xmin + ", ymin = " + ymin);

            const float shrink_rate = 1000.0f;
            grid_size /= shrink_rate;

            childObjects.backGround = this.gameObject;
            // 生成基本的势能场(格点矩阵)，来根据势能场判断人物的选点
            //			initialize_potenial_energy ();
            //			Debug.Log("X and Y is " + height + " and " + width);
            myLayer = gameObject.layer;
            //			= layer_num;
            //			lock (syncLock) {
            //				if (layer_num == 0) {
            //					layer_num = 1;
            //				} else {
            //					// *= 2;
            //					layer_num = 2 * layer_num;
            //				}
            //			}
            Debug.Log("BKG controller start.");
            initialize_potenial_energy();

        }

        // 初始化势能
        void initialize_potenial_energy()
        {
            //var bounds = GetComponent<BoxCollider2D>();

            //// 获得长与宽
            //width = bounds.size.x;
            //height = bounds.size.y;
            //xmin = transform.position.x - width / 2;    // x 最小的坐标
            //ymin = transform.position.y - height / 2;   // y 最小的坐标
            //Debug.Log("width= " + width + ", height= " + height);
            // the map may be bigger than you wish to be
            /*
			 * 以下内容是对于每个map而言的
			 * 对于复数个map
			 * 需要你们补充了（;￣O￣）
			 */
            init_map(FILE_NAME);
        }

        /*
		 *  初始化势能场
		 * 	从每个APF中读取数据
		 */
        void init_map(string filename) {
            //FILE_NAME = "D:\Users\lenovo\Desktop\simu 下午3.50.32\Resource\chengdu"+"\\"+this.name;
            int x, y;
            float grid_size;
            FileStream fs = File.OpenRead(filename);
            StreamReader sr = new StreamReader(fs);
            string s;
            s = sr.ReadLine();
            s = sr.ReadLine();
            grid_size = float.Parse(s);
            s = sr.ReadLine();
            y = int.Parse(s);
            s = sr.ReadLine();
            x = int.Parse(s);
            s = sr.ReadLine();
            float minx = float.Parse(s);
            s = sr.ReadLine();
            float miny = float.Parse(s);
            s = sr.ReadLine();
            float maxx = float.Parse(s);
            s = sr.ReadLine();
            float maxy = float.Parse(s);


            APF01 = new float[y, x];
            APF02 = new float[y, x];
            APF03 = new float[y, x];
            APF04 = new float[y, x];
            APF11 = new float[y, x];
            APF12 = new float[y, x];
            APF13 = new float[y, x];
            APF14 = new float[y, x];

            /*
             * 判断是什么梯子
             */
            int per_t;    // 循环的次数
            if (filename.Contains("Floor"))
            {
                per_t = 6;
                // 如果是FLOOR还要初始化列表
                APF15 = new List<float[,]>();
                APF16 = new List<float[,]>();
                APF05 = new List<float[,]>();
                APF06 = new List<float[,]>();
            }
            else
            {
                per_t = 4;
            }


            /*
             * 建立并初始化哈希
             */
            Hashtable digit2apf = new Hashtable();
            digit2apf.Add(1, APF01);
            digit2apf.Add(2, APF02);
            digit2apf.Add(3, APF03);
            digit2apf.Add(4, APF04);
            digit2apf.Add(5, APF05);
            digit2apf.Add(6, APF06);
            digit2apf.Add(11, APF11);
            digit2apf.Add(12, APF12);
            digit2apf.Add(13, APF13);
            digit2apf.Add(14, APF14);
            digit2apf.Add(15, APF15);
            digit2apf.Add(16, APF16);

            for (int j = 0; j < 2; j++)
            {
                // 4轮
                for (int i = 1; i <= 4; i++)
                {
                    // 获取目标是第几个(大概是I个吧)
                    int line = parse_apf(sr.ReadLine());
                    read_matrix(digit2apf[i + 10 * j] as float[,], x, y, sr);
                    sr.ReadLine();
                }

                /*
                 * 如果是0-4，由于PER_T = 4, 根本不会进入这个区域
                 */
                for (int i = 5; i <= per_t; i++)
                {
                    int line = parse_apf(sr.ReadLine());
                    read_list(x, y, sr, digit2apf[i + 10 * j] as List<float[,]>);
                    //                    sr.ReadLine();
                }
            }
        }

        private void read_matrix(float[,] mat, int sizex, int sizey, StreamReader streamReader)
        {
            Console.WriteLine("x: " + sizex + " and y: " + sizey);
            for (int i = 0; i < sizey; i++)
            {
                string s = streamReader.ReadLine();
                string[] str = s.Split(new char[] { ' ' });
                for (int j = 0; j < sizex; j++)
                {
                    try
                    {
                        mat[i, j] = float.Parse(str[j]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(i + " and " + j);
                        Console.WriteLine(str[j]);
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }

        /*
         * 读取列表
         */
        private void read_list(int sizex, int sizey, StreamReader streamReader, List<float[,]> floatses)
        {
            while (true)
            {
                string s = streamReader.ReadLine();
                if (char.IsDigit(s.ElementAt(0)))
                {
                    // 是数字还要读
                    float[,] floats = new float[sizey,sizex];
                    // 读取这个小矩阵
                    read_matrix(floats, sizex, sizey, streamReader);
                    floatses.Add(floats);
                    // 读取掉下一个数字
                    streamReader.ReadLine();
                }
                else
                {
                    return;
                }
            }
        }
        /*
         * 根据输入的APF行信息解析下一个APF是第几个      
         */
        private int parse_apf(string s)
        {
            s = s.Substring(3);
            int v = int.Parse(s);
            return v;
        }


        /*
		 * 地图坐标转化为整数坐标
		 */
        public Vector2 pos2mapv(Vector2 pos) {
			
			float gridx = (float)Math.Ceiling(Math.Abs((pos.x - xmin) / grid_size));
			float gridy = y-(float)Math.Ceiling(Math.Abs((pos.y - ymin)/ grid_size));
			return new Vector2 (gridx, gridy);
		}

//		/*
//		 * 以上函数的逆向转化
//		 * 想了想好像不怎么要用
//		 */ 
//		private Vector2 pos2map(Vector2 pos) {
//			
//		}
	}

}
