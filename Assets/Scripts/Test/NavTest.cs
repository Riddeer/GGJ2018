using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class NavTest : MonoBehaviour
{

    NavMeshPath path;
    private NavMeshAgent m_Agent;
    private Vector3[] paths;
    private Vector3 temp = Vector3.zero;
	// private Vector3 target = new Vector3(70,0,0);
    void Start()
    {
        path = new NavMeshPath();
        m_Agent = gameObject.GetComponent<NavMeshAgent>();
    }
    void Update()
    {
       
        // MoveByNavPaths(target,0.5f,path);
    }

    private void MoveByNavPaths(Vector3 target,float speed ,NavMeshPath path)
    {
		m_Agent.CalculatePath(target,path);
		paths = path.corners;
		if(paths.Length == 1)
		{
			Debug.Log("ArriveTargetPos");
		}
        if (paths.Length > 1)
        {
            Debug.DrawLine(paths[0], paths[1]);
            if (paths.Length > 2)
            {
                Debug.DrawLine(paths[1], paths[2]);
                if (Vector3.Distance(paths[0], paths[1]) < 0.05f)
                {
                    temp = paths[2] - transform.position;
                    temp = temp.normalized;
                    transform.position += temp * speed;
                }
                else
                {
                    temp = paths[1] - transform.position;
                    temp = temp.normalized;
                    transform.position += temp * speed;
                }
            }
            else
            {
                temp = paths[1] - transform.position;
                temp = temp.normalized;
                transform.position += temp * speed;
            }
        }
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

}
