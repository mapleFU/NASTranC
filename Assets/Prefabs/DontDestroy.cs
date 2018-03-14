using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyChildOnLoad (this.transform.gameObject);
	}

	public static void DontDestroyChildOnLoad( GameObject child )
	{
		Transform parentTransform = child.transform;

		// If this object doesn't have a parent then its the root transform.
		while ( parentTransform.parent != null )
		{
			// Keep going up the chain.
			parentTransform = parentTransform.parent;
		}
		GameObject.DontDestroyOnLoad(parentTransform.gameObject);
	}
}
