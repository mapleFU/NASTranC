using UnityEngine;
using System.Collections;
using SimuUtils;
using UnityEngine;

public class MetroController : BaseChildObject
{
	// 人类的prefab, 用于创建
	public static Transform humanPrefab;

	private Transform parentTransform;
	private BackgroundController p_script;
	private Bounds bound;
	private float height, width;
	private  System.Random rd;

	public override void Start() {
		base.Start ();
		p_script = get_parent_script ();
		parentTransform = p_script.transform;
		bound = get_box_render ();
		width = bound.size.x;
		height = bound.size.y;
		rd = new System.Random ();
	}

	/*
	 * 地铁站会有人下车
	 * 在这里填写下车的逻辑
	 */ 
	public static float down_time;		// 下车时间
	public static int per_wave;			// 每一波的人
	public void Update() {
		down_time -= Time.deltaTime;
		if (down_time < 0) {
			for (int i = 0; i < per_wave; ++i)
				add_person ();
		}
	}

	private Vector2 generate_pos() {

		return new Vector2 ((float)rd.NextDouble( ) * width + gameObject.transform.position.x,
			(float)rd.NextDouble( ) * height + gameObject.transform.position.y);
	}

	/*
	 * 获取某人下车的位置
	 */ 
	private Vector2 generate_position() {
		return new Vector2(width, height);
	}

	/*
	 * 某人下车
	 */ 
	private void add_person() {
		var gameobj = Instantiate (humanPrefab, generate_position(), transform.localRotation, parentTransform);
		gameobj.gameObject.layer = p_script.myLayer;
	}
}

