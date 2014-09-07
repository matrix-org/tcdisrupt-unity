using UnityEngine;
using System.Collections;
using SimpleJSON;

public class State : ScriptableObject {
	public ArrayList actions;
}

public class StickmanAction {
	public float speed;
	public float direction;
	public float time;
}

public class RobotControlScript : MonoBehaviour {
	
	protected Animator animator;
	
	public Transform target;
	public float DirectionDampTime = .25f;
	public float t;
	public static State state;
	public ArrayList actions;
	public int actionIndex = 0;

	// ze bridge!
	AndroidJavaClass jc;
	AndroidJavaObject jo;
	
	void Start () 
	{
		jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer"); 
		jo = jc.GetStatic<AndroidJavaObject> ("currentActivity"); 

		animator = GetComponent<Animator>();
		t = Time.time;

		if (!state) {
			state = new State ();
			state.name = "state";
			actions = new ArrayList();
			state.actions = actions;
		} else {
			actions = state.actions;
		}
	}

	void Awake ()
	{
		DontDestroyOnLoad(state);
	}

	void applyAction(StickmanAction action) {
		animator.SetFloat ("Speed", action.speed);
		animator.SetFloat ("Direction", action.direction, DirectionDampTime, Time.deltaTime);
	}

	void setState(string json) {
		//JSONNode s = JSON.Parse(json);
	}

	void getState() {
		JSONClass json = new JSONClass();
		string filename = Time.time + ".png";
		Application.CaptureScreenshot(filename);
		json.Add ("thumbnail", new JSONData(filename));
		JSONArray array = new JSONArray ();
		foreach (StickmanAction action in actions) {
			JSONClass a = new JSONClass ();
			a.Add("speed", new JSONData(action.speed));
			a.Add("direction", new JSONData(action.direction));
			a.Add("time", new JSONData(action.time));
			array.Add(a);
		}
		json.Add ("actions", array);

		System.IO.MemoryStream stream = new System.IO.MemoryStream();
		json.SaveToStream (stream);
		System.IO.StreamReader reader = new System.IO.StreamReader (stream, System.Text.Encoding.UTF8);
		jo.Call ("onReceiveUnityJson", reader.ReadToEnd ());
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

			if (actions.Count > 0 && actionIndex < actions.Count) {
				StickmanAction currentAction = (StickmanAction) actions[actionIndex];
				if (Time.time - t >= currentAction.time) {
					StickmanAction action = (StickmanAction) currentAction;
					applyAction(action);
					actionIndex++;
				}
			}

			if (update) {
				actions.RemoveRange(actionIndex, actions.Count - actionIndex);

				h = (screenPos.x - touchPosition.x) / Camera.main.pixelWidth;
				v = (screenPos.y - touchPosition.y) / Camera.main.pixelHeight;
				// h = Input.GetAxis("Horizontal");
				// v = Input.GetAxis("Vertical");

				float speed = -v;
				float direction = h;

				//set event parameters based on user input
				animator.SetFloat("Speed", speed);
				animator.SetFloat("Direction", direction, DirectionDampTime, Time.deltaTime);

				StickmanAction action = new StickmanAction();
				action.speed = speed;
				action.direction = direction;
				action.time = Time.time - t;
				actions.Add (action);
				applyAction(action);

				//Debug.Log ("h: " + h + ", v: " + v);
			}

			if (Time.time - t > 5) {
				actionIndex = 0;
				Application.LoadLevel ("Stickman");
				t = Time.time;
			}
		}               
	}                 
}