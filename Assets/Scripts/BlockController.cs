/*
 * 障碍物对象
 * 刚体
 */ 
using System.Collections;
using System.Collections.Generic;

using UnityEngine;



namespace SimuUtils
{
	public class BlockController : MonoBehaviour {
		public static List<BlockController> blocks = new List<BlockController>();
		private Bounds bounds;
		ChildObjects father_containers;
//		private Rigidbody2D valid_rb;

		// Use this for initialization
		void Start () {
			
			var daddy = transform.parent;
			if (!daddy) print("Object has no parent");
			var script = daddy.GetComponent<BackgroundController>();
			if (!script) print("Parent has no EnemyData script");
			father_containers = script.childObjects;
			father_containers.blocks.Add (this);

			HelperScript.change_z (this);

			Debug.Log ("Add a Block.");

			SpriteRenderer render = GetComponent<SpriteRenderer> ();
//			valid_rb = GetComponent<Rigidbody2D> ();
			bounds = render.bounds;
		}

		public double get_distance_to_human(HumanController human){
			return bounds.SqrDistance (human.transform.position);
		}

		public Vector2 get_closest_point(HumanController human) {
			return bounds.ClosestPoint (human.transform.position);
		}
		public Vector2 get_closest_point(Rigidbody2D human) {
			return bounds.ClosestPoint (human.transform.position);
		}


	}
}