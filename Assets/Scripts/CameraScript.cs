/*
 * 照相机的模型
 * 能够绑定于制定的对象
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	public float turnSpeed = 10f;
	public float height = -10f;
	public float distance = 5f;

	private Vector3 offsetX;
	private Vector3 offsetY;

	void Start () {

		offsetX = new Vector3 (0, height, distance);
		offsetY = new Vector3 (0, 0, distance);
	}

	void LateUpdate()
	{
		Transform player = transform.parent;
		offsetX = Quaternion.AngleAxis (Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetX;
		offsetY = Quaternion.AngleAxis (Input.GetAxis("Mouse Y") * turnSpeed, Vector3.right) * offsetY;
		transform.position = player.position - offsetX - offsetY;
		transform.LookAt(player.position);
	}
}