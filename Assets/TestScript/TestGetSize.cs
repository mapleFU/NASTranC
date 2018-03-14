using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetSize : MonoBehaviour
{
	// https://answers.unity.com/questions/960170/how-to-find-width-and-height-of-sprite-object-unit.html
	public Transform prefab;

	RectTransform rt;

	// Use this for initialization
	void Start ()
	{
		Debug.Log ("Create a TESTGETSIZE");
		rt = (RectTransform)transform;
		if (rt == null) {
			Debug.Log ("Rt error");
		} else {
			Debug.Log (rt.rect.width + " and " + rt.rect.height);
		}
	}

	void FixedUpdate ()
	{
		const float add = 1.0f;

		if (Input.GetButtonDown("Fire1")) {
			Debug.Log ("Clicked.");
			//			var pos = rt.position;

			rt.sizeDelta = new Vector2 (rt.rect.width + 1, rt.rect.height + 1);
//			rt.rect.width = add + rt.rect.width;
			Debug.Log (rt.rect.width + " and " + rt.rect.height);
//			rt.localScale 
			// only this is valid.
			rt.localScale *= 2;

			create_and_init ();
		}
	}

	// create prefab
	void create_and_init ()
	{
		// https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
		// http://docs.manew.com/Manual/InstantiatingPrefabs.html

		Transform clone;
		clone = Instantiate (prefab, new Vector2(2,3), transform.localRotation) as Transform;


	}
}
