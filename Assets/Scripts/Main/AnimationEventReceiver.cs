using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour {

	public void HandleEvent(string key)
	{
		// Debug.Log(gameObject.name + " get key : " + key);
		RoleBase role = GetComponentInParent<RoleBase>();
		if (role != null)
		{
			role.HandleEvent(key);
		}
	}
}
