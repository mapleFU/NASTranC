using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimuUtils;

/*
 * 控制CrossChannel
 * 楼梯在dests里面
 * 可以通过 dests[i].GetType() == typeof(LiftController) 判断是不是楼梯
 */ 
public class CrossChannelController : BaseChildObject {
	// 连接的 UpArea 对象
	public Transform to;
	public bool up_or_down;		// 楼梯是上是下
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
		HashSet<LiftController> reversed_equal_lists = new HashSet<LiftController> (); 	// 反向的eqlist
//		Debug.Log ("Create cross channel in " + script.gameObject);
		foreach (Transform myTrans in transform) {
			switch (myTrans.name) {
			case "MidLift":
			case "Lift":
				mid = myTrans.GetComponent<LiftController> ();
				mid.initCrossChannel ();
				mid.initStair ();
				reversed_equal_lists.Add (mid);
				break;
			case "UpLift":
				up = myTrans.GetComponent<LiftController> ();
				up.initCrossChannel ();
				up.initElescator (true);
				reversed_equal_lists.Add (up);
				break;
			case "Lift (2)":
				up = myTrans.GetComponent<LiftController> ();
				up.initCrossChannel ();
				up.initElescator (false);
				reversed_equal_lists.Add (up);
				break;
			case "DownLift":
				down = myTrans.GetComponent<LiftController> ();
				down.initCrossChannel ();
				down.initElescator (false);
				reversed_equal_lists.Add (down);
				break;
			case "Lift (1)":
				down = myTrans.GetComponent<LiftController> ();
				down.initCrossChannel ();
				down.initElescator (true);
				reversed_equal_lists.Add (down);
				break;
			default:
				break;
			}
		}



//		Debug.Log ("My mid is " + mid + " up is " + up);
		foreach (Transform childTrans in to ) {
			switch (childTrans.name) {
			case "SquareStairMid":
				get_aim (mid, childTrans.gameObject, equal_lists);
				break;
			case "SquareStairUp":
				get_aim (up, childTrans.gameObject, equal_lists);
				break;
			case "SquareStairDown":
				get_aim (down, childTrans.gameObject, equal_lists);
				break;
			default:
				break;
			}
		}
		/*
		 *  正向反向填充eqlist.
		 */ 
		foreach (LiftController lc in reversed_equal_lists) {
			lc.un_allowed_lifts = equal_lists;
		}
		foreach (LiftController lc in equal_lists) {
			lc.un_allowed_lifts = reversed_equal_lists;
		}
		return;
	}

	private void get_aim(LiftController lift1, GameObject stair, HashSet<LiftController> eqlist) {
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
			eqlist.Add (goto_left.gameObject.GetComponent<LiftController> ());
		} else {
			
//			Debug.Log ("Attach right lift.");
			lift1.to = goto_right.gameObject;
			goto_right.to = lift1.gameObject;
			eqlist.Add (goto_right.gameObject.GetComponent<LiftController> ());
		}

		return;
	}
}
