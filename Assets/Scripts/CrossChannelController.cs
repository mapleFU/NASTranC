using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimuUtils;

/*
 * 控制CrossChannel
 */ 
public class CrossChannelController : BaseChildObject {
	// 连接的 UpArea 对象
	public Transform to;
	public bool up_or_down;
	private LiftController up;
	private LiftController mid;
	private LiftController down;
	/*
	 * 在Awake的时候会帮助子对象设置脚本
	 * 子对象有三个，
	 */ 
	void Awake () {
		var script = get_parent_script ();
		HashSet<LiftController> equal_lists = new HashSet<LiftController> ();
//		Debug.Log ("Create cross channel in " + script.gameObject);
		foreach (Transform myTrans in transform) {
			switch (myTrans.name) {
			case "MidLift":
			case "Lift":
				mid = myTrans.GetComponent<LiftController> ();
				mid.parentObject = script.gameObject;
				equal_lists.Add (mid);
				break;
			case "UpLift":
			case "Lift (2)":
				up = myTrans.GetComponent<LiftController> ();
				up.parentObject = script.gameObject;
				equal_lists.Add (mid);
				break;
			case "DownLift":
			case "Lift (1)":
				down = myTrans.GetComponent<LiftController> ();
				down.parentObject = script.gameObject;
				equal_lists.Add (mid);
				break;
			default:
				break;
			}
		}
		// 每个 cross channel 是等同的。
		foreach (LiftController controller in equal_lists) {
			controller.un_allowed_lifts = equal_lists;
		}

//		Debug.Log ("My mid is " + mid + " up is " + up);
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
		lift1.up_or_down = up_or_down;
		if (up_or_down == false) {
			lift1.to = goto_left.gameObject;
			goto_left.to = lift1.gameObject;
		} else {
			
//			Debug.Log ("Attach right lift.");
			lift1.to = goto_right.gameObject;
			goto_right.to = lift1.gameObject;
		}

		return;
	}
}
