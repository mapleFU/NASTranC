/*
 * 照相机的模型
 * 能够绑定于制定的对象
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using System.Collections;

using SimuUtils;

/*
 * Camera, 使其能够绑定对象
 */ 
public class CameraScript : MonoBehaviour {
	// 唯一的 Camera 脚本
	// 考虑使用单例模式
	public static CameraScript Instance;
	public Transform watched_player;	// 被观察的player

	// 自身维护的camera
	private Camera cam;

	public float turnSpeed = 10f;
	public float height = -10f;
	public float distance = 5f;

	private Vector3 offsetX;
	private Vector3 offsetY;

	void Awake() {
		Instance = this;
	}

	void Start () {
		cam = this.gameObject.GetComponent<Camera> ();

		offsetX = new Vector3 (0, height, distance);
		offsetY = new Vector3 (0, 0, distance);
	}

	void LateUpdate()
	{
		if (watched_player == null)
			return;
		offsetX = Quaternion.AngleAxis (Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetX;
		offsetY = Quaternion.AngleAxis (Input.GetAxis("Mouse Y") * turnSpeed, Vector3.right) * offsetY;
		transform.position = watched_player.position - offsetX - offsetY;
		transform.LookAt(watched_player.position);
	}

	/*
	 * 绑定在一个对象或者层级更换的时候
	 * 发布订阅模式？
	 */ 
	public void onBind(HumanController humanController) {
		Debug.Log ("Bind " + humanController.ToString ());
		watched_player = humanController.gameObject.transform;
		// using it to bind and change layer.
		relayer_child_camera();
	}

	public void relayer_child_camera() {
		
		cam.cullingMask = 1 << watched_player.gameObject.layer;
	}

	public void deBind() {
		if (watched_player.parent)
			watched_player = watched_player.parent;
		else {
			watched_player = null;
			// is it first zero?
			cam.cullingMask = 0;
		}
	}
}