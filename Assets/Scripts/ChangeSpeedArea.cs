using UnityEngine;
using System.Collections;

using SimuUtils;
/*
 * 需要更改速度常数，变换速度
 * 用在电梯 闸机等区域
 */ 
public class ChangeSpeedArea :  BaseChildObject
{
	// 速度常数，在其中的物体速度乘以它
	public float speed_expr;

	/*
	 *  判断二者是否是同一个父亲
	 */ 
	private bool same_father(GameObject other) {
		return other.transform.parent == transform.parent;
	}

	void OnTriggerEnter2D(Collider2D other) {
		GameObject collider_object = other.gameObject;
		if (collider_object.CompareTag ("Human")) {
			if (same_father (collider_object)) {
				Rigidbody2D rigid = collider_object.GetComponent<Rigidbody2D> ();
				rigid.velocity *= speed_expr;
			}
		}
	}

//	void OnTriggerExit2D(Collider2D other)
//	{
//		GameObject collider_object = other.gameObject;
//		if (collider_object.CompareTag ("Human")) {
//			if (same_father (collider_object)) {
//				Rigidbody2D rigid = collider_object.GetComponent<Rigidbody2D> ();
//				rigid.velocity /= speed_expr;
//			}
//		}
//	}
}

