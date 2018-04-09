/*
 * 照相机的模型
 * 能够绑定于制定的对象
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	void Awake() {
		Instance = this;
	}

	void Start () {
		cam = this.gameObject.GetComponent<Camera> ();

		foreach (Button button in GetComponents<Button>()) {
			button.gameObject.layer = watched_player.gameObject.layer;
		}
		if (watched_player) {
			var script = watched_player.GetComponent<HumanController> ();
			if (script)
				onBind (script);
			else
				relayer_child_camera ();
		}
	}

	[SerializeField]
	private float m_Speed = 10f; // 控制镜头跟踪时的速度，用于调整镜头额平滑移动，如果速度过大，极限情况下直接把目标位置赋给镜头，那么对于闪现一类的角色瞬移效果，将会带来不利的视觉影像

	public float dist = 5.0f;//摄像机到投影屏幕的距离，即Z轴距离
	public float h = 5.0f;//摄像机到目标物体的高度
	public float heightDamping = 2.0f;//摄像机每次抬高的高度
	public float rotationDamping = 3.0f;//摄像机每次旋转的角度

	void LateUpdate()
	{

		//如果目标物体为空，则不进行任何处理
		if (!watched_player)
			return;

		//计算摄像机需要旋转的角度和抬高的位置
		float wantedRotationAngle = watched_player.transform.eulerAngles.y;
		float wantedHeight = watched_player.transform.position.y + h;

		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		//缓慢调整旋转角度
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

		//缓慢调整摄像机高度
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		//将旋转矢量转换成旋转值
		Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

		//放置摄像机在目标物体后面
		transform.position = watched_player.transform.position;
		transform.position -= currentRotation * Vector3.forward * dist;

		//放置摄像机的高度
		//transform.position = new Vector3(transform.position.x,currentHeight,transform.position.z);
	}

	/*
	 * 绑定在一个对象或者层级更换的时候
	 * 发布订阅模式？
	 */ 
	public void onBind(HumanController humanController) {
//		Debug.Log ("Bind " + humanController.ToString ());
		watched_player = humanController.gameObject.transform;
		// using it to bind and change layer.
		cam.gameObject.layer = watched_player.gameObject.layer;
		foreach (Button button in GetComponents<Button>()) {
			button.gameObject.layer = watched_player.gameObject.layer;
		}
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