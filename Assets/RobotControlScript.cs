using UnityEngine;
using System.Collections;


public class RobotControlScript : MonoBehaviour {
	
	protected Animator animator;
	
	public Transform target;
	public float DirectionDampTime = .25f;
	
	void Start () 
	{
		animator = GetComponent<Animator>();
	}
	
	void Update () 
	{
		if(animator)
		{
			//get the current state
			//AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			
			float h = 0.0f;
			float v = 0.0f;
			
			Vector2 touchPosition;
			touchPosition.x = 0;
			touchPosition.y = 0;

			Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
			bool update = false;
			
			if (Application.platform == RuntimePlatform.IPhonePlayer ||
			    Application.platform == RuntimePlatform.Android)
			{
				if (Input.touchCount > 0)
				{
					update = true;
					touchPosition = Input.GetTouch(0).position;
				}
			}
			else {
				if (Input.GetMouseButton(0)) {
					update = true;
					touchPosition = Input.mousePosition;
				}
			}
			
			//update = true;
			
			if (update) {
				h = (screenPos.x - touchPosition.x) / Camera.main.pixelWidth;
				v = (screenPos.y - touchPosition.y) / Camera.main.pixelHeight;
				//                              h = Input.GetAxis("Horizontal");
				//                              v = Input.GetAxis("Vertical");
				
				//set event parameters based on user input
				animator.SetFloat("Speed", h*h+v*v);
				animator.SetFloat("Direction", h, DirectionDampTime, Time.deltaTime);

				Debug.Log ("h: " + h + ", v: " + v);
			}
		}               
	}                 
}
