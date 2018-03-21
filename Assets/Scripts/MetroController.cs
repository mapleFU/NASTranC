using UnityEngine;
using System.Collections;
using SimuUtils;
using UnityEngine;

public class MetroController : BaseChildObject
{
	// 人类的prefab, 用于创建
	public Transform humanPrefab;

	private Transform parentTransform;
	private BackgroundController p_script;
	private Bounds bound;
	private float height, width;
	private  System.Random rd;
    private Vector2 oc_direct;

    public override void Start() {
        base.Start();
        p_script = get_parent_script();
        parentTransform = p_script.transform;
        var ymin = p_script.ymin;

        if (Mathf.Abs(ymin - transform.position.y) < 0.1f)
        {
            oc_direct = new Vector2(0, -1);
        }
        else
        {
            oc_direct = new Vector2(0, 1);
        }

        bound = GetComponent<BoxCollider2D>().bounds;
        width = bound.size.x * transform.localScale.x;
        height = bound.size.y * transform.localScale.y;
        rd = new System.Random();
    }

    /*
	 * 地铁站会有人下车
	 * 在这里填写下车的逻辑
	 */
    public float down_time;		// 下车时间间隔
	public int per_wave;		// 每一波的人

	/*
	 * 被定时调用的人物生成函数
	 */ 
	private void invoked() {
		Invoke ("invoked", down_time);
		for (int i = 0; i < per_wave; ++i)
			add_person ();
	}

    /*
	 * 获取某人下车的位置
	 */
    private Vector3 generate_pos()
    {
        const float ocrate = 0.1f;
        Vector2 v2 = new Vector2((float)rd.NextDouble() * width + gameObject.transform.position.x,
                         (float)rd.NextDouble() * height + gameObject.transform.position.y) + oc_direct * ocrate;
        Vector3 v3 = v2;
        v3.z = -0.41f;
        return v3;
    }



    /*
	 * 某人下车
	 */
    private void add_person() {
//		Debug.Log ("My daddy: " + get_parent_script().transform);
		var pos = generate_pos ();
//		Debug.Log ("Pos = " + pos);
		var gameobj = Instantiate (humanPrefab, pos,
			Quaternion.identity, parentTransform);
		gameobj.transform.parent = p_script.transform;


//		Debug.Log ("Add person!");
//		Debug.Log ("Pos is " + gameobj.transform.position);
		gameobj.gameObject.layer = p_script.gameObject.layer;
		HumanController p_c = gameobj.GetComponent<HumanController> ();
		p_c.Start ();
	}

	void Awake()  {
		Invoke ("invoked", down_time);
	}
}

