﻿/*
 * 电梯转移通道的模型
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
					Debug.Log ("Lift Controller init with aim " + to + " and father " + this.parentObject);		
				}
				var lift = to.GetComponent<LiftController> ();
				lift.to = this.gameObject;
			}

		}



		protected virtual void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Human"))
			{
				
//				other.gameObject.SetActive (false);
				GameObject game_obj = other.gameObject;
				HumanController script = game_obj.GetComponent<HumanController> ();
				if (!script.lift_available(this)) {
					return;
				}

//				// 每个 cross channel 是等同的。
//				Debug.Log ("Print eqlist in trigger.This is: " + this);
//				if (script.used_list != null) {
//					foreach (LiftController controller in script.used_list) {
//						Debug.Log (controller);
//					}
//				} else {
//					Debug.Log ("No used list.");
//				}
//

				if (script == null) {
					Debug.Log ("Bad Human! Human here don't have script!");
					return;
				}

				// 变换主从关系
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

				/*
				 * 更换父对象的代码
				 * 考虑父对象使用自己的生成算法
				 */ 
				// 更改game_obj的父对象
				// TODO: 搞清楚是不是要多重改变
				var rotate = other.transform.localRotation;
				var localscale = other.transform.localScale;

				// 更新 used_lift
				LiftController to_controller = to.GetComponent<LiftController> ();
				if (to_controller != null) {
					// the to object is still a lift
					// should we handle the slow lift?
//					script.used_lift = to_controller;

					script.used_list = un_allowed_lifts;
					//					to_controller.speed_init (script.gameObject);
				}


				game_obj.transform.parent = to.transform.parent;
				script.gameObject.transform.parent = to.transform.parent;
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



				script.transform.localScale = localscale;
				script.transform.rotation = rotate;
				HelperScript.change_z (script);

			}
		}
			
	}
}
