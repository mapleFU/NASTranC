using UnityEngine;
using System.Collections;

public abstract class BaseCreator
{
	protected Transform prefab;
	public BaseCreator(Transform prefab) {
		this.prefab = prefab;
	}

	public abstract void create ();
}

