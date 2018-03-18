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

namespace SimuUtils {

	// 用于存放子对象，处理子元素的类
	// TODO: 是否考虑加入事件
	public class ChildObjects 
	{
		private static int n = 0;
		private int num;
		public override string ToString ()
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
		private ArrayList _lifts = new ArrayList ();

		public ArrayList dests {
			get { return _dests;}
			set {  _dests = value;}
		}
		public ArrayList blocks {
			get { return _blocks;}
		}
		public ArrayList humans {
			get { return _humans;}
		}
		public ArrayList lifts {
			get { return _lifts;}
		}


	}

	public class BackgroundController : MonoBehaviour {
		const int GRID_SIZE = 1;		// 元胞点的大小

		// 装有所有的子对象
		public ChildObjects childObjects;

		public int myLayer;
		private float height;
		private float width;
		private float xmin, ymin;
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
		public int x, y;	// x y坐标点的数目
		private float[,] map;
		public float[,] Map {
			get {
				return map;
			}
		}

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

		bool has_disaster;	// 若本层拥有局部灾情
		void LateUpdate() {
			// TODO: 填充获得险情的场景


			//  
			var human_lists = childObjects.humans;
			int size = human_lists.Count;
			for (int i = 1; i < size; ++i) {
				HumanController h = human_lists [i] as HumanController;
				if (h.in_disaster) {
					for (int j = 0; j < size; ++j) {
						HumanController humanj = human_lists [j] as HumanController;
						if (Vector2.Distance (h.transform.position, humanj.transform.position)
							< HumanController.MAX_DISASTER_BROADCAST) {
							// 应急模式
							humanj.to_disaster_mode ();
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
			return new Vector2 ((float)Math.Truncate(position.x), (float)Math.Truncate(position.y));
		}

		public void add_person(GameObject person)
		{
			HumanController person_script = person.GetComponent<HumanController> ();
			childObjects.humans.Add (person_script);
		}

		public void pop_person(GameObject person)
		{
			HumanController person_script = person.GetComponent<HumanController> ();
			childObjects.humans.Remove (person_script);
		}

		// 最早应该做的事情
		public static int uid_z = 0;
		void Awake() 
		{
			childObjects = new ChildObjects ();

//			var pos = this.transform.position;
//			pos.z = uid_z;
//			transform.position = pos;
//			++uid_z;
		}

		// Use this for initialization
		private static readonly object syncLock = new object();
		public virtual void Start () {
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
			Debug.Log ("BKG controller start.");
		}

		// 初始化势能
		void initialize_potenial_energy()
		{
//			// initial map
//			GameObject[] borders = GameObject.FindGameObjectsWithTag ("Border");
//			// TODO: fix this
//
//			foreach (GameObject bdr in borders)
//			{
//				if (bdr.transform.position.x > 0) {
//					if (bdr.transform.position.y > 0) {
//						ur_border = bdr;
//					} else {
//						dr_border = bdr;
//					}
//				} else {
//					if (bdr.transform.position.y > 0) {
//						ul_border = bdr;
//					} else {
//						dl_border = bdr;
//					}
//				}
//			}
//			int width_all = (int)Math.Ceiling(ur_border.transform.position.x / GRID_SIZE) + (int)Math.Ceiling(-ul_border.transform.position.x / GRID_SIZE);
//			int height_all = (int)Math.Ceiling(ul_border.transform.position.y / GRID_SIZE) + (int)Math.Ceiling(-dl_border.transform.position.y / GRID_SIZE);
//			width = width_all;
//			height = height_all;


//			SpriteRenderer render = GetComponent<SpriteRenderer> ();
			var bounds = GetComponent<BoxCollider2D> ();

			// 获得长与宽
			width = bounds.size.x;
			height = bounds.size.y;
			xmin = transform.position.x - width;	// x 最小的坐标
			ymin = transform.position.y - height;	// y 最小的坐标
			Debug.Log("width= " + width + ", height= " + height);
			// the map may be bigger than you wish to be
			/*
			 * 以下内容是对于每个map而言的
			 * 对于复数个map
			 * 需要你们补充了（;￣O￣）
			 */ 
			map = new float[(int)width + 1, (int)height + 1];	// 建立一个大型的数组
			// 先把所有内容初始化为墙壁级别
			for (int i = 0; i < (int)width + 1; ++i) {
				for (int j = 0; j < (int)height + 1; ++j) {
					map [i, j] = 30000.0f;
				}
			}
			// don't init it now.
			init_map ();
		}

		/*
		 *  初始化势能场
		 * 	从每个APF中读取数据
		 */ 
		void init_map() {
			using (FileStream fs = File.OpenRead(FILE_NAME)) {
				BinaryReader br = new BinaryReader (fs);

				for (int i = 0; i < x; ++i) {
					for (int j = 0; j < y; ++j) {
						// read from it
						map [i, j] = br.ReadSingle ();
					}
				}
			}

		}

		/*
		 * 地图坐标转化为整数坐标
		 */ 
		public Vector2 pos2mapv(Vector2 pos) {
			float gridx = (float)Math.Ceiling((pos.x - xmin) / grid_size);
			float gridy = (float)Math.Ceiling((pos.y - ymin)/ grid_size);
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
