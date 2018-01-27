using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class BehaviorTest : MonoBehaviour {

	private BehaviorTree m_BehaviorTree;
	void Start () {
		// m_BehaviorTree = Resources.Load<BehaviorTree>("BehaviorTree/EnemyNormalAI");
		ExternalBehaviorTree extBT = Resources.Load<ExternalBehaviorTree>("BehaviorTree/EnemyNormalAI");
		BehaviorTree bt = gameObject.AddComponent<BehaviorTree>();
		bt.ExternalBehavior = extBT;
		bt.StartWhenEnabled = false;

		m_BehaviorTree = bt;
		// m_BehaviorTree = gameObject.GetComponent<BehaviorTree>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			m_BehaviorTree.SendEvent("EnemyHurt");
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			m_BehaviorTree.SendEvent("ArriveTarget");
		}
		
	}
}
