using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SimuUtils;

public class BoxClick : MonoBehaviour {
	private bool used;	// 是否使用过
	void Start() {
		used = false;
		this.GetComponent<Button> ().onClick.AddListener (
			delegate() {
				if (!used) {
					// TODO: 召唤一个计时器
					ConfigConstexpr.set_disaster();
					DisasterBase.StartDisaster();
					used = true;
				}
			}
		);
	}
}
