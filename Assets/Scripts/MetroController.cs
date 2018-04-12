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
	private float my_generate_y;	// 生成的y的位置
    public override void Start() {
        base.Start();
        p_script = get_parent_script();
		if (p_script == null) {
			Debug.LogError ("We cannot find p_script in it " + gameObject.name);
		}
        parentTransform = p_script.transform;
        var ymin = p_script.ymin;

        if (Mathf.Abs(ymin - transform.position.y) < 0.1f)
        {
            oc_direct = new Vector2(0, -2.8f);
			my_generate_y = -2.8f;
        }
        else
        {
            oc_direct = new Vector2(0, -0.2f);
			my_generate_y = -0.2f;
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
	private float wait_time = 10.0f;		// 每波车等待的事件
    private float down_time = 20.0f;		// 允许上车时间间隔
	private int per_wave = 3;		// 每一波的人

	private bool can_go_up;		// 可以上车

	/*
	 * 被定时调用的人物生成函数
	 */ 
	private void invoked() {
		// 可以上车了
		can_go_up = true;
		Invoke ("invoked", down_time);
		// 取消go up
		Invoke ("set_cannot_goup", wait_time);
		for (int i = 0; i < per_wave; ++i)
			add_person ();
		// 延时执行

	}

	private void set_cannot_goup() {
		can_go_up = false;
	}

    /*
	 * 获取某人下车的位置
	 */
    private Vector3 generate_pos()
    {
        const float ocrate = 0.2f;
		Vector3 v2 = new Vector3 ((float)rd.NextDouble () * width + gameObject.transform.position.x,
			my_generate_y, 0.41f);
        Vector3 v3 = v2;
        v3.z = -0.41f;
        return v3;
    }



    /*
	 * 某人下车
	 */
    private void add_person() {
		if (!ConfigConstexpr.human_addable ()) {
			return;
		}
//		Debug.Log ("My daddy: " + get_parent_script().transform);
		var pos = generate_pos ();
		Debug.Log ("Add Pos = " + pos + " with father " + this.gameObject.name);
		var gameobj = HumanController.add_human(pos, parentTransform.gameObject);
		gameobj.transform.parent = p_script.transform;	// reset father.

		gameobj.gameObject.layer = p_script.gameObject.layer;
		HumanController p_c = gameobj.GetComponent<HumanController> ();
		p_c.transform.parent = parentTransform;
		p_c.take_subway = false;
		p_c.Start ();
	}

	void Awake()  {
		//TODO: DEBUG!!!!
		Invoke ("invoked", down_time);
//		Invoke ("set_cannot_goup", wait_time + down_time);
	}

	public void OnTriggerEnter2D (Collider2D other)
	{
		
		if (!can_go_up || ConfigConstexpr.get_instance().has_disaster)
			return;
		if (other.CompareTag ("Human")) {
			HumanController hc = other.GetComponent<HumanController> ();
			if (hc.take_subway) {
				other.gameObject.SetActive (false);
				Debug.Log ("Some one take the subway and go away");
			} 
		}
	}

	void OnTriggerStay2D( Collider2D other) {
		OnTriggerEnter2D (other);
	}
}

