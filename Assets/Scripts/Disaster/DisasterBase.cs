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
				new Vector3(-3.0f, -1.5f, -0.5f), Quaternion.identity, father.transform) as GameObject;
			instance.transform.localScale = new Vector3 (0.05f, 0.5f, 1.0f);
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
		public float max_broadcast = 4.0f;
		void Update() {
			foreach (HumanController human in father_script.childObjects.humans) {

				if (Vector2.Distance(human.transform.position, transform.position) <= max_broadcast) {
					human.in_disaster = true;
				}
			}
		}
	}


}
