using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 专门创建能用于当作背景的对象
 * 对每个层设置了不同的Layer
 * 需要设置专门的Layer
 */ 
public class BackGroundCreator: BaseCreator {
	// 每个对象需要处在不同的层级中
	static int layer_num = 0;
	public int self_layer;	// 自己所在的层
	Vector2 bgsize;			// 背景的size

	// constructor
	public BackGroundCreator(Vector2 size, int grid_size, Transform prefab): base(prefab) {
		self_layer = layer_num;
	}

}
