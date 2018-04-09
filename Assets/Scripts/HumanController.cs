/*
 * 控制人类对象行为的模型
 * 圆形
 */ 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SimuUtils
{
	using Force = UnityEngine.Vector2;


	public class HumanController : BaseChildObject {

		public static GameObject add_human(Vector2 pos , GameObject parent) {
			GameObject instance = Instantiate(Resources.Load("GamePrefab/LittleHuman"),
				pos, Quaternion.identity, parent.transform) as GameObject;
			instance.transform.localScale = new Vector3 (0.005f, 0.04f, 1f);
			instance.tag = "Human";
//			var colid = instance.GetComponent<BoxCollider2D> ();
//			colid.isTrigger = true;
			instance.transform.parent = parent.transform;
			BackgroundController hc = parent.GetComponent<BackgroundController> ();
			if (hc == null) {
				Debug.LogError ("Parent we sent to add_human is null");
			} else {
				instance.layer = hc.gameObject.layer;	
			}

			var cur_pos = instance.transform.position;
			cur_pos.z = -0.41f;
			instance.transform.position = cur_pos;
			return instance;
		}

		// 个人是否察觉到灾害
		public bool in_disaster = false;
		public static float MAX_DISASTER_BROADCAST;	// 人物时间步中传递灾害模式的半径
		// 进入灾害模式
		public void to_disaster_mode () {
			in_disaster = true;	
			SpriteRenderer sr = GetComponent<SpriteRenderer> ();
			sr.color = new Color (1.0f, 0, 0);

		}

		// 人出现的目标点

		// 每一个格子格点的大小，在一个项目中是固定的
		public static float grid_size;

		static private System.Random rnd = new System.Random ();

		private ChildObjects father_containers;
		// 使用过的lift
//		public LiftController used_lift = null;
		public HashSet<LiftController> used_list;

		/*
		 * 判断给出的楼梯是可以使用的
		 */ 
		public bool lift_available(LiftController c) {
			// at first is null.
			if (used_list == null)
				return true;
			return !used_list.Contains (c);
		}

		static public double RandomGussion(double mean, double stdDev)
		{
			double u1 = 1.0-rnd.NextDouble(); //uniform(0,1] random doubles
			double u2 = 1.0-rnd.NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
				Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
			double randNormal =
				mean + stdDev * randStdNormal;
			return randNormal;
		}

		static public double RandomWithBound (double upper, double lower)
		{
			double random01 = rnd.NextDouble ();
			return (upper - lower) * random01 + lower;
		}

		// 常量-平均值-分布 对应的常量调整

		// 半径量
		private const double unity_radius_scale = 0.05f;		// 缩小倍率
		private const double reality_radius_normal = 0.45f;	// 半径平均是多少m
		private const double reality_radius_upper = 0.51f;	// 设计的上限
		private const double reality_radius_lower = 0.39f;	// 设计的下限

		// 社会力作用最大范围，暂定2m
		private const double reality_max_force_range = 2.0f;
//		public double reality_max_force_range;

		// 体重 
		private const double reality_weight_lower = 44;
		private const double reality_weight_upper = 70;
//		public double reality_weight_lower;
//		public double reality_weight_upper;

		// 正常情况下的 初始化速度和期望速度 
		private const double reality_excspeed_mean = 1.26f;
		private const double reality_excspeed_stddev = 0.36f;
		private const double disaster_excspeed_mean = 1.56f;
		private const double disaster_excspeed_stddev = 0.16f;



		private const double reality_inispeed_mean = 1.0f;
		private const double reality_inispeed_stddev = 0.2f;
		private const double disaster_inispeed_mean = 1.16f;
		private const double disaster_inispeed_stddev = 0.2f;
//		public double reality_inispeed_mean;
//		public double reality_inispeed_stddev;


		// running constexpr 
		private const double TIME_EXPR = 0.5;
		private const double MAX_COUNT_DISTANCE = 1.1;
		private const double MAX_MENTALLY_DISTANCE = 1.6;
		private const double MAX_B2P_DISTANCE = 0.75;
		private const double P2P_CONSTEXPR = 1.0;
		private const double B2P_CONSTEXPR = 2.0;		
		private const double B2P_PHY_CONST = 1000;


		// rigidbody
		public Rigidbody2D rb;

		private double weight;			// 质量-> Rigid2d.mass
		private double radius;			// 半径
		private float _normal_exc_speed;	// 预期速度
		private float _disaster_exc_speed;	// 灾情预期速度
		private float exc_speed {
			get { 
				if (in_disaster)
					return _disaster_exc_speed;
				return _normal_exc_speed;
			}
		}

		private float cur_speed;	// 现有速度
		private Vector2 cur_vec2;	// 目前运行的向量

		// 获取目标
//		private static GameObject[] all_dests = null;
		private GameObject dest;

		// 目前的坐标，希望能够根据网络获得，不过这里通过随机生成(或者通过)
		private Vector3 current_position{
			get{ 
				return rb.position;
			}
		}
		private Vector2 Current_velocity{
			get{ 
				return rb.velocity;
			}
		}

		// change destine
		public void change_destine()
		{
			init_destine ();
			var to_dest = dest.GetComponent<DestController> ();
//			Debug.Log ("THe new dest is " + to_dest);
		}
			
		// Use this for initialization
		// 初始化目的地, 本算法选取的是最小的
		private void init_destine()
		{
//			if (all_dests == null) {
//				
////				all_dests = father_containers.dests;
//				all_dests = GameObject.FindGameObjectsWithTag ("Dest");
//			}
			GameObject min_dst = null;
			float min_length = float.MaxValue;
//			this.transform.gameObject
			// need to change this
			int cnt = 0;
			foreach (MonoBehaviour behaviour in father_containers.dests) {
				++cnt;
//				if (behaviour == used_lift) 
//					continue;
				if (behaviour.GetType() == typeof(LiftController) && !lift_available (behaviour as LiftController)) {
					continue;
				}
				GameObject dst = behaviour.gameObject;
				float value = Vector3.Distance (this.transform.position, dst.transform.position);
				if (value < min_length) {
					min_length = value;
					min_dst = dst;
				}
			}
//			Debug.Log ("There are " + cnt + " dests in this layer.");
			// init dest
			dest = min_dst;
//			Debug.Log ("Now " + ToString() +  " dest is " + dest);
		}

		/*
		 * 对人的半径进行初始化
		 */ 
		private void init_radius()
		{

//			radius = RandomWithBound (reality_radius_lower, reality_radius_upper);
////			Debug.Log ("radius: " + radius);
//			// init scale with actual value
//			Vector3 current_scale = transform.localScale;
//			current_scale.x = (float)(radius * unity_radius_scale);
//			current_scale.y = (float)(radius * unity_radius_scale);
//			transform.localScale = current_scale;
		}

		/*
		 * 对人的速度进行初始化
		 */ 
		private void init_speed()
		{
			
//			Debug.Log("Start init speed");
			_normal_exc_speed = (float)RandomGussion (reality_excspeed_mean, reality_excspeed_stddev);
			_disaster_exc_speed = (float)RandomGussion (disaster_excspeed_mean, disaster_inispeed_stddev);
//			Debug.Log ("Init speed expr");

			cur_speed = (float)RandomGussion (reality_inispeed_mean, reality_inispeed_stddev);
            //			Debug.Log ("Init cur speed.");
            // TODO: 初始化对应速度
            //		rb.velocity;
            //			if (dest == null) {
            //				Debug.LogError ("We dont have a dest. And Below are things:");
            //				if (father_containers == null) {
            //					Debug.LogError ("Even no dad!");
            //				} else {
            //					init_destine ();
            ////					foreach (HumanController human in father_containers.humans) {
            ////						Debug.Log (human);
            ////					}
            //				}

            //			}
            Vector3 dir=new Vector3() ;
            System.Random rd = new System.Random();
            dir.x = rd.Next(1, 10); 
            dir.y = rd.Next(1, 10); 
            dir.z = 0;
			dir = dir.normalized;
			//rb.velocity = dir * (float)cur_speed;
            rb.velocity = dir *0;
		}

		// 初始化体重，希望能和radius相关
		private void init_weight()
		{
			weight = RandomWithBound (reality_radius_lower, reality_weight_upper);
			//		Rigidbody2D.mass = weight;
			rb.mass = (float)weight;
		}

		/*
		 * 变换对象的父对象、
		 * 非我别动
		 */ 
		public void change_new_father_container(MonoBehaviour to_script)
		{
			BackgroundController new_father_script = to_script as BackgroundController;
			this.transform.parent = new_father_script.transform;
			father_containers = new_father_script.childObjects;
			if (father_containers == null) {
				Debug.Log ("Fathers are null.");
			}
		}

		/*
		 * 脚本初始化  
		 */ 
		public override void Start () {
			// to myself first.
//			Debug.Log("Human want's to start.");
			var daddy = get_parent_script();
//			Debug.Log ("Human's daddy :" + daddy.gameObject);
			father_containers = daddy.childObjects;
			father_containers.humans.Add (this);

			rb = GetComponent<Rigidbody2D> ();
            // 防止旋转
            rb.freezeRotation=true;
			// 添加自己的对象

			init_radius ();
			init_weight ();
			//init_destine ();
			init_speed ();

//			Debug.Log ("New Human: position:" + transform.position +", and map_position: " + daddy.pos2mapv(transform.position));
			// TODO: 将 in_disaster
			in_disaster = false;
		
		}

		// Update is called once per frame

		// count force 
		public void Update () {
			// DEBUG
//			Debug.Log ("My pos: " + transform.position + " and pos in map " + get_parent_script().pos2mapv(transform.position) + " MyLocalScale: " + transform.localScale );
			// update speed
			cur_speed = rb.velocity.magnitude;
            if(cur_speed> exc_speed)//如果算出来的当前速度大于预期速度 那么就要调整大小
            {
                rb.velocity = rb.velocity* (1/cur_speed)*exc_speed;
            }
			Force 
				pfe = potential_energy_field() * 0.5f,
				p2p = count_p2p() * 2,
				b2p = count_b2p() * 0.05f;

			Force all = /*fhe*/ p2p + b2p + pfe;
            if (Vector3.Dot(rb.velocity, pfe) < 0)
            {
                Vector3 dir = new Vector3();
                dir.x = pfe.x;
                dir.y = pfe.y;
                dir.z = 0;
                this.rb.velocity = dir * 0;
            }
            this.rb.AddForce(all);
			// DEBUG

			Debug.Log ("POS: " + this.current_position + " with "+ get_parent_script().pos2mapv(this.current_position) + " and force " + all + " with father name " + 
				get_parent_script().gameObject.name);

            //点击检测
            if (Input.GetButtonDown("Fire1")) {
				// TODO: fill in
				CameraScript.Instance.onBind(this);
			}
		}

		private Force count_fhe()
		{
			return (float)(weight / TIME_EXPR) * ((float)exc_speed * get_direction_to_dest() - rb.velocity)  ;
		}

		private Force count_p2p()
		{
			return aux_physics_p2p () + aux_mentally_p2p ();
		}

		private Force aux_physics_p2p()
		{
			// temporary empty
			return new Force(0, 0);
		}

		private Force aux_mentally_p2p()
		{
			Force mental_force = new Force(0, 0);

			foreach (HumanController player in father_containers.humans) {

				// 遍历这一层的player，如果是自己则continue
				if (player == this || player.gameObject.activeSelf == false)
					continue;

				// 直接的距离值
				var distance = Vector2.Distance (player.current_position, current_position);
				if (distance >= MAX_COUNT_DISTANCE) {
					continue;
				}
				mental_force += (float)(P2P_CONSTEXPR * Math.Exp ((-Vector3.Distance(this.transform.position, player.transform.position) - this.radius - player.radius) / MAX_MENTALLY_DISTANCE))
					* get_direction_to_dest ();
			}
//			Debug.Log (humans + " human in this layer.");
			return mental_force;
		}

		// Problem: 线性障碍物计算
		// DEBUG
		private Force count_b2p()
		{
			Force f = new Force (0, 0);

//			int blocks = 0;
			foreach (BlockController controller in father_containers.blocks) {
//				++blocks;
				double current_distance = controller.get_distance_to_human (this) - this.radius;
			
				if (current_distance >= 2)
					continue;
				Vector2 closest_point = controller.get_closest_point (this);
				// Still this way...?
				Vector2 b2p_direction = ((Vector2)this.transform.position - closest_point).normalized;
				// 心理接触力 修正
				var cur_force = (float)(B2P_CONSTEXPR * Math.Exp (-current_distance / 0.2)) * b2p_direction;
//				Debug.Log ("b2p mentally " + cur_force);
				f += cur_force;
//				//Debug.Log ("Cur force " + cur_force);
				// 身体接触力
				double physic_distance = current_distance;
				if (physic_distance < 1) {
					Force normal = (float)(-physic_distance * B2P_PHY_CONST) * b2p_direction;
//					Debug.Log ("normal " + normal + " distance * const = " + (-physic_distance * B2P_PHY_CONST));
					Vector2 tang_direc = get_tang_direct (b2p_direction, this, controller);
					Force tangent = tang_direc * VectorMultiply (tang_direc, this.Current_velocity) * 20.0f * (float)current_distance;
//					Debug.Log ("b2p en " + normal);
//					Debug.Log ("Tang " + tangent);
//					Debug.Log ("b2p et " + tang_direc);
					f += (normal + tangent);
				}

			}

//			// DEBUG
//			if (!used) {
//				Debug.Log ("There are " + blocks + "blocks.");
////				used = true;
//			}

			return f;
		}

		public static float VectorMultiply(Vector2 v1, Vector2 v2) {
			return v1.x * v2.x + v1.y * v2.y;
		}

		private Vector2 get_tang_direct(Vector2 edirec, HumanController human, BlockController block) {
			var vec2 = block.transform.position - human.transform.position;
			Vector2 v1 = new Vector2 (0, 0);
			v1.x = edirec.y; v1.y = edirec.x;
			if (VectorMultiply(v1, vec2)  > 0) {
				v1.x = -v1.x;
				v1.y = -v1.y;
			}
			return v1;
		}

		private Vector2 get_direction_to_dest()
		{
			Vector3 dir = dest.transform.position - transform.position;
			dir.z = 0;
			return dir.normalized;
		}

		// 势能场判断逻辑
		public bool has_app;	// 有app
		public bool take_subway;	// 是否乘车, 为true是想上车，否则思想出去



		// 势能场力常数
		public static float potential_energy_field_constexpr=100.0f;

		/*
		 * 根据人的属性得到需要的apf
		 */ 
		// 上一次可以的apf
		private float[,] last_used_apf = null;
		private float[,] find_min_apf(List<float[,]> apf_list, int cur_x, int cur_y) {
			// TODO:test and DEBUG in this function

			if (apf_list == null) {
				Debug.LogError ("May because that apf_list is null.");
			}

			float[,] min_arr = null;
			float min_value = 300000;		// 设置一个很大的初值
			try {
				foreach (float[,] arr in apf_list) {
					float aaa = arr[cur_x, cur_y];
					if (aaa < min_value && aaa >= 0) {
						min_value = arr [cur_x, cur_y];
						min_arr = arr;
					}
				}
			} catch (Exception e) {
				Debug.LogError ("Error pos: " + transform.position + " and mapv"
					+ get_parent_script().pos2mapv(transform.position) + "cur_x, cur_y" + cur_x + ","+ cur_y);
				this.gameObject.SetActive (false);

			}

			if (min_arr == null) {
//				Debug.LogError ("MIN_ARR IS NULL. 无法找到对应的目标。");
//				if (apf_list == null) {
//					Debug.LogError ("May because that apf_list is null.");
//				} else {
//					
//					Debug.LogError ("But apf_list is not null with size" + apf_list.Count);
//					Debug.Log ("Minvalue: " + min_value + " with position: (" + cur_x +", " + cur_y + ")" + " and father scale: " + 
//						get_parent_script().get_pf_scale());
//					
//				}
				min_arr = last_used_apf;
			}

			if (min_arr != null)
				last_used_apf = min_arr;
			
			return min_arr;
		}

		private float[,] get_apf() {
			// 背景脚本
			BackgroundController bkg_script = get_parent_script ();
			float[,] needed_apf;
			var pos = bkg_script.pos2mapv (transform.position);

			// 获得现在的x y坐标
			int cur_x = (int)pos.x, cur_y = (int)pos.y;

			if (!bkg_script.on_stair_or_not ()) {
				// 在楼层上
					// 楼梯扶梯可用
				if (has_app) {
						// 有app
					if (take_subway) {
						// 要乘车
						if (ConfigConstexpr.get_instance ().es_is_running) {
							needed_apf = bkg_script.APF01;
							Debug.Log ("Choose APF01");
						} else {
							needed_apf = bkg_script.APF11;
							Debug.Log ("Choose APF11");
						}
					} else {
						if (ConfigConstexpr.get_instance ().es_is_running) {
							needed_apf = bkg_script.APF02;
							Debug.Log ("Choose APF02");
						} else {
							needed_apf = bkg_script.APF12;
							Debug.Log ("Choose APF12");
						}
							
					}

				} else {

					List<float[,]> search_list;

					// 根据有无app选择对应的list
					if (take_subway) {
						if (ConfigConstexpr.get_instance ().es_is_running) {
							search_list = bkg_script.APF05;
							Debug.Log ("Choose APF05");
						} else {
							search_list = bkg_script.APF15;
							Debug.Log ("Choose APF15");
						}
 							
					} else {
						if (ConfigConstexpr.get_instance ().es_is_running) {
							search_list = bkg_script.APF06;
							Debug.Log ("Choose APF06");
						} else {
							search_list = bkg_script.APF16;
							Debug.Log ("Choose APF16");
						}
							
					}

					const float MIN_V = 30.0f;
					// 找到最小的数组
					float[,] min_arr = find_min_apf (search_list, cur_y, cur_x);
					needed_apf = min_arr;
					// 先直接找最近的出口吧，这个逻辑恨坑的
					init_destine ();
				}


			} else {
				// 如果在楼梯／扶梯上
				if (ConfigConstexpr.get_instance ().es_is_running) {
					// 楼梯扶梯可用
					if (has_app) {
						// 有app

						if (take_subway) {
							needed_apf = bkg_script.APF01;
						} else {
							needed_apf = bkg_script.APF02;
						}
					} else {
						if (take_subway) {
							needed_apf = bkg_script.APF03;
						} else {
							needed_apf = bkg_script.APF04;
						}
					}
				} else {
					// 扶梯不能运行
					if (has_app) {
						// 有app
						if (take_subway) {
							needed_apf = bkg_script.APF11;
						} else {
							needed_apf = bkg_script.APF12;
						}
					} else {
						if (take_subway) {
							needed_apf = bkg_script.APF13;
						} else {
							needed_apf = bkg_script.APF14;
						}
					}
				}


			}

			return needed_apf;
		}

		private Vector2 last_stair_map2v;
		private delegate bool inmap(int x, int y);
		private Force potential_energy_field() {
			var bkg_script = get_parent_script();
//			var map = bkg_script.Map;
			var map = get_apf();
			if (map == null) {
				Debug.Log ("We cannot find map in potential_energy_field, get apf");
			}
			// get map of size
			int rank = bkg_script.x;
			int length = bkg_script.y;

			// in map lambda
			inmap if_in = (a, b) => {
				// DEBUG
				return a < length && b < rank && a >= 0 && b >= 0;
			};

			Vector2 map_vec2;
			try {
				map_vec2 = bkg_script.pos2mapv (this.transform.position);	
			} catch(Exception e) {
				if (bkg_script.gameObject.name.Contains ("Stair") && last_stair_map2v != null) {
					map_vec2 = last_stair_map2v;
				} else {
					throw e;
				}
			}
			last_stair_map2v = map_vec2;
			// DEBUG


			int x = (int)map_vec2.x;
			int y = (int)map_vec2.y;


			int minx=-5, miny=-5;
			float min_value = 300000;
            float cur_value = map[y, x];
            for (int i = y - 3; i <= y + 3; ++i) {
                for (int j = x - 3; j <= x + 3; ++j) {
                    if (if_in(i, j))
                    {
						if (i == y && x == j) {
							// 同一个点...就不要谈了
							continue;
						}
                        // 这个点在map中
                        if (map[i, j] >= 0)
                        {
                            float bbb = (map[i, j] - cur_value) / Mathf.Sqrt(Mathf.Pow((i - y), 2) + Mathf.Pow((j - x), 2));
                            if (bbb < min_value)
                            {
                                minx = j;
                                miny = i;
                                min_value = bbb;
                            }
                        }
                    }
				}
			}
//			if (minx == miny && minx == 30000) {
//				// 旁边都是墙，我也不知道怎么走
//				return null;
//			}

			var force_direc = new Force (minx-x, y-miny);
			force_direc.Normalize ();

////			 DEBUG: 对于父亲层次如果是 Stair，请给你一个反方向的力量
//			if (get_parent_script().gameObject.name.Contains("Stair")) {
//				var fx = force_direc;
//				fx.x = -force_direc.y;
//				fx.y = force_direc.x;
//				force_direc = fx;
//			}

			// 方向的单位矢量乘以常数
			return force_direc * potential_energy_field_constexpr;

		}

		void OnCollisionEnter2D (Collision2D other)
		{
			if (other.gameObject.CompareTag ("Blocking")) {
				Debug.Log ("Hey Blocking");
				BlockController block = (BlockController)other.gameObject.GetComponent<BlockController> ();
				Vector2 closest_point = block.get_closest_point (this);
				Rigidbody2D rb2 = GetComponent<Rigidbody2D> ();
				var vec2 = block.transform.position - this.transform.position;
				Vector2 v1 = new Vector2 (0, 0);
				var edirec = ((Vector2)this.transform.position - closest_point).normalized;
				v1.x = edirec.y; v1.y = edirec.x;
				if (VectorMultiply(v1, vec2)  > 0) {
					v1.x = -v1.x;
					v1.y = -v1.y;
				}
//				Debug.Log (v1);
				rb2.velocity = (float)this.exc_speed * v1;
			} 
			else if (other.gameObject.CompareTag ("Dest")) {
				Debug.Log ("Find dest.");
				this.gameObject.SetActive (false);
			}

		}


	}

}