using UnityEngine;
using System.Collections;

/*
 * 这是一个基本的装饰器
 */ 
public class LiftState : HumanState
{
	HumanState wrapped;
	public float lift_arg;
	public LiftState(HumanState t): base(t) {
		wrapped = t;
	}
	public override float get_exc_speed () {
		return 0;	
	}
	// 获取行人的初始化速度
	public override float get_init_speed () {
		return 0;
	}
}

