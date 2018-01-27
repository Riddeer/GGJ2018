using UnityEngine;
using System.Collections;
using Spine.Unity;
using System.Collections.Generic;


	public class AnimationTest : MonoBehaviour {

		// #region Inspector
		[SpineAnimation]
		public string Run = "Run";
		[SpineAnimation]
		public string Attack = "Attack";
		[SpineAnimation]
		public string Idle = "Idle";
		[SpineEvent]
		public string footstepEvent = "Attack";
		public AudioSource footstepAudioSource;	
		public AudioSource attackAudioSource;	
		public float speed = 5f;
		public float m_MoveVec = 1f;
		public Transform target;
		public Transform[] m_NormalRotate;
		public Transform[] m_LessRotate;
		public float rotateSize = 1;
		// #endregion
		private List<Vector3> normalV3 = new List<Vector3>();
		private List<Vector3> lessV3 = new List<Vector3>();
		private bool isAttack = false;
		private bool m_Flip = false;
		bool isIdle = false;
		bool isRun = false;
		private SkeletonAnimation skeletonAnimation;
		public bool IsAttack
		{
			get {return isAttack;}
			set {isAttack = value;}
		}
		public enum AttackAimType
		{
			NULL = -1,
			Aim = 1,
			NotAim = 2,
			Size
		}
		public AttackAimType type = AttackAimType.Aim;
		void Start () {
			skeletonAnimation = gameObject.transform.GetChild(0).GetComponent<SkeletonAnimation>();		
			SaveDefaultRotation(m_NormalRotate,normalV3);
			SaveDefaultRotation(m_LessRotate,lessV3);
			// skeletonAnimation = GetComponent<SkeletonAnimation>();
			skeletonAnimation.state.Event += HandleEvent;
			// StartCoroutine(GunGrabRoutine());
		}

		void HandleEvent (Spine.TrackEntry trackEntry, Spine.Event e) {
			if (e.Data.Name == "Step") {
				footstepAudioSource.pitch = 0.5f + Random.Range(-0.1f, 0.1f);
				footstepAudioSource.Play();
			}
			if (e.Data.Name == "Attack") {
				// footstepAudioSource.pitch = 0.5f + Random.Range(-0.2f, 0.2f);
				attackAudioSource.Play();
			}
		}
		void Update()
		{		
			AimTarget(m_NormalRotate,1,normalV3);
			AimTarget(m_LessRotate,rotateSize,lessV3);
			RoleMove();
			RoleAttack();
		}
		IEnumerator GunGrabRoutine () 
		{		
			// Play the walk animation on track 0.
			skeletonAnimation.state.SetAnimation(0, Idle, true);
				
			// Repeatedly play the gungrab and gunkeep animation on track 1.
			while (true) 
			{
				yield return new WaitForSeconds(Random.Range(0.5f, 3f));
				// isAttack = true;
				// skeletonAnimation.state.SetAnimation(1, Attack, false);
				// yield return new WaitForSeconds(1);
				// isAttack = false;
				// skeletonAnimation.state.SetAnimation(1, Run, true);

			}

		}
		private void SaveDefaultRotation(Transform[] transList,List<Vector3> v3List)
		{
			for (int i = 0 ; i < transList.Length ; i++)
			{
				v3List.Add(new Vector3(transList[i].rotation.x,transList[i].rotation.y,transList[i].rotation.z));
			}
		}

		private void TurnToDefaultRotation(Transform[] transList,List<Vector3> v3List)
		{
			for (int i = 0 ; i < transList.Length ; i++)
			{
				transList[i].rotation = Quaternion.Euler(Mathf.Lerp(transList[i].rotation.x,v3List[i].x,Time.deltaTime*0.5f),Mathf.Lerp(transList[i].rotation.y,v3List[i].y,Time.deltaTime*0.5f),Mathf.Lerp(transList[i].rotation.z,v3List[i].z,Time.deltaTime*0.5f));
			}
		}
		private void TurnToTargetRotation(Transform[] transList,float size,List<Vector3> v3List)
		{
			for (int i = 0 ; i < transList.Length ; i++)
			{
				Vector2 direction = target.position - transList[i].position;
				float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg * size;
				Quaternion rotation = Quaternion.AngleAxis(angle,Vector3.forward);
				transList[i].rotation = Quaternion.Slerp(transList[i].rotation,rotation,speed * Time.deltaTime);
			}
		}
		private void AimTarget(Transform[] transList,float size,List<Vector3> v3List)
		{
			switch (type)
			{

				case AttackAimType.Aim:
				
					TurnToTargetRotation(transList,size,v3List);
				
				break;

				case AttackAimType.NotAim:	
					if (!isAttack)
					{
						TurnToDefaultRotation(transList,v3List);
					}
					else if(isAttack)
					{
						TurnToTargetRotation(transList,size,v3List);
					}		
				break;
			}
		}
		private void RoleMove()

		{
			
			Vector3 movingVec = new Vector3(
			Input.GetAxis("Horizontal"), 
			Input.GetAxis("Vertical"), 0);
			Move(movingVec);
			if(isIdle == false && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
			{
				isIdle = true;
				isRun = false;
				skeletonAnimation.state.SetAnimation(0, Idle, true);
			}
			 if(isRun == false )
			{
				if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
				{
					isRun = true;
					isIdle = false;
					skeletonAnimation.state.SetAnimation(0, Run, true);
				}
			}
			
		}
		private void Move(Vector3 vec)
    	{
        	if (vec == Vector3.zero) return;
       	 	vec = vec.normalized;
       	 	vec *= m_MoveVec;

        	this.SetFlipX(vec.x < 0);

        	Rigidbody rig = GetComponent<Rigidbody>();
        	if (rig != null)
        	{
            	rig.MovePosition(transform.position + vec * Time.deltaTime);
        	}
        	else
        	{
        	    transform.Translate(vec * Time.deltaTime);
       		 }

       	 	// action
        	// if (m_AniMng != null)
        	// {
           	 	RoleRun();
        	// }	
   		}
		private void SetFlipX(bool flip)
    	{
        	if (m_Flip == flip) return;
        	m_Flip = flip;
        	float flipVal = flip ? -1f : 1f;
        	this.GetRes().SetLocalScale_X(
            Mathf.Abs(this.GetRes().localScale.x) * flipVal);
   		}
		protected Transform GetRes()
   		{
        	return transform.GetChild(0);
    	}
		// private bool RoleRun(string aniName, bool loop, bool overrideSameName = false)
		private bool RoleRun()
		{
			bool result = false;
		// if (m_SkeAnimation && m_SkeAnimation.state.GetCurrent(0) != null)
		// 	result = m_SkeAnimation.state.GetCurrent(0).Animation.Name == aniName;

		// if (overrideSameName)
		// {
		// 	m_SkeAnimation.state.SetAnimation(0, aniName, loop);
		// }
		// else
		// {
		// 	if (!result)
		// 	{
		// 		m_SkeAnimation.state.SetAnimation(0, aniName, loop);
		// 	}
		// }

			return result;
		}
		private void RoleAttack()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				skeletonAnimation.state.SetAnimation(1, Attack, false);
			}
		}
	}
