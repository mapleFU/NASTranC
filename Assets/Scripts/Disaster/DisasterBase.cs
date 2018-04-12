using UnityEngine;
using System.Collections;

using SimuUtils;

namespace SimuUtils {
	public class DisasterBase : BaseChildObject
	{
		public static void StartDisaster() {
			Debug.LogWarning ("Disaster !!!!!!!!!!!!!Boooooooooooooom!!!!!!!!!");
			var father = GameObject.Find ("SecondLayer");
			GameObject instance = Instantiate(Resources.Load("GamePrefab/Disaster", typeof(GameObject)),
				new Vector3(-6.0f, -1.5f, -0.5f), Quaternion.identity, father.transform) as GameObject;
			instance.transform.localScale = new Vector3 (0.05f, 0.25f, 1.0f);
			instance.layer = father.layer;

			ConfigConstexpr.set_disaster ();
		}
		// 生成的事件
		//	public float generated_time;
		void Start() {
			base.Start ();
			father_script = get_parent_script ();
		}
		private BackgroundController father_script;
		// 火灾最大传播空间
		private float max_broadcast = 6.0f;

		const float FIRE_EXPR = 15.0f;
		Vector2 fire_force_to_human(HumanController human_c) {
			//  /r^2
			return (this.transform.position - human_c.transform.position) * 
				FIRE_EXPR / Vector2.Distance(human_c.transform.position, this.transform.position);
		}

		void Update() {
			foreach (HumanController human in father_script.childObjects.humans) {
				Rigidbody2D rb = human.GetComponent<Rigidbody2D> ();
				rb.AddForce (fire_force_to_human(human));


				if (Vector2.Distance (human.transform.position, transform.position) <= max_broadcast) {
					Debug.Log ("灾害爆发，让世界感受痛苦！");
					human.to_disaster_mode ();
				} 
			}
		}
	}


}
