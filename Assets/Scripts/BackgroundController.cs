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

using System.Threading;


using Ceil = UnityEngine.Vector2;

namespace SimuUtils {

	// 用于存放子对象，处理子元素的类
	public class ChildObjects 
	{
		// 对应的background, 需要对象来赋值产生
		public GameObject backGround;

		public ArrayList dests = new ArrayList();
		public ArrayList blocks = new ArrayList();
		public ArrayList humans = new ArrayList();
		public ArrayList lifts = new ArrayList ();


	}

	public class BackgroundController : MonoBehaviour {
		const int GRID_SIZE = 1;		// 元胞点的大小

		// 装有所有的子对象
		public ChildObjects childObjects;

		public int myLayer;
		private int height;
		private int width;

		// from 0 to start
		public static int layer_num = 0;
		/*
		 * 势能场相关的力
		 */
		// 格子中每个点的大小
		// 在初始化的时候设置
		public float grid_size;
		private int[,] map;
		public int[,] Map {
			get {
				return map;
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
		void Start () {
			// 生成基本的势能场(格点矩阵)，来根据势能场判断人物的选点
//			initialize_potenial_energy ();
//			Debug.Log("X and Y is " + height + " and " + width);
			myLayer = gameObject.layer = layer_num;
			Interlocked.Increment (ref layer_num);
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

			SpriteRenderer render = GetComponent<SpriteRenderer> ();
			width = (int)Math.Ceiling(render.bounds.size.x);
			height = (int)Math.Ceiling(render.bounds.size.y);
			Debug.Log("width= " + width + ", height= " + height);
			map = new int[width, height];
		}
			
	}

}
