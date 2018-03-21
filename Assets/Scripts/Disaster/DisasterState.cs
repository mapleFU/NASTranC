using UnityEngine;
using System.Collections;

public class DisasterState : HumanState
{
	public DisasterState(Transform human): base(human) {}
	// 获取行人的期望举动速度
	public override float get_exc_speed () {
		return 0;
	}
	// 获取行人的初始化速度
	public override float get_init_speed () {
		return 0;
	}
}

