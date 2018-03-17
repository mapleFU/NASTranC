using UnityEngine;
using System.Collections;

/*
 * 用来制定是对象子对象对象的行为
 */ 
namespace SimuUtils {


	public class BaseChildObject : MonoBehaviour
	{
		public GameObject parentObject;		// 父对象，可手动设置

		virtual public void Start() {
			var script = get_parent_script ();
			gameObject.layer = script.myLayer;
		}
		/*
		 * 获得父对象的脚本
		 */ 
		public BackgroundController get_parent_script() {
			
			BackgroundController bkg;
			if (parentObject != null) {
				bkg = parentObject.GetComponent<BackgroundController> ();
				Debug.Log ("parentObject in " + this.ToString () + " is null!");
			} else {
				var parentTrans = this.transform.parent;
				if (parentTrans == null) {
					Debug.LogError ("A " + this.GetType() + " doesn't have a parent.");
				}
				// 再次递归向上的查找
				while (!(parentTrans.parent is BackgroundController)) {
					parentTrans = parentTrans.parent;
				}
				parentObject = this.transform.parent.gameObject;
				bkg = parentObject.GetComponent<BackgroundController> ();
				if (bkg == null) {
					Debug.LogError ("parentObject in " + this.ToString() + "'s father is still null!");
				}
			}
			gameObject.layer = bkg.myLayer;
			return bkg;
		}

		public Bounds get_box_render() {
			SpriteRenderer cur_render = GetComponent<SpriteRenderer> ();
			return cur_render.bounds;
		}
	}
}
