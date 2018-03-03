using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class SceneObject : MonoBehaviour
{
	public int m_PlanesID = 0;
    
    public float m_Amplitude = 4.5f;
    public float m_UnitDuration = 0.01f;
    public int m_LoopTimes = 2;
    public float m_FadeTime = 1f;
    public GameObject m_ColParticle;
    public Transform m_ColParticlePos;
    public SpriteRenderer m_Res;
    [Tooltip("Count must > 0")]
    public List<Sprite> m_AtkedSpSheet = new List<Sprite>();
    public bool m_CannotBeDestroy = false;
    public float m_RebornTime = 5f;

    private Sequence m_RotateSeq;
    private int m_CurAtkedIdx = 0;
    private readonly float m_HurtProtectTime = 0.5f;
    private bool m_Invincible = false;
    private IEnumerator m_IE_StopInvicible;

    void Awake()
    {
        Debug.Assert(m_AtkedSpSheet.Count > 0, "m_AtkedSpSheet.Count must > 0 !!!");
        m_CurAtkedIdx = 0;
        m_Res.sprite = m_AtkedSpSheet[m_CurAtkedIdx];
        m_Invincible = false;
        m_IE_StopInvicible = null;
        if (m_ColParticlePos == null) m_ColParticlePos = transform;

    }


    private void OnParticleCollision(GameObject other)
    {
        this.GetHurt();
    }

    public virtual bool GetHurt()
    {
        if (m_Invincible) return false;

        // Debug.Log(gameObject.name + "got hurt !!!");


        // sprite sheet
        if (m_CurAtkedIdx < m_AtkedSpSheet.Count - 1 || m_CannotBeDestroy)
        {
            m_CurAtkedIdx++;
            if (m_CurAtkedIdx >= m_AtkedSpSheet.Count)
                m_CurAtkedIdx = m_AtkedSpSheet.Count - 1;
            m_Res.sprite = m_AtkedSpSheet[m_CurAtkedIdx];

            // create particle
            if (m_ColParticle != null)
            {
                ParticleManager.instance.CreateParticle(m_ColParticle,
                    m_ColParticlePos.position, Vector3.zero);

            }
            // action
            if (m_RotateSeq == null)
            {
                m_RotateSeq = DOTween.Sequence();
                m_RotateSeq.Append(transform.DORotate(new Vector3(0, 0, m_Amplitude), m_UnitDuration))
                    .Append(transform.DORotate(new Vector3(0, 0, -m_Amplitude), 2 * m_UnitDuration))
                    .Append(transform.DORotate(new Vector3(0, 0, 0), m_UnitDuration))
                    .SetLoops(m_LoopTimes)
                    .OnComplete(delegate ()
                    {
                        m_RotateSeq = null;
                    });

            }
            // audio
            float pitch = 0.9f + UnityEngine.Random.Range(-0.3f, 0.3f);
            // AudioManager.instance.Play(Constants.BGM_Noise_CutTree_01);

            if (m_CurAtkedIdx == m_AtkedSpSheet.Count - 1 && !m_CannotBeDestroy)
            {

                // reborn
                StartCoroutine(IE_WaiteAndReborn());
            }

        }

        this.HurtProtect(m_HurtProtectTime);

        return true;
    }

    protected virtual void HurtProtect(float t)
    {
        if (m_IE_StopInvicible != null)
        {
            StopCoroutine(m_IE_StopInvicible);
            m_IE_StopInvicible = null;
        }

        m_Invincible = true;
        m_IE_StopInvicible = StopInvicible(t);
        StartCoroutine(m_IE_StopInvicible);
    }
    protected virtual IEnumerator StopInvicible(float t)
    {
        yield return new WaitForSeconds(t);
        m_Invincible = false;
    }

    private IEnumerator IE_WaiteAndReborn()
    {
        // yield return new WaitForSeconds(2f);

        // disable colliders
        Collider2D[] cols = gameObject.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D one in cols)
        {
            one.enabled = false;
        }
        // fade
        SpriteRenderer[] sps = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer one in sps)
        {
            one.DOFade(0f, m_FadeTime);
        }
        // navmesh
        NavMeshObstacle navmesh = gameObject.GetComponent<NavMeshObstacle>();
        if (navmesh != null)
        {
            navmesh.carving = false;
        }

        yield return new WaitForSeconds(m_FadeTime + m_RebornTime);

        m_CurAtkedIdx = 0;
        m_Res.sprite = m_AtkedSpSheet[m_CurAtkedIdx];

        foreach (SpriteRenderer one in sps)
        {
            one.DOFade(1f, m_FadeTime);
        }

        yield return new WaitForSeconds(m_FadeTime);

        // enable colliders
        foreach (Collider2D one in cols)
        {
            one.enabled = true;
        }
        // navmesh
        if (navmesh != null)
        {
            navmesh.carving = true;
        }

    }
}
