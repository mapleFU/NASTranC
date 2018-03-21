using UnityEngine;
using System.Collections;

/*
 * 用来制定是对象子对象对象的行为
 */ 
namespace SimuUtils {


	public class BaseChildObject : MonoBehaviour
	{
//		public GameObject parentObject;		// 父对象，可手动设置

		virtual public void Start() {
			var script = get_parent_script ();
			if (script == null) {
				if (this.CompareTag ("Human")) {
					var obj = GameObject.FindGameObjectWithTag ("Subway");
					Debug.Log ("OBJ: " + obj);
					MetroController m_s = obj.GetComponent<MetroController> ();
					script = m_s.get_parent_script ();

				}
			}
			if (gameObject == null) {
				Debug.Log ("Gameobject Not here");
			}
			gameObject.layer = script.gameObject.layer;
		}
		/*
		 * 获得父对象的脚本
		 */ 
		public BackgroundController get_parent_script() {
			
			BackgroundController bkg = null;
			Transform parentTrans = transform.parent;		// init parentTrans
			while (bkg == null) {
				bkg = parentTrans.GetComponent<BackgroundController> ();
				parentTrans = parentTrans.parent;
			}
			return bkg;
		}

		public Bounds get_box_render() {
			BoxCollider2D cur_render = GetComponent<BoxCollider2D> ();
			return cur_render.bounds;
		}
	}
}
