using UnityEngine;
using System.Collections;

using SimuUtils;

public class PersonAdder : MonoBehaviour
{
	public static void LayerChange(HumanController human, BackgroundController bkg_layer) {
		var to_script = bkg_layer;
		human.gameObject.layer = to_script.gameObject.layer;
		var script = human;
		/*
		 *  need to change layer.
		 */ 
		// change the child and change camera
//		if (CameraScript.Instance.watched_player == script.transform) {
//			CameraScript.Instance.relayer_child_camera ();
//			Debug.Log ("Camera Layer num: " + CameraScript.Instance.gameObject.layer);
//		}

		float resizeRate = 0.6f;
		if (to_script.gameObject.name.Contains ("First")) {
			script.transform.localScale = new Vector3 (0.007142857f, 0.025f, 5.0f) * resizeRate;
		} else if (to_script.gameObject.name.Contains ("Second")) {
			script.transform.localScale = new Vector3 (0.00625f, 0.03333334f, 5.0f) * resizeRate;
		} else if (to_script.gameObject.name.Contains ("Stair")) {
			if (to_script.gameObject.name.Contains ("Mid")) {
				script.transform.localScale = new Vector3 (0.02f, 0.3f, 5f) * resizeRate;
			} else {
				script.transform.localScale = new Vector3 (0.02f, 0.3f, 5f) * resizeRate;
			}
		}
	

		script.gameObject.layer = to_script.gameObject.layer;
//		script.transform.rotation = rotate;
		HelperScript.change_z (script);
	}

	public static void checkScale(HumanController c, BackgroundController bkg_layer) {
		return;

//		}
	}
}

