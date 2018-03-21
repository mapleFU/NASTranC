﻿using UnityEngine;
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
			
			BackgroundController bkg;
			if (parentObject != null) {
				// parent o is not null, then run script.
				bkg = parentObject.GetComponent<BackgroundController> ();
//				Debug.Log ("parentObject in " + this.ToString () + " is null!");
			} else {
				// parent is null, then write it
				var parentTrans = this.transform.parent;
				if (parentTrans == null) {
					Debug.LogError ("A " + this.GetType() + " doesn't have a parent.");
					return null;
				}
				// 再次递归向上的查找
				// 父对象不是bkgC而且有再上一层的父对象
				BackgroundController d_script = parentTrans.gameObject.GetComponent<BackgroundController>();
				while ( d_script != null && d_script.GetType() != typeof(BackgroundController)
					&& parentTrans.parent != null) {
					parentTrans = parentTrans.parent;
					d_script = parentTrans.gameObject.GetComponent<BackgroundController>();
				}
				parentObject = this.transform.parent.gameObject;
				bkg = parentObject.GetComponent<BackgroundController> ();
				if (bkg == null) {
					if (this.gameObject.name.Contains ("Block")) {
//						Debug.Log ("This area is runned.");
						parentObject = transform.parent.parent.gameObject;
						return parentObject.GetComponent<BackgroundController> ();
					}
					Debug.LogError ("parentObject in " + this.ToString() + "'s father is still null!"+ "(from " + parentObject + ")");
					// TODO:fix this bug
					var sl = GameObject.Find("SecondLayer");
					parentObject = sl;
					return sl.GetComponent<BackgroundController> ();
				}
			}
			// TODO:find out what's wrong
//			gameObject.layer = bkg.myLayer;
			return bkg;
		}

		public Bounds get_box_render() {
			BoxCollider2D cur_render = GetComponent<BoxCollider2D> ();
			return cur_render.bounds;
		}
	}
}
