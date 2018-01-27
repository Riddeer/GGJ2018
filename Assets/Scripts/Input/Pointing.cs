using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointing : MonoBehaviour
{
    [HideInInspector]
    public static Pointing instance { get; private set; }

    public float m_MinCastPos2Castle = 4f;
    public GameObject m_Pointing;
    public SpriteRenderer m_ClickArea;
    public Color m_ClickAreaColor_Right;
    public Color m_ClickAreaColor_Wrong;
    public Transform m_HeightArrow;

    [HideInInspector]
    public bool m_CastLegal = false;
    [HideInInspector]
    public Vector3 m_CastPos2World = Vector3.zero;


    private float m_CastHeight = 0f;
    private Tweener m_ClickAreaColorAct;


    protected void OnEnable()
    {
        // DontDestroyOnLoad(gameObject);

        if (Pointing.instance == null)
        {
            Pointing.instance = this;
        }

    }

    void Start()
    {
        this.ResetPointing();

        // set action of click area
        m_ClickArea.color = m_ClickAreaColor_Right;
        m_ClickAreaColorAct = m_ClickArea.DOFade(0.2f, 1f)
            .SetLoops(-1, LoopType.Yoyo);
        m_ClickAreaColorAct.Pause();
    }

    public void SetPointing(Vector2 screenPos)
    {
        
    }


    public void ResetPointing()
    {
        m_CastLegal = false;
        m_CastPos2World = Vector3.zero;

        m_CastHeight = 0f;

    }

    void LateUpdate()
    {
        
    }

}
