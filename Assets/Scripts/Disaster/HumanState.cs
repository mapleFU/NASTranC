using UnityEngine;
using System.Collections;

public abstract class HumanState
{
	private Transform human;
	public HumanState(Transform human) {
		this.human = human;
	}
	// 获取行人的最大速度
	public abstract float get_max_speed ();
	// 更新时刻, 每个事件步的举动
	public abstract void when_updating();
}

