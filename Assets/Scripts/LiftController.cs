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

		// 是否是扶梯 和 是否在这个属性上被填充
		private bool filled_elescator = false;
		// 是否是elescator
		private bool is_elescator;	
		// 是向上的扶梯
		private bool elescator_up;
		// 是否在交错的楼梯口内
		private bool in_cross_channel = false;
		public void initCrossChannel() {
			in_cross_channel = true;
		}

		public void initElescator(bool up_elescator) {
			if (filled_elescator) {
				return;
//				throw new System.SystemException ("Filled already!");
			}
			is_elescator = true;
			elescator_up = up_elescator;
			filled_elescator = true;
		}
		public void initStair() {
			if (filled_elescator) {
				return;
//				throw new System.SystemException ("Filled already!");
			}
			is_elescator = false;
			filled_elescator = true;
		}

		private bool IsElescator {
			get { 
				if (!filled_elescator) {
//					Debug.Log ("Not cc and uninited");
					return false;
//					throw new System.SystemException ("Unfilled the elescator attr");
				}
				return is_elescator;
			}
		}
		private bool IsStair {
			get { 
				if (!filled_elescator) {
//					Debug.Log ("Not cc and uninited");
					return false;
//					throw new System.SystemException ("Unfilled the elescator attr");
				}
				return !is_elescator;
			}	
		}

		public HashSet<LiftController> un_allowed_lifts = null;

		public override void Start()
		{
			
			var daddy = get_parent_script ();	// 获得父对象
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
				
				Debug.Log (this.gameObject.name + " and father " + get_parent_script().gameObject + " and is ele " + IsElescator + " and is stair " + IsStair + " and ele up " + elescator_up );		

				var lift = to.GetComponent<LiftController> ();
				lift.to = this.gameObject;
			}

		}

		// DO nothing in awake!!!!!
		public void Awake() {}

//		private static Interlocked atom_lock = new Interlocked();
		private int pass_by_cnt = 0;
		protected virtual void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Human"))
			{
				GameObject game_obj = other.gameObject;
				HumanController script = game_obj.GetComponent<HumanController> ();
				if (script == null) {
					Debug.Log ("Bad Human! Human here don't have script!");
					return;
				}

				string parent_name = script.get_parent_script ().name;

				if (in_cross_channel && !ConfigConstexpr.get_instance().has_disaster) {
					if (parent_name.Contains ("First")) {
						if (script.in_disaster)
							return;
						if (!script.take_subway) {
							// 一楼，乘地铁
							return;
						} 
						// 判断是向上的扶梯还是向下的扶梯
						if (IsElescator && elescator_up) {
							// 向上扶梯
							return;
						}
					} else if (parent_name.Contains ("Second")) {

						if (script.take_subway && !script.in_disaster) {
							Debug.Log("二楼，乘地铁");
							return;
						}
						if (IsElescator && elescator_up) {
							return;
						}
					}
				}
					
				// 变换灾害形象
//				script.destroy_fixed_apf();

				// 变换主从关系


				/*
				 * 更换父对象的代码
				 * 考虑父对象使用自己的生成算法
				 */ 
				// 更改game_obj的父对象
				var rotate = other.transform.localRotation;
				var localscale = other.transform.localScale;

				{
					++pass_by_cnt;
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

					game_obj.layer = to_script.gameObject.layer;
					/*
					 *  need to change layer.
					 */ 
					script.transform.rotation = rotate;
					PersonAdder.LayerChange (script, to_script);
				}


			}
		}
			
	}
}
