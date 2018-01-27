using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndieModelTest : MonoBehaviour {

	private IEnumerator m_IE_IndividuationModel_Talk = null;
	void Start () {
		 Canvas canvas = gameObject.GetComponentInChildren<Canvas>();
        GameObject TalkShow = canvas.transform.GetChild(1).gameObject;
        TalkShow.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			IndividuationModel_Talk("Fuck You!",1f);
		}
	}
	public void IndividuationModel_Talk(string wordstring, float duatime = 5.0f)
    {
        if (m_IE_IndividuationModel_Talk != null)
		{
			return;
		}
            
        else
        {
            m_IE_IndividuationModel_Talk = IE_IndividuationModel_Talk(wordstring, duatime);
            StartCoroutine(m_IE_IndividuationModel_Talk);
        }
    }

    IEnumerator IE_IndividuationModel_Talk(string wordstring, float duatime = 5.0f)
    {
        Canvas canvas = gameObject.GetComponentInChildren<Canvas>();
        GameObject TalkShow = canvas.transform.GetChild(1).gameObject;
        TalkShow.SetActive(true);
        Text word = TalkShow.GetComponentInChildren<Text>();
        word.text = wordstring;
        yield return new WaitForSeconds(duatime);
        TalkShow.SetActive(false);
		m_IE_IndividuationModel_Talk = null;
    }
}
