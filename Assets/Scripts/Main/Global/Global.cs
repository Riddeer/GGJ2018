using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DG.Tweening;
using Hashtable = ExitGames.Client.Photon.Hashtable;
// using log4net.Config;

public enum ResultType
{
    NULL = -1,

    Win = 0,
    Loss = 1,


    SIZE
};

public class Global : MonoBehaviour
{
    // private static readonly log4net.ILog log =
    //     log4net.LogManager.GetLogger(
    //         System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public static Global instance { get; private set; }

    public Transform m_BattleGround;
    public GameObject m_AimMark;
    public Text m_TimeOutText;

    public Hero m_Hero_01;
    public Hero m_Hero_02;
    [HideInInspector]
    public List<Hero> m_Hero_All = new List<Hero>();
    [HideInInspector]
    public bool m_GameStart = false;
    public ResultType m_ResultType = ResultType.NULL;
    private readonly float m_TimeOutDur = 4f;

    protected void OnEnable()
    {
        // DontDestroyOnLoad(gameObject);

        if (Global.instance == null)
        {
            Global.instance = this;
        }
    }

    void Awake()
    {
        m_GameStart = false;

        m_Hero_All.Clear();
        m_Hero_All.Add(m_Hero_01);
        m_Hero_All.Add(m_Hero_02);
    }


    void Start()
    {
        // send game start time out
        float tarTime = Time.time + m_TimeOutDur;
        StartCoroutine(IE_WaiteForGameStart(tarTime));
    }

    // Use this for initialization

    void Update()
    {

        this.CheckGameResult();
    }

    private void CheckGameResult()
    {
        if (!m_GameStart) return;
        if (m_Hero_01 == null || m_Hero_02 == null) return;

        if (m_Hero_01.m_CurStatus == RoleStatus.Die ||
        m_Hero_02.m_CurStatus == RoleStatus.Die)
        {
            m_ResultType = ResultType.Loss;
        }
        else
        {

        }
    }

    public IEnumerator IE_WaiteForGameStart(double tarT)
    {
        // action
        double actionDur = tarT - m_TimeOutDur + 1f - Time.time;
        if (actionDur > 0)
        {
            m_TimeOutText.gameObject.SetActive(true);
            Sequence textSq = DOTween.Sequence();
            Sequence scaleSq = DOTween.Sequence();
            textSq.Append(m_TimeOutText.DOText("3", (float)actionDur));
            textSq.Append(m_TimeOutText.DOText("2", 1f));
            textSq.Append(m_TimeOutText.DOText("1", 1f));
            textSq.Append(m_TimeOutText.DOText("GO", 1f));
            textSq.AppendInterval(0.2f);
            textSq.OnComplete(delegate ()
            {
                m_TimeOutText.gameObject.SetActive(false);
            });
            scaleSq.Append(
                m_TimeOutText.transform.DOScale(1.5f, (float)actionDur));
            scaleSq.Append(
                m_TimeOutText.transform.DOScale(1f, 0.2f));
            scaleSq.Append(
                m_TimeOutText.transform.DOScale(1.5f, 0.8f));
            scaleSq.Append(
                m_TimeOutText.transform.DOScale(1f, 0.2f));
            scaleSq.Append(
                m_TimeOutText.transform.DOScale(1.5f, 0.8f));
            scaleSq.Append(
                m_TimeOutText.transform.DOScale(1f, 0.2f));
            scaleSq.Append(
                m_TimeOutText.transform.DOScale(1.5f, 0.8f));
            scaleSq.Append(
                m_TimeOutText.transform.DOScale(0f, 0.2f));

        }

        while (tarT > Time.time)
        {
            yield return null;
        }

        this.GameStart();
    }

    public void GameStart()
    {
        m_GameStart = true;
        m_Hero_01.SetPlayerAliveStatus(true);
        m_Hero_02.SetPlayerAliveStatus(true);
    }

    public void GameOver()
    {
        CameraEffect.instance.CameraEffect_GameOver();
    }

}
