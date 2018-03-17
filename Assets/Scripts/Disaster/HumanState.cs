using UnityEngine;
using System.Collections;

public abstract class HumanState
{
	private Transform human;
	public HumanState(Transform human) {
		this.human = human;
	}
	// 获取行人的期望举动速度
	public abstract float get_exc_speed ();
	// 获取行人的初始化速度
	public abstract float get_init_speed ();
	// 更新时刻, 每个事件步的举动
	public abstract void when_updating();
	// 解除上一个状态
	public virtual HumanState unbindState() {
		return this;
	}
}

