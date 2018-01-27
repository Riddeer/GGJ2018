using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneUIManager_Main : MonoBehaviour
{

    public static SceneUIManager_Main instance { get; private set; }

    // Game result
    [Header("Game Result")]
    public GameObject m_ResultDialog;
    public Text m_Result_Title;
    public Text m_Result_Content;

    void OnEnable()
    {
        if (SceneUIManager_Main.instance == null)
        {
            SceneUIManager_Main.instance = this;
        }
        else if (SceneUIManager_Main.instance != this)
        {
            Destroy(this);
        }
    }

    void OnAwake()
    {
        m_ResultDialog.SetActive(false);
    }

    void Update()
    {
        if (Global.instance == null) return;
        if (!Global.instance.m_GameStart) return;

        this.CheckResult();
    }

    private void CheckResult()
    {
        if (Global.instance.m_ResultType == ResultType.NULL) return;

        switch (Global.instance.m_ResultType)
        {
            case ResultType.Win:
                {
                    m_ResultDialog.SetActive(true);
                    m_Result_Title.text = "WINNER WINNER CHICKEN DINNER!";
                    m_Result_Content.text = "";
                }
                break;

            case ResultType.Loss:
                {
                    m_ResultDialog.SetActive(true);
                    m_Result_Title.text = "AGAIN?";
                    m_Result_Content.text = "";
                }
                break;

            default:
                break;
        }
    }

    public void ReturnToSceneExStart()
    {
        

    }

}
