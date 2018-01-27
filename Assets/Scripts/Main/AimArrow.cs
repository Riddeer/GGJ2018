using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AimArrowType
{
    NULL = -1,
    Arrow = 0,
    ArrowNCircle = 1,
    SIZE
}
public class AimArrow : MonoBehaviour
{
    public GameObject m_AimArrow;
    public Hero m_OpaHero;
    private Vector3 castPos2Role = Vector3.zero;
    private SpriteRenderer m_SkillPointing_CastDis;

    void Awake()
    {
        m_SkillPointing_CastDis = gameObject.GetComponentInChildren<SpriteRenderer>();

    }
    void Start()
    {
        InputManager.instance.OnButtonDown += UpdateInteChoisePointing;
        InputManager.instance.OnButtonLeft += SetMySelfActiveFalse;
        gameObject.SetActive(false);
    }
    void Update()
    {
        // UpdateInteChoisePointing();
    }

    private void SetMySelfActiveFalse(Hero player)
    {
        if(player != m_OpaHero) return;
        gameObject.SetActive(false);
    }
    private void UpdateInteChoisePointing(Hero player)
    {
        if(player != m_OpaHero) return;
        if (m_OpaHero.m_CurWeapon.m_AimType == AimArrowType.NULL) return;
        gameObject.SetActive(true);
        // m_AimArrow.SetActive(false);
        gameObject.transform.position = m_OpaHero.GetMidPos();
        castPos2Role = m_OpaHero.GetCurFaceVec();
        castPos2Role = castPos2Role.normalized;
        //scale
        float dis = 0f;
        if (m_OpaHero.m_CurWeapon.m_AimType == AimArrowType.Arrow)
        {
            m_AimArrow.SetActive(false);
            RaycastHit2D hit = Physics2D.Raycast(
               m_OpaHero.GetMidPos(), castPos2Role,
               m_OpaHero.m_CurWeapon.m_AlertRange, m_OpaHero.m_TarLayer);

            if (hit.point == Vector2.zero)
            {
                dis = m_OpaHero.m_CurWeapon.m_AlertRange;
            }
            else
            {
                dis = Vector3.Distance(hit.point, m_OpaHero.GetMidPos() + new Vector3(0, 0.4f, 0));

                dis = dis * 0.5f;
            }

            // dis = Mathf.Clamp(dis,1,m_CurWeapon.m_AlertRange);
            dis = Mathf.Clamp(dis, 0, 5.25f);
        }
        else if (m_OpaHero.m_CurWeapon.m_AimType == AimArrowType.ArrowNCircle)
        {
            dis = Vector3.Distance(new Vector3(0, 0.4f, 0), m_OpaHero.GetCurFaceVec());
            m_AimArrow.SetActive(true);
            m_AimArrow.transform.position = (Vector3)m_OpaHero.GetCurFaceVec() + m_OpaHero.GetMidPos() + new Vector3(0, 0.4f, 0);
            dis = dis * 0.5f;
        }

        m_SkillPointing_CastDis.transform.parent.SetLocalScale_Y(dis);
        // rotation
        float angle = Mathf.Atan2(castPos2Role.x, castPos2Role.y) * Mathf.Rad2Deg;

        angle = -angle;
        gameObject.transform.SetLocalEulerAngles_Z(angle);
    }
}
