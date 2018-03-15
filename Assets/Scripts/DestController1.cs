using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimuUtils
{
	public class DestController1 : MonoBehaviour {

		// Use this for initialization
		ChildObjects father_containers;

		void Start () {
//			var daddy = transform.parent;
//			if (!daddy) print("Object has no parent");
//			var script = daddy.GetComponent<BackgroundController>();
			var script = 
			if (!script) print("Parent has no EnemyData script");
			father_containers = script.childObjects;
			father_containers.dests.Add (this);
			gameObject.layer = script.myLayer;

			Debug.Log ("Add a Dest.");
		}



		// if find "human" with tag
		void OnTriggerEnter (Collider other)
		{
			if (other.gameObject.CompareTag ("Human"))
			{
				other.gameObject.SetActive (false);
			}

		}

	}
}
