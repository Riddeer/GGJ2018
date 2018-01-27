using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandle : MonoBehaviour
{

    public GameObject ShockWave;
	private Transform m_Transform;
	private BoxCollider2D m_ParentBoxCollider2D;
	void Awake()
	{	m_Transform = transform;
		m_ParentBoxCollider2D = m_Transform.parent.GetComponent<BoxCollider2D>();
	}
	void Start()
	{
		if (ShockWave == null)
		{
			return;
		}
		ShockWave.SetActive(false);
	}
    public void CreateDmg()
    {
       
        CreateShockWave();
		StartDmg();
		CameraEffect.instance.Shake(0.1f,14,0.1f);
    }
    public void CreateShockWave()
    {
		if (ShockWave == null)
		{
			return;
		}
		ShockWave.SetActive(true);
    }
	private void StartDmg()
	{
		m_ParentBoxCollider2D.enabled = true;
	}
	public void DestroySelf()
	{
		Destroy(m_Transform.parent.gameObject);
	}
}
