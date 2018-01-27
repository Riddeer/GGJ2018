using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
	public int m_PoolBullet = 50;

	private int m_CurBullet = 0;
	private Dictionary<GameObject, Vector3> m_DefaultChildrenScale =
		new Dictionary<GameObject, Vector3>();
	private float m_SuckedVal = 0f;

	void Awake()
	{
		m_SuckedVal = 0f;
	}

    // Use this for initialization
    void Start()
    {
		m_CurBullet = m_PoolBullet;

		SpriteRenderer[] childrenSps = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer one in childrenSps)
		{
			m_DefaultChildrenScale.Add(one.gameObject, one.transform.localScale);
		}
    }

	// update function
    public void BeSucked(Hero sucker)
	{
		m_SuckedVal += sucker.m_SuckSpeed * Time.deltaTime;
		if (m_SuckedVal > 1)
		{
			float floor = Mathf.Floor(m_SuckedVal);
			m_SuckedVal -= floor;
			if (m_CurBullet >= (int)floor)
			{
				m_CurBullet -= (int)floor;
				sucker.m_AllBullet += (int)floor;
			}
			
		}

		this.UpdateLeftBulletShow();
	}

	private void UpdateLeftBulletShow()
	{
		float sliderVal = (float)m_CurBullet / (float)m_PoolBullet;
		foreach (KeyValuePair<GameObject, Vector3> one in m_DefaultChildrenScale)
		{
			one.Key.transform.localScale = one.Value * sliderVal;
		}
	}
}
