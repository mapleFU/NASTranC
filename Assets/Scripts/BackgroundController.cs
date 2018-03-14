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

		private int height;
		private int width;
		private GameObject ur_border; 	// 边境对象
		private GameObject ul_border;
		private GameObject dr_border;
		private GameObject dl_border;

		// 格子中每个点的大小
		// 在初始化的时候设置
		public float grid_size;
		private int[,] map;
		public int[,] Map {
			get {
				return map;
			}
		}

		const string MAL_PATH = "/Users/fuasahi/Desktop/CreateAPF2.m";
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
		}

		// 初始化势能
		void initialize_potenial_energy()
		{
			// initial map
			GameObject[] borders = GameObject.FindGameObjectsWithTag ("Border");
			// TODO: fix this

			foreach (GameObject bdr in borders)
			{
				if (bdr.transform.position.x > 0) {
					if (bdr.transform.position.y > 0) {
						ur_border = bdr;
					} else {
						dr_border = bdr;
					}
				} else {
					if (bdr.transform.position.y > 0) {
						ul_border = bdr;
					} else {
						dl_border = bdr;
					}
				}
			}
//			Debug.Log ("XR: "  + (int)Math.Ceiling(ur_border.transform.position.x / GRID_SIZE) + " XL: "+ (int)Math.Ceiling(-ul_border.transform.position.x / GRID_SIZE));
//			Debug.Log((int)Math.Ceiling(ul_border.transform.position.y / GRID_SIZE) + " SPLIT "+ (int)Math.Ceiling(-dl_border.transform.position.y / GRID_SIZE));
			int width_all = (int)Math.Ceiling(ur_border.transform.position.x / GRID_SIZE) + (int)Math.Ceiling(-ul_border.transform.position.x / GRID_SIZE);
			int height_all = (int)Math.Ceiling(ul_border.transform.position.y / GRID_SIZE) + (int)Math.Ceiling(-dl_border.transform.position.y / GRID_SIZE);
			width = width_all;
			height = height_all;
//			SpriteRenderer render = GetComponent<SpriteRenderer> ();
//			width = (int)render.bounds.size.x + 1;
//			height = (int)render.bounds.size.y + 1;
//			Debug.Log("width= " + width + ", height= " + height);
			map = new int[width, height];
		}

//		void call_matlab () {
//			// Create the MATLAB instance 
//			MLApp.MLApp matlab = new MLApp.MLApp(); 
//
//			// Change to the directory where the function is located 
//			matlab.Execute(@"cd c:\temp\example"); 
//
//			// Define the output 
//			object result = null; 
//
//			// Call the MATLAB function myfunc
//			matlab.Feval("myfunc", 2, out result, 3.14, 42.0, "world"); 
//
//			// Display result 
//			object[] res = result as object[]; 
//
//			Console.WriteLine(res[0]); 
//			Console.WriteLine(res[1]); 
//			Console.ReadLine(); 
//		}
	}

}
