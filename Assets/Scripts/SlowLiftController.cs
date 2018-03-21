using UnityEngine;
using System.Collections;

using SimuUtils;

/*
 *  进入之后速度的改变
 */ 
public class SlowLiftController : LiftController
{
	public float delta_rate;

	protected override void OnTriggerEnter2D (Collider2D other)
	{
		/*
		 * 有可能物体在这上面是一个反向电梯，需要减速
		 */ 
//		public float delta_rate = 1.0f;
//		public void speed_init(GameObject other) {
//			var rb = other.GetComponent<Rigidbody2D> ();
//			rb.velocity *= delta_rate;
//		}

		base.OnTriggerEnter2D (other);
		if (other.CompareTag ("Human")) {
			var rb = other.GetComponent<Rigidbody2D> ();

		}
	}
}

