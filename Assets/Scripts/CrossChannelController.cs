using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimuUtils;

/*
 * 控制CrossChannel
 */ 
public class CrossChannelController : MonoBehaviour {
	// 连接的 UpArea 对象
	public Transform to;
	private LiftController up;
	private LiftController mid;
	private LiftController down;
	/*
	 * 在Awake的时候会帮助子对象设置脚本
	 * 子对象有三个，
	 */ 
	void Awake () {
		foreach (Transform myTrans in transform) {
			switch (myTrans.name) {
			case "MidLift":
				mid = myTrans.GetComponent<LiftController> ();
				break;
			case "UpLift":
				up = myTrans.GetComponent<LiftController> ();
				break;
			case "DownLift":
				down = myTrans.GetComponent<LiftController> ();
				break;
			default:
				break;
			}
		}

		foreach (Transform childTrans in to ) {
			switch (childTrans.name) {
			case "SquareStairMid":
				get_aim (mid, childTrans.gameObject);
				break;
			case "SquareStairUp":
				get_aim (up, childTrans.gameObject);
				break;
			case "SquareStairDown":
				get_aim (down, childTrans.gameObject);
				break;
			default:
				break;
			}
		}
	}

	private void get_aim(LiftController lift1, GameObject stair) {
		LiftController goto_left = null, goto_right = null;

		foreach (Transform child in stair.transform) {
			switch (child.name) {
			case "LeftLift":
			case "Lift (1)":
				goto_left = child.gameObject.GetComponent<LiftController> ();
				break;
			case "RightLift":
			case "Lift":
				goto_right = child.gameObject.GetComponent<LiftController> ();
				break;
			default:
				break;
			}
		}
		if (lift1.up_or_down == false) {
			lift1.to = goto_left.gameObject;
			goto_left.to = lift1.gameObject;
		} else {
			lift1.to = goto_right.gameObject;
			goto_right.to = lift1.gameObject;
		}

		return;
	}
}
