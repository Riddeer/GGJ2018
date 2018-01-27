using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierTest : MonoBehaviour
{
    // public Transform prefabExplosion;
    private Transform battleground;
    private Vector3 m_StartPos;
    public float m_MoveVec;
    private float m_Time;
    public float width = 5f;
    public Transform m_Target;
    
    // private Particle m_pa;

    void Start()
    {   
        battleground = EnemyManager.instance.transform;
        m_StartPos = transform.position;
        m_Time = 0;
      

       
    }
    void Update()
    {
            m_Time += Time.deltaTime;
            // BezierCurve(m_BezierCtrlHight);  
            ComeBackBezier(m_Target.position);
          
    }
    private Vector3[] CalcBezierControlPoint(Vector3 tarPos)
    {
        float dis = Vector3.Distance(m_StartPos,tarPos);
       
        float standardControlPointHigh = dis/2f/0.375f;
        // Debug.Log(standardControlPointHigh)
        Vector3[] controlPoints = new [] {new Vector3(-width,standardControlPointHigh,0f)
        ,new Vector3 (width,standardControlPointHigh,0f)};
        // Vector3 tempPos = new Vector3(m_StartPos.x,m_StartPos.y,0);
        // for ( int i = 0; i <controlPoints.Length; i++)
        // {
        //     controlPoints[i] = controlPoints[i] + tempPos;
        // }

        Vector3 dir = tarPos - m_StartPos;
        dir = dir.normalized;
        float angle = Vector3.Angle(Vector3.up,dir);
        if (tarPos.x - m_StartPos.x < 0f)
            angle = 360f - angle;
        float deg = (angle * Mathf.PI)/180f;
        Debug.Log(angle);
        Vector3 temp = Vector3.zero;
        
        for (int i = 0; i < controlPoints.Length; i++ )
        {
           
          
         
            
            temp.y = (controlPoints[i].y - Vector3.zero.y) * Mathf.Cos(deg) - (controlPoints[i].x - Vector3.zero.x) * Mathf.Sin(deg) + Vector3.zero.y;
            temp.x = (controlPoints[i].y - Vector3.zero.y) * Mathf.Sin(deg) + (controlPoints[i].x - Vector3.zero.x) * Mathf.Cos(deg) + Vector3.zero.x;
            
            controlPoints[i] = temp;
          
          
            
            
        }
        return controlPoints;
    }
    private void ComeBackBezier(Vector3 target)
    {
        // bezier
        
        Bezier myBezier = new Bezier(m_StartPos,
            CalcBezierControlPoint(target)[0],
            CalcBezierControlPoint(target)[1], m_StartPos);
           CalcBezierControlPoint(target);
        //  Bezier myBezier = new Bezier(m_StartPos,
        //     (target-m_StartPos)*1.88f,
        //     (target-m_StartPos)*1.88f, m_StartPos);
        //     Debug.Log(target-m_StartPos);
        float allTime = Vector3.Distance(m_StartPos, target) / m_MoveVec;
        float temp = Mathf.Clamp(m_Time / allTime, 0f, 1f);
        Vector3 curPos = myBezier.GetPointAtTime(temp);
        Debug.DrawLine(transform.position,curPos,Color.red,30f);
        transform.position = curPos ;
    }
}
