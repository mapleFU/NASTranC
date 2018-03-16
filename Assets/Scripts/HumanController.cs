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
		// 每一个格子格点的大小，在一个项目中是固定的
		public static float grid_size;

		static private System.Random rnd = new System.Random ();

		private ChildObjects father_containers;
		// 使用过的lift
		LiftController used_lift = null;

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

		// 初始化速度和期望速度 
		private const double reality_excspeed_mean = 1.26f;
		private const double reality_excspeed_stddev = 0.36f;
		public double 行人速度期望v1ß;
		public double 行人速度反差σ1;

		private const double reality_inispeed_mean = 1.0f;
		private const double reality_inispeed_stddev = 0.2f;
//		public double reality_inispeed_mean;
//		public double reality_inispeed_stddev;


		// running constexpr 
		private const double TIME_EXPR = 0.5;
		private const double MAX_COUNT_DISTANCE = 1.1;
		private const double MAX_MENTALLY_DISTANCE = 1.6;
		private const double MAX_B2P_DISTANCE = 0.75;
		private const double P2P_CONSTEXPR = 1.0;
		private const double B2P_CONSTEXPR = 2.0;		
		private const double B2P_PHY_CONST = 25;


		// rigidbody
		public Rigidbody2D rb;

		private double weight;			// 质量-> Rigid2d.mass
		private double radius;			// 半径
		private double exc_speed;	// 预期速度
		private double cur_speed;	// 现有速度
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
			dest.GetComponent<DestController> ();

		}

		// Use this for initialization
		// 初始化目的地
		private void init_destine()
		{
//			if (all_dests == null) {
//				
////				all_dests = father_containers.dests;
//				all_dests = GameObject.FindGameObjectsWithTag ("Dest");
//			}
			GameObject min_dst = null;
			float min_length = float.MaxValue;
			Debug.Log (father_containers.dests);
//			this.transform.gameObject
			foreach (MonoBehaviour behaviour in father_containers.dests) {

				if (behaviour == used_lift) 
					continue;

				GameObject dst = behaviour.gameObject;
				float value = Vector3.Distance (this.transform.position, dst.transform.position);
				if (value < min_length) {
					min_length = value;
					min_dst = dst;
				}
			}

			// init dest
			dest = min_dst;
		}

		/*
		 * 对人的半径进行初始化
		 */ 
		private void init_radius()
		{

			radius = RandomWithBound (reality_radius_lower, reality_radius_upper);
//			Debug.Log ("radius: " + radius);
			// init scale with actual value
			Vector3 current_scale = transform.localScale;
			current_scale.x = (float)(radius * unity_radius_scale);
			current_scale.y = (float)(radius * unity_radius_scale);
			transform.localScale = current_scale;
		}

		/*
		 * 对人的速度进行初始化
		 */ 
		private void init_speed()
		{

			exc_speed = RandomGussion (reality_excspeed_mean, reality_excspeed_stddev);
			cur_speed = RandomGussion (reality_inispeed_mean, reality_inispeed_stddev);
			// TODO: 初始化对应速度
			//		rb.velocity;
			Vector3 dir = dest.transform.position - transform.position;
			dir.z = 0;
			dir = dir.normalized;
			rb.velocity = dir * (float)cur_speed;
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
		public void change_new_father_container()
		{
//			var daddy = transform.parent;
//			if (!daddy) print("Object has no parent");
//			var script = daddy.GetComponent<BackgroundController>();
			var script = get_parent_script();
			if (!script) print("Parent has no EnemyData script");
			father_containers = script.childObjects;
			father_containers.humans.Add (this);
			gameObject.layer = script.myLayer;
		}

		/*
		 * 脚本初始化  
		 */ 
		public override void Start () {
			change_new_father_container ();

			rb = GetComponent<Rigidbody2D> ();
			// 添加自己的对象
//			humans.Add (this);
//			HelperScript.change_z (this);

			init_radius ();
			init_weight ();
			init_destine ();
			init_speed ();


			Debug.Log ("Add a Human.");
		}

		// Update is called once per frame

		// count force 
		void Update () {
//			if (!this.isActiveAndEnabled)
//				return;
			
			// update speed
			cur_speed = rb.velocity.magnitude;

			Force fhe = count_fhe (),
			p2p = count_p2p(),
			b2p = count_b2p();

			Force all = fhe + p2p + b2p;
			this.rb.AddForce (all);

			// 点击检测
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
			// DEBUG
//			int humans = 0;
			foreach (HumanController player in father_containers.humans) {
//				++humans;
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
		bool used = false;
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
//				Debug.Log ("Cur force " + cur_force);
				// 身体接触力
				double physic_distance = current_distance;
				if (physic_distance < 0.3) {
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