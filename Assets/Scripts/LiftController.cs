/*
 * 电梯转移通道的模型
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace SimuUtils
{
	// 采用电梯后速度如何变化

	/*
	 * 脚本用于将人传送到新的层上
	 * 楼梯口对象
	 */ 
	public class LiftController: DestController
	{
		// 通向的GameObject, 不可以是null, 应该是BackGroundController控制的
		public GameObject to;
		public bool up_or_down;	// 为true则为up, 为false则为down。
		private BackgroundController bkg_ctrl;
		private BackgroundController to_script;

		public HashSet<LiftController> un_allowed_lifts = null;

		public override void Start()
		{
			
			var daddy = get_parent_script ();	// 获得父对象
//			Debug.Log ("Intended to create lifts in bkg: " + daddy.gameObject);
			base.Start ();

			if (!daddy.childObjects.dests.Contains (this)) {
				daddy.childObjects.dests.Add (this);
			}

//			bkg_ctrl = this.gameObject.transform.parent.gameObject.GetComponent<BackgroundController> ();
			bkg_ctrl = daddy;
			if (!bkg_ctrl) {
				// if background controller is null
//				Debug.Log ("Lift should be a component of the background!");
				return;
			} else {
				if (this.gameObject.name == "Lift") {
					Debug.Log ("Lift Controller init with aim " + to + " and father " + get_parent_script().gameObject);		
				}
				var lift = to.GetComponent<LiftController> ();
				lift.to = this.gameObject;
			}

		}

		// DO nothing in awake!!!!!
		public void Awake() {}

//		private static Interlocked atom_lock = new Interlocked();
		protected virtual void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Human"))
			{
				GameObject game_obj = other.gameObject;
				HumanController script = game_obj.GetComponent<HumanController> ();
				if (!script.lift_available(this)) {
					Debug.Log ("Lift not availabe");
					return;
				}
				string parent_name = script.get_parent_script ().name;

				if (parent_name.Contains ("First")) {
					if (script.in_disaster)
						return;
					if (!script.take_subway) {
						// 一楼，乘地铁
						Debug.Log("一楼，不乘地铁");
						return;
					}
				} else if (parent_name.Contains ("Second")) {

					if (script.take_subway && !script.in_disaster) {
						Debug.Log("二楼，乘地铁");
						return;
					}
				}
					

				if (script == null) {
					Debug.Log ("Bad Human! Human here don't have script!");
					return;
				}

				// 变换主从关系


				/*
				 * 更换父对象的代码
				 * 考虑父对象使用自己的生成算法
				 */ 
				// 更改game_obj的父对象
				// TODO: 搞清楚是不是要多重改变
				var rotate = other.transform.localRotation;
				var localscale = other.transform.localScale;

//				lock (atom_lock) 
				{
					bkg_ctrl.childObjects.humans.Remove (game_obj);
					// 获得Link对象的父脚本to_script
					var to_s = to.GetComponent<LiftController>();
					BackgroundController to_script = to_s.get_parent_script ();
					//				BackgroundController to_script = to.transform.parent.GetComponent<BackgroundController> ();

					if (to_script == null) {
						var lifts = GameObject.FindGameObjectsWithTag ("Lift");
						foreach (GameObject lift_go in lifts) {
							LiftController lc = lift_go.GetComponent<LiftController> ();
							if (lc.to == this) {
								to_script = lc.get_parent_script ();
							}
						}
						if (to_script == null) {
							Debug.LogError ("Empty1");
						}

					} 

					// 更新 used_lift
					LiftController to_controller = to.GetComponent<LiftController> ();
					if (to_controller != null) {
						script.used_list = un_allowed_lifts;
					}

					Rigidbody2D rb = GetComponent<Rigidbody2D> ();

					game_obj.transform.parent = to.transform.parent;

					to_script.add_person (game_obj);

					// 改变位置
					other.transform.position = to.transform.position;

					// 更换目标
					script.change_new_father_container (to_script);
					script.change_destine ();

					//				Debug.Log ("To : " + to_script.gameObject);
					game_obj.layer = to_script.gameObject.layer;
					/*
					 *  need to change layer.
					 */ 
					// change the child and change camera
					if (CameraScript.Instance.watched_player == script.transform) {
						CameraScript.Instance.relayer_child_camera ();
						Debug.Log ("Camera Layer num: " + CameraScript.Instance.gameObject.layer);
					}

//					Vector2 std_scale = HumanController.HUMAN_REAL_SCALE;

//					float x = 0.1f / to_script.get_x();
//					float y = 0.1f / to_script.get_y();
//					float z = 1.0f;
//					script.transform.localScale = new Vector3(x, y, z);
					if (to_script.gameObject.name.Contains ("First")) {
						script.transform.localScale = new Vector3 (0.007142857f, 0.025f, 5.0f);
					} else if (to_script.gameObject.name.Contains ("Second")) {
						script.transform.localScale = new Vector3 (0.00625f, 0.03333334f, 5.0f);
					} else if (to_script.gameObject.name.Contains ("Stair")) {
						if (to_script.gameObject.name.Contains ("Mid")) {
							script.transform.localScale = new Vector3 (0.016666667f, 0.25f, 5f);
						} else {
							script.transform.localScale = new Vector3 (0.016666667f, 0.25f, 5f);
						}
					}
					script.gameObject.layer = to_script.gameObject.layer;
					script.transform.rotation = rotate;
					HelperScript.change_z (script);
				
				}


			}
		}
			
	}
}
