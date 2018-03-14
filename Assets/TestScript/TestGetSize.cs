using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetSize : MonoBehaviour {
	// https://answers.unity.com/questions/960170/how-to-find-width-and-height-of-sprite-object-unit.html

	// Use this for initialization
	void Start () {
		RectTransform rt = (RectTransform)transform;
		if (rt == null) {
			Debug.Log ("Rt error");
		} else {
			Debug.Log (rt.rect.width + " and " + rt.rect.height);
		}
	}

}
