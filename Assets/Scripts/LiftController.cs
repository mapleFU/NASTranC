/*
 * 电梯转移通道的模型
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimuUtils
{
	// 采用电梯后速度如何变化
	public class LiftController: DestController
	{
		// 通向的GameObject, 不可以是null, 应该是BackGroundController控制的
		public GameObject to;

		private BackgroundController bkg_ctrl;

		public void Start()
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
				// 获得Link对象的父脚本
				BackgroundController to_script = to.transform.parent.GetComponent<BackgroundController> ();
				if (to_script == null) {
					to_script = to.transform.parent.GetComponent<StairController> ();
					Debug.Log ("Empty1");
				} 
				if (to_script == null) {
					to_script = to.transform.parent.GetComponent<EscalatorController> ();
				} 

				// 更改game_obj的父对象
				game_obj.transform.parent = to.transform.parent;
				to_script.add_person (game_obj);

				// 改变位置
				other.transform.position = to.transform.position;
//				Debug.Log ("new pos: " + other.transform.position);
				// 更换目标

				script.change_new_father_container ();
				script.change_destine ();

				HelperScript.change_z (script);

			}
		}
			
	}
}
