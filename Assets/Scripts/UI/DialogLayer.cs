using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogLayer : MonoBehaviour
{
	public bool m_Visible = false;
	public float m_ShowTime = 2f;
	public GameObject m_BgGo;
	public Text m_Text;
	public Transform m_DialogTrans;

	private float m_LeftShowingTime = 0;
	private bool m_IsShowing = false;
	private List<string> m_WaitingTexts = new List<string>();

	void Start()
	{
		m_IsShowing = false;

		// this.SetBgImgCallbacks();
	}

	void Update()
	{
		this.UpdateVisible(m_Visible);
	}

	private void SetBgImgCallbacks()
	{
		Image bgImg = GetComponent<Image>();
        UIEventListener bgListener = bgImg.gameObject.AddComponent<UIEventListener>();

        bgListener.OnDown += delegate (GameObject gb)
        {
			Debug.Log("UIListener: on down !!!");
            this.CloseDialog();
        };
	}

	public void SetVisible(bool val)
	{
		m_Visible = val;
	}

    public void UpdateVisible(bool val)
	{
		// set all img and text
		// Image[] imgs = GetComponentsInChildren<Image>();
		// foreach (Image one in imgs)
		// {
		// 	one.enabled = val;
		// }
		// Text[] texts = GetComponentsInChildren<Text>();
		// foreach (Text one in texts)
		// {
		// 	one.enabled = val;
		// }

		m_BgGo.SetActive(val);
	}

	public void ShowText(string val)
	{
		if (m_IsShowing)
		{
			this.AddWaitingText(val);
			return;
		}

		m_IsShowing = true;
		this.SetVisible(true);
		m_Text.text = val;

		m_DialogTrans.localScale = Vector3.zero;
		m_DialogTrans.DOScale(Vector3.one, 0.2f);

		StartCoroutine(IE_WaiteForShowing(m_ShowTime));
	}

	private IEnumerator IE_WaiteForShowing(float t)
	{
		m_LeftShowingTime = t;

		while(m_LeftShowingTime > 0)
		{
			m_LeftShowingTime -= Time.deltaTime;
			yield return null;
		}

		m_DialogTrans.DOScale(Vector3.zero, 0.2f)
		.OnComplete( delegate () 
		{
			m_IsShowing = false;
			this.CheckWaitingText();
		});
	}

	public void CloseDialog()
	{
		Debug.Log("Close dialog");
		m_LeftShowingTime = 0;
	}

	private void CheckWaitingText()
	{
		if (m_WaitingTexts.Count <= 0)
		{
			this.SetVisible(false);
			return;
		}

		string headString = m_WaitingTexts[0];
		m_WaitingTexts.RemoveAt(0);
		this.ShowText(headString);
	}

	private void AddWaitingText(string val)
	{
		m_WaitingTexts.Add(val);
	}
}
