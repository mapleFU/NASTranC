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
			var script = get_parent_script();
			father_containers = script.childObjects;
			father_containers.dests.Add (this);
			gameObject.layer = script.myLayer;

			HelperScript.change_z (this);

			Debug.Log ("Add a Dest.");
		}

	

		// if find "human" with tag
		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Human"))
			{
				other.gameObject.SetActive (false);
			}
		}

	}
}

