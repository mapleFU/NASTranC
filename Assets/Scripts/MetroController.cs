using UnityEngine;
using System.Collections;
using SimuUtils;

// TODO: finish this
public class MetroController : BaseChildObject
{
	// 人类的prefab, 用于创建
	public static Transform humanPrefab;

	private Transform parentTransform;
	private BackgroundController p_script;
	private Bounds bound;
	private float height, width;

	public override void Start() {
		base.Start ();
		p_script = get_parent_script ();
		parentTransform = p_script.transform;
		bound = get_box_render ();
		width = bound.size.x;
		height = bound.size.y;
	}

	/*
	 * 地铁站会有人下车
	 * 在这里填写下车的逻辑
	 */ 
	public void Update() {
		
	}

	/*
	 * 获取某人下车的位置
	 */ 
	private Vector2 generate_position() {
//		return null;
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

