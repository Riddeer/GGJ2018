using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

public class BehaviorEventSetter : MonoBehaviour {

	BehaviorTree behaviorTree;
	void Start () {
		behaviorTree = GetComponent<BehaviorTree>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			
			behaviorTree.SetVariableValue("Fury",true);
		}
	}
}
