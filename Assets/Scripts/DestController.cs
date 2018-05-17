/*
 * 目标模型
 * 展示用矩形代替
 * 考虑需要采用直线
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimuUtils
{
	public class DestController : BaseChildObject {

		// Use this for initialization
		ChildObjects father_containers;

		public override void Start () {
//			var daddy = transform.parent;
//			if (!daddy) print("Object has no parent");
//			var script = daddy.GetComponent<BackgroundController>();
//			if (!script) print("Parent has no EnemyData script");
//			Debug.Log ("DestController ready to get parent script.");
			var script = get_parent_script();
			father_containers = script.childObjects;
			father_containers.dests.Add (this);
			gameObject.layer = script.myLayer;

			HelperScript.change_z (this);

//			Debug.Log ("Add a Dest in layer in " + father_containers.backGround);
		}

		// 可以设置的
		private int person_per_wave = 5;	// 每一波添加的人，可以设置成相关的别的常数
		private float add_time = 5;		// 时间相关的常数
		/*
		 * 
		 */ 

		private int add_person_cnt = 0;
		// 添加人
		void dest_add_person() {
			// 如果是不可以加人的状态，那么什么都不会发生
			++add_person_cnt;
			Debug.Log ("召唤准备！");
			for (int i = 0; i < person_per_wave; ++i) {
				if (!ConfigConstexpr.human_addable()) {
					// 什么都不召唤
					break;
				}
				var newman = HumanController.add_human(transform.position, get_parent_script().gameObject);
				if (newman == null) {
					Debug.LogError ("召唤失败");
				}
				newman.layer = gameObject.layer;
				HumanController new_script = newman.GetComponent<HumanController> ();
				PersonAdder.LayerChange (new_script, get_parent_script());
				new_script.take_subway = true;

			}
				
			Invoke ("dest_add_person", add_time);
		}

		virtual protected void Awake()  {
			// TODO: DEBUG and fill this
			Invoke ("dest_add_person", add_time);
		}

		// if find "human" with tag
		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Human"))
			{
				
				if (this.gameObject.layer != other.gameObject.layer)
					return;
				HumanController hc = other.GetComponent<HumanController> ();
				// 
				if (!ConfigConstexpr.get_instance ().has_disaster && hc.take_subway) {
					// 无灾难，坐地铁，跑路
					return;
				}
				if (CameraScript.Instance.watched_player == other.transform) {
					CameraScript.Instance.deBind ();
				}
				other.gameObject.SetActive (false);
				Debug.Log ("Human pass! in Layer" + other.gameObject.layer);
			}
		}

	}
}

