/*
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

		private BackgroundController bkg_ctrl;

		public override void Start()
		{
			base.Start ();
			bkg_ctrl = this.gameObject.transform.parent.gameObject.GetComponent<BackgroundController> ();
			if (!bkg_ctrl) {
				// if background controller is null
				Debug.Log("Lift should be a component of the background!");
				return;
			}
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Human"))
			{
//				other.gameObject.SetActive (false);
				GameObject game_obj = other.gameObject;
				HumanController script = game_obj.GetComponent<HumanController> ();
				if (script == null) {
					Debug.Log ("Bad Human! Human here don't have script!");
					return;
				}

				// 变换主从关系
				bkg_ctrl.childObjects.humans.Remove (game_obj);
				// 获得Link对象的父脚本to_script
				BackgroundController to_script = to.transform.parent.GetComponent<BackgroundController> ();
				if (to_script == null) {
					to_script = to.transform.parent.GetComponent<StairController> ();
					Debug.Log ("Empty1");
				} 
				if (to_script == null) {
					to_script = to.transform.parent.GetComponent<EscalatorController> ();
				}
//				if (to_script == null) {
//					to_script = to.transform.parent.GetComponent<StairController> ();
//				}

				/*
				 * 更换父对象的代码
				 * 考虑父对象使用自己的生成算法
				 */ 
				// 更改game_obj的父对象
				// TODO: 搞清楚是不是要多重改变
				var rotate = other.transform.localRotation;
				var localscale = other.transform.localScale;

				game_obj.transform.parent = to.transform.parent;
				script.gameObject.transform.parent = to.transform.parent;
				to_script.add_person (game_obj);
				Debug.Log (game_obj + " change father to " + to_script.gameObject + 
					" which layer num is " + to_script.gameObject.layer);

				// 改变位置
				other.transform.position = to.transform.position;
				// 更换目标

				script.change_new_father_container (to_script);
				script.change_destine ();


				game_obj.layer = to_script.myLayer;
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
