using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

public class BD_Patrol : Action
{

    public SharedVector2 areaX;
    public SharedVector2 areaY;
    public bool isAttackPatrol = false;
    public bool isMoveAroundStaticPoint = false;
    private Vector3 m_Target;
    public SharedFloat m_Distance;
    public SharedVector3 m_TargetStaticPoint;
    private NavMeshAgent m_Agent;
    private NavMeshPath path;
    private Vector3[] paths;
    private Vector3 temp = Vector3.zero;
    private Vector3 temp_RandomRun = Vector3.zero;
    private Transform navAgent_Transform;
    private float tempFixBugTime = 0;
    private RoleBase m_Enemy;
    private float m_MoveVec;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private Vector3 tempVector3 = Vector3.zero;


    public override void OnAwake()
    {


    }
    public override void OnStart()
    {
        minX = areaX.Value.x;
        maxX = areaX.Value.y;
        minY = areaY.Value.x;
        maxY = areaY.Value.y;
        m_Enemy = gameObject.GetComponentInChildren<RoleBase>();
        path = new NavMeshPath();
        m_Agent = m_Enemy.m_Agent;
        navAgent_Transform = m_Agent.transform;

        m_MoveVec = m_Enemy.m_MoveVec;
        if (isMoveAroundStaticPoint)
        {
            tempVector3.x = Random.Range(minX + m_TargetStaticPoint.Value.x, maxX + m_TargetStaticPoint.Value.x);
            tempVector3.y = Random.Range(minY + m_TargetStaticPoint.Value.y, maxY + m_TargetStaticPoint.Value.y);
        }
        else if (isAttackPatrol)
        {
            tempVector3.x = Random.Range(minX + transform.position.x, maxX + transform.position.x);
            tempVector3.y = Random.Range(minY + transform.position.y, maxY + transform.position.y);
        }
        else
        {
            tempVector3.x = Random.Range(minX, maxX);
            tempVector3.y = Random.Range(minY, maxY);
        }


        m_Target = tempVector3;

    }
    public override TaskStatus OnUpdate()
    {
        if (m_Target == null)
        {
            return TaskStatus.Failure;
        }
        FixPosBug();
        if (!IsArrive(m_Target, m_Distance.Value))
        {
            MoveByNavPaths(m_Target);
            return TaskStatus.Running;
        }
        else
        {
            if (isMoveAroundStaticPoint)
            {
                tempVector3.x = Random.Range(minX + m_TargetStaticPoint.Value.x, maxX + m_TargetStaticPoint.Value.x);
                tempVector3.y = Random.Range(minY + m_TargetStaticPoint.Value.y, maxY + m_TargetStaticPoint.Value.y);
            }
            else if (isAttackPatrol)
            {
                tempVector3.x = Random.Range(minX + transform.position.x, maxX + transform.position.x);
                tempVector3.y = Random.Range(minY + transform.position.y, maxY + transform.position.y);
            }
            else
            {
                tempVector3.x = Random.Range(minX, maxX);
                tempVector3.y = Random.Range(minY, maxY);
            }


            m_Target = tempVector3;

            MoveByNavPaths(m_Target);
            return TaskStatus.Running;
        }


    }
    public override void OnEnd()
    {

    }
    public bool IsArrive(Vector3 target, float stopdis)
    {

        m_Agent.CalculatePath(target, path);
        paths = path.corners;
        if (Vector3.Distance(transform.position, target) < stopdis)
        {
            return true;
        }
        return false;
    }
    private void MoveByNavPaths(Vector3 target)
    {

        if (m_Enemy.m_CurStatus == RoleStatus.Die)
        {

            return;
        }
        if (m_Enemy.m_CurStatus == RoleStatus.Atked)
        {

            return;
        }
        if (m_Enemy.m_CurStatus == RoleStatus.Attack)
        {

            // return;
        }

        m_MoveVec = m_Enemy.m_MoveVec;
        m_Agent.CalculatePath(target, path);
        paths = path.corners;
        if (paths.Length == 1)
        {

            // Debug.Log("ArriveTargetPos");
        }
        if (paths.Length > 1)
        {
            Debug.DrawLine(paths[0], paths[1]);
            if (paths.Length > 2)
            {
                Debug.DrawLine(paths[1], paths[2]);
                if (Vector3.Distance(paths[0], paths[1]) < 0.07f)//dont not change this var
                {
                    temp = paths[2] - transform.position;
                    temp = temp.normalized;

                }
                else
                {
                    temp = paths[1] - transform.position;
                    temp = temp.normalized;

                }
            }
            else
            {
                temp = paths[1] - transform.position;
                temp = temp.normalized;

            }
        }

        m_Enemy.M_CurVec_Move = temp;
        // Debug.Log(m_Enemy.m_CurVec_AtkTar);
        temp *= m_MoveVec;


        transform.Translate((Vector2)temp * Time.deltaTime);
        navAgent_Transform.position = transform.position;
        if (paths.Length > 3)
        {
            Debug.DrawLine(paths[2], paths[3]);
        }
        if (paths.Length > 4)
        {
            Debug.DrawLine(paths[3], paths[4]);
        }
        if (paths.Length > 5)
        {
            Debug.DrawLine(paths[4], paths[5]);
        }
        if (paths.Length > 6)
        {
            Debug.DrawLine(paths[5], paths[6]);
        }
        if (paths.Length > 7)
        {
            Debug.DrawLine(paths[6], paths[7]);
        }
    }

    // protected override void FixedUpdate()
    // {
    //     FixPosBug();
    // }
    private void FixPosBug()
    {
        if (paths == null || paths.Length < 1) return;
        if (!navAgent_Transform) return;

        if (Vector2.Distance(navAgent_Transform.position, paths[0])
            > 1f)
        {
            if (tempFixBugTime == 0)
            {
                tempFixBugTime = Time.time;
            }
            if (Time.time - tempFixBugTime > 2f)
            {
                Debug.LogWarning("Path:" + paths[0] + " pos:" + transform.position);
                transform.position = paths[0];
                tempFixBugTime = 0f;
                Debug.LogWarning("not the same pos !!!");
            }
        }
        else
        {
            tempFixBugTime = 0;
        }
    }
}
