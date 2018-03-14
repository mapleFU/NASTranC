using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetSize : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var rect = GetComponent<Rect> ();
		if (rect == null) {
			Debug.Log ("Rect is null!");
		} else {
			Debug.Log ("Height: " + rect.height + ", width: " + rect.width);
		}
			
	}

}
