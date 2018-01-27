using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class SortingOrderSetting : MonoBehaviour {

	public bool m_Update = false;

    private Vector2 m_Dir = Vector2.up;

	// Use this for initialization
	void Start () {
		this.SetOrder();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (!m_Update) return;

		this.SetOrder();
	}

	public void SetOrder()
	{
        // sprite
        SpriteRenderer[] rends_sp = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
		foreach(SpriteRenderer one in rends_sp)
		{
			Vector3 pos = one.transform.parent.position;
            int orderVal = this.GetOrderValByPosWorld(pos);
			one.sortingOrder = orderVal;

            // Debug.Log("+++ " + gameObject.name + "is Setting order :" );
            // Debug.Log("+++ " + one.gameObject.name + "'s order is " + orderVal );
		}

        // skeanimation
		MeshRenderer[] rends_mesh = gameObject.GetComponentsInChildren<MeshRenderer>(true);
		foreach(MeshRenderer one in rends_mesh)
		{
			Vector3 pos = one.transform.parent.position;
            int orderVal = this.GetOrderValByPosWorld(pos);
			one.sortingOrder = orderVal;
		}
	}

    // [-32768, 32767]
	private int GetOrderValByPosWorld(Vector3 pos)
    {
        Vector2 pos2 = new Vector2(pos.x, pos.y);
        float dot = Vector2.Dot(pos2, m_Dir);
        int orderVal = -1 * Mathf.FloorToInt(dot * 20f) + 16384;

        if (orderVal < -32768 || orderVal > 32767)
            Debug.LogWarning("WARRING!!! <" + pos + "> is out of range!");

        return orderVal;
    }

}
