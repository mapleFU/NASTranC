using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimuUtils
{
	public class HelperScript
	{
		public static void change_z(MonoBehaviour script)
		{
			var pos = script.transform.position;
			pos.z = -0.41f;
			script.transform.position = pos;
//			var daddy = script.transform.parent;
//			if (!daddy) Debug.Log("Object has no parent");
//			var father_script = daddy.GetComponent<BackgroundController>();
//			if (!script) Debug.Log("Parent has no EnemyData script");
//			var curpos = script.transform.position;
//			curpos.z = father_script.transform.position.z;
//			script.transform.position = curpos;
		}
	}


}

