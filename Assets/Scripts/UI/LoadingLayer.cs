using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingLayer : MonoBehaviour
{
	public bool m_Visible = false;
    public Text m_PointsText;
	public GameObject m_BgGo;

	void Start()
	{
		m_PointsText.text = "";
		m_PointsText.DOText("......", 3f).SetLoops(-1, LoopType.Restart);
	}

	void Update()
	{
		this.UpdateVisible(m_Visible);
	}

	private void UpdateVisible(bool val)
	{
		m_BgGo.SetActive(val);
	}

	public void SetVisible(bool val)
	{
		m_Visible = val;
	}
}
