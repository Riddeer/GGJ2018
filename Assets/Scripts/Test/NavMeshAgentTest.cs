using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentTest : MonoBehaviour {

	private NavMeshAgent agent;
	void Start () {
		agent = GetComponent<NavMeshAgent>();
	}
	
	
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			Target();
		}
	}
	private void Target()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray,out hitInfo))
		{
			Vector3 target = hitInfo.point;
			agent.destination = target;
		}
	}
}
