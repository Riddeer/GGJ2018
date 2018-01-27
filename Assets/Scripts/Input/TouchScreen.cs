using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchScreen : MonoBehaviour
{
    public InputManager m_InputMng;
    private Camera m_Cam;
    private bool m_IsTouching = false;

    // Use this for initialization
    void Awake()
    {
        m_Cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        this.CheckInput_UseCard();

    }

    private void CheckInput_UseCard()
    {
        if (!m_InputMng.CheckType(InputType.NULL) &&
            !m_InputMng.CheckType(InputType.Touch_Move) &&
            !m_InputMng.CheckType(InputType.Touch_Up))
        {
            return;
        }

        switch (m_InputMng.m_PlatformType)
        {
            case PlatformType.EditorAndWIN:
                {
                    m_InputMng.SetType(InputType.NULL);
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!this.IsPointerOverUIObject() &&
                            this.IsScreenPosInRightArea(Input.mousePosition, true))
                        {
                            m_InputMng.SetType(InputType.Touch_Move);
                            m_IsTouching = true;
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButton(0))
                        {
                            m_InputMng.SetType(InputType.Touch_Move);
                        }


                        if (m_IsTouching && Input.GetMouseButtonUp(0))
                        {
                            m_InputMng.SetType(InputType.Touch_Up);
                            m_IsTouching = false;
                        }

                    }

                    if (m_InputMng.CheckType(InputType.Touch_Move))
                    {

                        this.PreUseSkill(Input.mousePosition);

                    }
                    else if (m_InputMng.CheckType(InputType.Touch_Up))
                    {
                        this.DoUseSkill(Input.mousePosition);
                    }
                    else
                    {
                        Pointing.instance.ResetPointing();
                    }
                }
                break;

            case PlatformType.Android:
            case PlatformType.iOS:
                {
                    m_InputMng.SetType(InputType.NULL);

                    Touch curTouch = new Touch();

                    if (!this.IsScreenPosInRightArea(out curTouch))
                    {
                        return;
                    }

                    if (curTouch.phase == TouchPhase.Began)
                    {
                        if (!this.IsPointerOverUIObject())
                        {
                            m_InputMng.SetType(InputType.Touch_Move);
                            m_IsTouching = true;
                        }
                    }
                    else
                    {
                        if (curTouch.phase == TouchPhase.Stationary ||
                            curTouch.phase == TouchPhase.Moved)
                        {
                            m_InputMng.SetType(InputType.Touch_Move);
                        }

                        if (m_IsTouching && curTouch.phase == TouchPhase.Ended)
                        {
                            m_InputMng.SetType(InputType.Touch_Up);
                            m_IsTouching = false;
                        }
                    }

                    if (m_InputMng.CheckType(InputType.Touch_Move))
                    {
                        this.PreUseSkill(curTouch.position);
                    }
                    else if (m_InputMng.CheckType(InputType.Touch_Up))
                    {
                        this.DoUseSkill(curTouch.position);
                    }
                    else
                    {
                        Pointing.instance.ResetPointing();
                    }
                }
                break;
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private bool IsScreenPosInRightArea(Vector2 pos, bool touchBegan)
    {
        bool result = false;

        // if (touchBegan || m_IsTouching)
        // {
        //     if (pos.x > Screen.width * 0.65f &&
        //         pos.y < Screen.height * 0.35f)
        //     {
        //         result = true;
        //     }
        // }

        return true;
    }

    private bool IsScreenPosInRightArea(out Touch touch)
    {
        touch = new Touch();
        int i = 0;

        while (Input.touchCount > i)
        {
            touch = Input.GetTouch(i);

            if (this.IsScreenPosInRightArea(touch.position,
                touch.phase == TouchPhase.Began))
                return true;

            i++;
        }

        return false;
    }

    private void PreUseSkill(Vector2 screenPos)
    {
        
    }
    private void DoUseSkill(Vector2 screenPos)
    {
        
    }
}
