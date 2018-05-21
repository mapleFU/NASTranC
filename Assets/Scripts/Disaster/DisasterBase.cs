using UnityEngine;
using System.Collections;

using SimuUtils;

namespace SimuUtils {
	public class DisasterBase : BaseChildObject
	{
		/**
		 * 
		 */ 
		private float DELTA_RATE = 0.1f;
		/**
		 *  更新圆形的半径
		 */ 
		private void _update_round() {
			Vector3 current_scale = transform.localScale;
			current_scale.x = (float)((1 + DELTA_RATE) * current_scale.x);
			current_scale.y = (float)((1 + DELTA_RATE) * current_scale.y);
			transform.localScale = current_scale;
			Invoke ("_update_round", 0.9f);
		}
			
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
			Invoke ("_update_round", 1.3f);
		}
		private BackgroundController father_script;
		// 火灾最大传播空间
		private float max_broadcast = 0.7f;

		const float FIRE_EXPR = 15.0f;
		Vector2 fire_force_to_human(HumanController human_c) {
			//  /r^2
			return (this.transform.position - human_c.transform.position) * 
				FIRE_EXPR / Vector2.Distance(human_c.transform.position, this.transform.position);
		}

		void Update() {
			if (father_script == null)
				return;
			foreach (HumanController human in father_script.childObjects.humans) {
				Rigidbody2D rb = human.GetComponent<Rigidbody2D> ();

				if (Vector2.Distance (human.transform.position, transform.position) <= max_broadcast) {
					rb.AddForce (fire_force_to_human(human));
					Debug.Log ("灾害爆发，让世界感受痛苦！");
					human.to_disaster_mode ();
//					StartCoroutine (human_to_disaster_mode(human, 0.5f));
				} 
			}

		}

		IEnumerator human_to_disaster_mode(HumanController c, float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			c.to_disaster_mode ();
			// Now do your thing here
		}
	}


}
