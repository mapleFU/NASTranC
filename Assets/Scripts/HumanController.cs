/*
 * 控制人类对象行为的模型
 * 圆形
 */ 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimuUtils;

namespace SimuUtils
{
	using Force = UnityEngine.Vector2;


	public class HumanController : BaseChildObject {
		private static int human_uid = 0;

		/**
		 * 输出人的uid
		 * 在表中表示力的存储和方向
		 */ 
		public int getUID() {
			return this.current_uid;
		}

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
		public static float MAX_DISASTER_BROADCAST = 1.0f;	// 人物时间步中传递灾害模式的半径
		// 进入灾害模式
		public void to_disaster_mode () {
			// 自我调整模式
			in_disaster = true;	
			// 颜色变化
			SpriteRenderer sr = GetComponent<SpriteRenderer> ();
			Color c = sr.color;
			bool colored = false;
			Color resp_c = Color.black;
			take_subway = false;
			bool color_in_mode = false, color_in_upper_mode = false;
			foreach (Color cr in HumanController.corespond_color) {
				if (cr == c) {
					resp_c = cr;
					colored = true;
					break;
				}
			}
			if (colored) {
				sr.color = HumanController.upper_corespond_color [HumanController.color_to_num [resp_c]];
			} else {
				foreach (Color cr in HumanController.upper_corespond_color) {
					if (cr == c) {
						resp_c = cr;
						return;
					}
				}
				// 否则变绿
				sr.color = Color.green;
			}

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
        /*
		 * 判断给出的楼梯是可以使用的
		 */
        public bool lift_available(LiftController c)
        {
            // at first is null.
            if (used_list == null)
                return true;
            return !used_list.Contains(c);
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

		private const double mutirate = 0.3f;
		// 正常情况下的 初始化速度和期望速度 
		private const double reality_excspeed_mean = 1.26f * mutirate;
		private const double reality_excspeed_stddev = 0.16f * mutirate;
		private const double disaster_excspeed_mean = 1.56f * mutirate;
		private const double disaster_excspeed_stddev = 0.06f * mutirate;



		private const double reality_inispeed_mean = 1.0f;
		private const double reality_inispeed_stddev = 0.2f;
		private const double disaster_inispeed_mean = 1.16f;
		private const double disaster_inispeed_stddev = 0.2f;
		//		public double reality_inispeed_mean;
		//		public double reality_inispeed_stddev;


		// running constexpr 
		private const double TIME_EXPR = 0.5;
		private const double MAX_COUNT_DISTANCE = 0.5; 		// max_count_dis
		private const double MAX_MENTALLY_DISTANCE = 1.6;
		private const double MAX_B2P_DISTANCE = 0.75;
		private const double P2P_CONSTEXPR = 0.2;
		private const double B2P_CONSTEXPR = 3;
		private const double B2P_PHY_CONST = 1000;



		// rigidbody
		public Rigidbody2D rb;

		// 这个人对应的uid
		private int current_uid;
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
			rb.velocity = dir *0.01f;
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

		private static Dictionary<BackgroundController, bool> init_dict = new Dictionary<BackgroundController, bool>();

		private static void static_init(BackgroundController bkg_script) {
			if (init_dict.ContainsKey(bkg_script))
				return;
			
			int cnt = 0;

			foreach (float[,] farr in bkg_script.APF16) {
				if (farr == null)
					continue;

				float_arr [cnt++] = farr;	
				if (cnt == 6) {
					break;
				}
			}
			foreach (float[,] farr in float_arr) {
				Debug.Log ("Farr " + farr);
			}
			max_fixed_apf_count = cnt;

			corespond_color [0] = Color.blue;
			corespond_color [1] = Color.yellow;
			corespond_color [2] = Color.cyan;
			corespond_color [3] = Color.magenta;
			corespond_color [4] = new Color (0.2f, 0.3f, 0.4f);
			corespond_color [5] = new Color (1.0f, 0.3f, 0.8f);

			upper_corespond_color [0] = new Color (0.2f, 0.3f, 0.4f);
			upper_corespond_color [1] = new Color (1.0f, 0.3f, 0.8f);
			upper_corespond_color [2] = new Color (0.5f, 1.0f, 0.25f);
			upper_corespond_color [3] = new Color (0.7f, 0.5f, 0.3f);

			// 构造对应的映射字典。
			for (int i = 0; i < 4; i++) {
				color_to_num [corespond_color [i]] = i;
				upper_color_to_num [upper_corespond_color [i]] = i;
			}
		}

		/*
		 * 脚本初始化  
		 */ 
		private static int max_fixed_apf_count = 0;
		private Force last_pfe;
		public override void Start () {
			if (rnd.NextDouble() >= 0.7) {
				this.take_subway = true;
			}
			current_uid = human_uid++;

			var daddy = get_parent_script();
			//			Debug.Log ("Human's daddy :" + daddy.gameObject);
			father_containers = daddy.childObjects;
			father_containers.humans.Add (this);



			if (daddy.gameObject.name.Contains ("Second")) {
				// 需要根据固定的apf来生成对应的场。
				generate_fixed_apf();
			}

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


		void setNonstatic(GameObject collider) {
			collider.GetComponent<Rigidbody2D>().isKinematic = false;
		}

		IEnumerator MyFunction(GameObject collider, float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			setNonstatic (collider);
			is_fallen = false;
		}

		// 是否摔倒 -- 没有摔倒
		private bool is_fallen = false;
		private static float FALLEN_TIME = 2.0f;
		// 人物跌倒
		private void fallen_down() {
			Debug.Log("Fallen for person(" + human_uid + ") in place " + current_position);
			this.GetComponent<Rigidbody2D>().isKinematic = true;
			StartCoroutine (MyFunction (this.gameObject, FALLEN_TIME));
		}

		// Update is called once per frame
		// count force 
		private float K_CONST = 10.0f;
		// 在Update里面打好表，每次即时读取表中的距离等数据
		// 总消耗N人*N读 O(N^2), 但是少了N方次计算？
		public void Update () {
			if (in_disaster) {
				foreach (HumanController c in father_containers.humans) {
					if (c == this || c.gameObject.activeSelf == false)
						continue;
					if (c.in_disaster)
						continue;
					// 直接的距离值
					var distance = Vector2.Distance (c.current_position, current_position);
					if (distance <= MAX_DISASTER_BROADCAST) {
						// 小于距离
						c.to_disaster_mode();
					}
				}
			}

			if (is_fallen) {
				return;
			}

//			PersonAdder.checkScale (this, get_parent_script ());

			// update speed
			cur_speed = rb.velocity.magnitude;
			if(cur_speed >= exc_speed)//如果算出来的当前速度大于预期速度 那么就要调整大小
			{
				rb.velocity = rb.velocity* (1/cur_speed)*exc_speed;
			}
			Force 
			pfe = potential_energy_field () ;
			last_pfe = pfe;
			Force
			p2p = count_p2p(),
			b2p = -count_b2p() ;

			// DEBUG
			p2p += 100.0f*(p2p-  (p2p.x * pfe.x + p2p.y * pfe.y) * pfe / pfe.magnitude);
			Force all = /*fhe*/  b2p + pfe;
			all += Vector2.Angle (all, rb.velocity) * K_CONST * all.normalized;
			this.rb.AddForce(all);
			// DEBUG

//			Debug.Log ("POS: " + this.current_position + " with "+ get_parent_script().pos2mapv(this.current_position) + " and force " + all + " with father name " + 
//				get_parent_script().gameObject.name);
			
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

		private double LOWER_P2P_DELTA_LENGTH = 0.3;
		private float P2P_K_CONST = 1.0f;
		private double LOWEST_P2P_DELTA_LENGTH = 0.18;
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
				Force cur_f = (float)(P2P_CONSTEXPR * Math.Exp ((-Vector3.Distance(this.transform.position, player.transform.position) - this.radius - player.radius) / MAX_MENTALLY_DISTANCE))
					* get_direction_to_dest ();
				if (distance <= LOWER_P2P_DELTA_LENGTH) {
					// 小于距离
					cur_f += Vector2.Angle(player.rb.velocity, this.rb.velocity) * P2P_K_CONST * cur_f.normalized;
				} 
				if (distance <= LOWEST_P2P_DELTA_LENGTH) {
					float xf = Current_velocity.x;
					float yf = Current_velocity.y;
					Force added = new Force (-yf, xf);
				
					cur_f += added.normalized * cur_f.magnitude * 3;
				}
				mental_force += cur_f;
			}
			//			Debug.Log (humans + " human in this layer.");
			return mental_force;
		}

		// Problem: 线性障碍物计算
		// DEBUG
		private Force count_b2p()
		{
			Force f = new Force (0, 0);

			foreach (BlockController controller in father_containers.blocks) {
				
				double current_distance = controller.get_distance_to_human (this) - this.radius;

				if (current_distance >= 0.18)
					continue;
				
				Vector2 closest_point = controller.get_closest_point (this);
				// Still this way...?
				Vector2 b2p_direction = ((Vector2)this.transform.position - closest_point).normalized;
				// 心理接触力 修正
				var cur_force = (float)(B2P_CONSTEXPR * Math.Exp (-current_distance / 5)) * b2p_direction;
				f += -cur_force;
				//// 身体接触力


			}

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
		public static float potential_energy_field_constexpr=5.0f;

		/*
		 * 根据人的属性得到需要的apf
		 */ 
		// 上一次可以的apf
		private float[,] last_used_apf = null;
		private float[,] find_min_apf(List<float[,]> apf_list, int cur_x, int cur_y, ref int arg) {
			// TODO:test and DEBUG in this function
			arg = -1;
			if (apf_list == null) {
				Debug.LogError ("May because that apf_list is null.");
			}

			float[,] min_arr = null;
			float min_value = 300000;		// 设置一个很大的初值
			int cnt = 0;
			try {
				foreach (float[,] arr in apf_list) {
					float aaa = arr[cur_x, cur_y];
					if (aaa < min_value && aaa >= 0) {
						min_value = arr [cur_x, cur_y];
						min_arr = arr;
						arg = cnt;
					}
					++cnt;
				}
			} catch (Exception e) {
				Debug.LogError ("Error pos: " + transform.position + " and mapv"
					+ get_parent_script().pos2mapv(transform.position) + "cur_x, cur_y" + cur_x + ","+ cur_y);
				this.gameObject.SetActive (false);

			}

			if (min_arr == null) {
				min_arr = last_used_apf;
			}

			if (min_arr != null)
				last_used_apf = min_arr;

			return min_arr;
		}

		/**
		 * 是否有固定的apf
		 * @如果fixed_apf 为true 则--  直接选择固定的apf
		 * 				 为false 则--  正常进行
		 */ 
		private bool fixed_apf;
		/**
		 * 选中的固定的apf
		 */ 
		private float[,] fixed_already_apf;
		/// <summary>
		/// 随机数生成用rnd
		/// </summary>
//		private Random rnd = new Random();

		private static float[][,] float_arr = new float[6][,];
		private static Color[] corespond_color = new Color[6];
		private static Dictionary<Color, int> color_to_num = new Dictionary<Color, int>();
		private static Color[] upper_corespond_color = new Color[4];
		private static Dictionary<Color, int> upper_color_to_num = new Dictionary<Color, int>();

		public void destroy_fixed_apf() {
			if (!fixed_apf) {
				return;
			}
			// destory the bad apf
			fixed_apf = false;
			var sr = GetComponent<SpriteRenderer> ();
			sr.color = Color.black;
		}

		// 按照一定的可能性，生成对应的fixed_apf
		private void generate_fixed_apf() {
			BackgroundController bkg_script = get_parent_script ();
			// 如果小与这个概率
			if (rnd.NextDouble() <= 0.15) {
				fixed_apf = true;
				int to_choose = (int)(rnd.NextDouble() * max_fixed_apf_count);
				fixed_already_apf = float_arr [to_choose];

				// change color
				var renderer = this.GetComponent<SpriteRenderer>();
				// TODO:颜色转变(为什么不用状态及)
				if (in_disaster) {
					renderer.color = upper_corespond_color [to_choose];
				} else {
					renderer.color = corespond_color [to_choose];
				}

			}
		}
		private float[,] get_apf() {
			if (fixed_apf) {
				if (fixed_already_apf != null) {
					Debug.Log ("Fixed apf!");

					return fixed_already_apf;
				}
			}
			// 背景脚本
			BackgroundController bkg_script = get_parent_script ();
			float[,] needed_apf;
			var pos = bkg_script.pos2mapv (transform.position);

			// 获得现在的x y坐标
			int cur_x = (int)pos.x, cur_y = (int)pos.y;
			bool if_print = false;
			string debug_str;
			int pos_in_list = 0;
			if (bkg_script.gameObject.name.Contains ("Second")) {
				if_print = true;
			}

			if (!bkg_script.on_stair_or_not ()) {
				// 在楼层上
				// 楼梯扶梯可用
				if (has_app) {
					// 有app
					if (take_subway) {
						// 要乘车
						if (in_disaster) {
							needed_apf = bkg_script.APF01;

							debug_str = ("Choose APF01");
						} else {
							needed_apf = bkg_script.APF11;
							debug_str =  ("Choose APF11");
						}
					} else {
						if (in_disaster) {
							needed_apf = bkg_script.APF02;
							debug_str =  ("Choose APF02");
						} else {
							needed_apf = bkg_script.APF12;
							debug_str =  ("Choose APF12");
						}

					}

				} else {

					List<float[,]> search_list;

					// 根据有无app选择对应的list
					if (take_subway) {
						if (in_disaster) {
							search_list = bkg_script.APF05;
							debug_str =  ("Choose APF05");
						} else {
							search_list = bkg_script.APF15;
							debug_str =  ("Choose APF15");
						}

					} else {
						if (in_disaster) {
							search_list = bkg_script.APF06;
							debug_str =  ("Choose APF06");
						} else {
							search_list = bkg_script.APF16;
							debug_str =  ("Choose APF16");
						}

					}

					const float MIN_V = 30.0f;
					// 找到最小的数组
					float[,] min_arr = find_min_apf (search_list, cur_y, cur_x, ref pos_in_list);
					needed_apf = min_arr;
					// 先直接找最近的出口吧，这个逻辑恨坑的
					init_destine ();
				}
				if (if_print) {
					Debug.Log (debug_str + " and pos " + pos_in_list);
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
			float cur_value = 0.0f;
			try {
				cur_value = map[y, x];	
			} catch (IndexOutOfRangeException e) {
				gameObject.SetActive (false);
			}

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

			var force_direc = new Force (minx-x, y-miny);
			force_direc.Normalize ();

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