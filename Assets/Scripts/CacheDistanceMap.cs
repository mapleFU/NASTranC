using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SimuUtils;

public class CacheDistanceMap
{
	private BackgroundController controler;
	/**
	 * 对于这样的情况，表述的是
	 * key 施加力的一方
	 * value 受力的一方
	 * map 受力的一方收到的力
	 */ 
	// 距离的map
	private Dictionary<KeyValuePair<int, int>, float> humanDisMap; 

	public CacheDistanceMap(BackgroundController humanc) {
		controler = humanc;
		humanDisMap = new Dictionary<KeyValuePair<int, int>, float> ();
	}
		
	/**
	 * Update 并非是内部的update，
	 * 而只是一个同名的更新函数。
	 */ 
	void Update ()
	{
		// O(N^2) 的一个分离更新，用于缓存优化
		foreach (HumanController perHuman1 in this.controler.childObjects.humans) {
			foreach (HumanController perHuman2 in this.controler.childObjects.humans) {
				// uid pair
				int uid1 = perHuman1.getUID ();
				int uid2 = perHuman2.getUID ();
				if (uid1 == uid2) {
					continue;
				}
				float distance = Vector2.Distance (perHuman1.transform.position, perHuman2.transform.position);
				// <小 -- 大> ：pair
				if (uid1 > uid2) {
					int tmp = uid1;
					uid1 = uid2;
					uid2 = tmp;
				} 
				humanDisMap.Add (new KeyValuePair<int, int> (uid1, uid2), distance);
			}
		}
	}

//	public float getDistance(HumanController c1, HumanController c2) {
//		return getDistance (c1.getUID (), c2.getUID ());
//	}

//	public float getDistance(int uid1, int uid2) {
//		
//	}

	/**
	 * 
	 */ 
	private void adjustUid() {
		
	}
}

