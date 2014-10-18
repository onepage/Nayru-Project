using UnityEngine;
using System.Collections;


public class CameraController : MonoBehaviour
{
    #region public variable

    public Transform target;
	private Transform target2; //temporary change for this game
    public Vector3 targetOffset;
    public bool incrementalCameraAngles = true;
    public bool stopOrbitAtTop = false;
    public bool multipleTargets = false;
	public float distance = 5.0f;
	public float maxDistance = 20;
	public float minDistance = 2f;
	public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -75;
    public int yMaxLimit = 75;
    public int yMinOrbitLimit = -723;
    public int yMaxOrbitLimit = 877;
	public int zoomRate = 40;
	public float panSpeed = 0.3f;
	public float zoomDampening = 5.0f;
    public float transitionDuration = 2.5f;

    public Transform groupTargetTrans;

    #endregion

    #region private variables

    private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	private float currentDistance;
	private float desiredDistance;
	private Quaternion currentRotation;
	private Quaternion desiredRotation;
	private Quaternion rotation;
	private Vector3 position;
	
	private Vector3 newPos;             // The position the camera is trying to reach.
	private int cameraViewIndex = 0;
	private int startCameraCheck = 0;
	private Vector3[] checkPoints = new Vector3[10]; // An array of 5 points to check if the camera can see the target.
	private bool changeTarget = false;

	private bool groupTarget = false;

    private GamePadController.Controller _gamePad;

    #endregion

    void Start() {
        _gamePad = GamePadController.GamePadOne;
        Init();
        if (incrementalCameraAngles)
        {
            Vector3 angles = transform.eulerAngles;
            xDeg = angles.y;
            yDeg = angles.x;
        }
    }
	//void OnEnable() { Init(); }
	
	public void Init()
	{
		//If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
		if (!target)
		{
			GameObject go = new GameObject("Cam Target");
			go.transform.position = transform.position + (transform.forward * distance);
			target = go.transform;
		}
		if(target.childCount > 0 && multipleTargets) {
			GameObject targetCenter = new GameObject("TargetCenter");
			targetCenter.transform.parent = target;
			targetCenter.transform.localPosition = Vector3.zero;
			groupTargetTrans = targetCenter.transform;
			GameObject targetFollow = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			targetFollow.transform.position = targetFollow.transform.position;
			target = targetFollow.transform;
			groupTarget = true;
		} else {
			groupTarget = false;
		}
		transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z+maxDistance/2);
		transform.GetChild(0).localPosition = Vector3.zero;
		distance = Vector3.Distance(transform.position, target.position);
		currentDistance = distance;
		desiredDistance = distance;
		
		//be sure to grab the current rotations as starting points.
		position = transform.position;
		rotation = transform.rotation;
		currentRotation = transform.rotation;
		desiredRotation = transform.rotation;
		
		xDeg = Vector3.Angle(Vector3.right, transform.right );
		yDeg = Vector3.Angle(Vector3.up, transform.up );		
		
		// The standard position of the camera is the relative position of the camera from the target.
		Vector3 standardPos = new Vector3(0,0,0);		
		// The abovePos is directly above the target at the same distance as the standard position.
		Vector3 abovePos = new Vector3(90,0,0);			
		// The first is the standard position of the camera.
		checkPoints[0] = standardPos;		
		// The next three are 25%, 50% and 75% of the distance between the standard position and abovePos.
		checkPoints[1] = Vector3.Lerp(standardPos, abovePos, 0.25f);
		checkPoints[2] = Vector3.Lerp(standardPos, abovePos, 0.5f);
		checkPoints[3] = Vector3.Lerp(standardPos, abovePos, 0.75f);		
		// The last is the abovePos.
		checkPoints[4] = abovePos;
        //Now we add 5 other checkpoints for the opposite direction (in front of the target)
		Vector3 newAbovePos = new Vector3(90,180,0);
		Vector3 newStandPos = new Vector3(0,180,0);
		checkPoints[5] = newAbovePos;
		checkPoints[6] = Vector3.Lerp(newAbovePos, newStandPos, 0.25f);
		checkPoints[7] = Vector3.Lerp(newAbovePos, newStandPos, 0.5f);
		checkPoints[8] = Vector3.Lerp(newAbovePos, newStandPos, 0.75f);
		checkPoints[9] = newStandPos;
		
	}
	
	/*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
	void LateUpdate()
	{
		//transform.GetChild(0).localPosition = Vector3.zero;
		//othographic camera
		/*float margin = 15f;
		if(target.childCount > 0)
			transform.GetComponent<Camera>().orthographicSize = target.GetChild(0).transform.renderer.bounds.extents.y+margin;
		*/		
		if(groupTarget) {
			target.position = groupTargetTrans.position;
		}
		if(Input.GetKeyUp(KeyCode.LeftAlt)) {
			changeTarget = true;
		}
		if(changeTarget)
			StartCoroutine(Transition());
		else 
        {
            if (incrementalCameraAngles)
            {
                ChangeCameraVIew();
                ChangeCameraRotation();
                UpdateCameraView();

                // affect the desired Zoom distance if we roll the scrollwheel
                float zoomAmount = 0;
                if (_gamePad.R3.Pressed || _gamePad.R3.Held)
                    zoomAmount -= 0.01f;
                if (_gamePad.L3.Pressed || _gamePad.L3.Held)
                    zoomAmount += 0.01f;
                desiredDistance -= zoomAmount * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
                //clamp the zoom min/max
                desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
                // For smoothing of the zoom, lerp distance
                currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
                //orthographic camera
                //transform.GetComponent<Camera>().orthographicSize = desiredDistance;

                // calculate position based on the new currentDistance 
                position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
                transform.position = position;
            } 
            else
                OrbitCamera();
		}
	}

    void OrbitCamera()
    {
        xDeg -= _gamePad.RightStick.X * xSpeed * 0.02f;
        yDeg += _gamePad.RightStick.Y * ySpeed * 0.02f;

        if (!stopOrbitAtTop)
            yDeg = ClampAngle(yDeg, yMinOrbitLimit, yMaxOrbitLimit);
        else
        {
            float addedVAl = 15f;
            if (yDeg <= yMinLimit && _gamePad.RightStick.Y < 0)
            {
                yDeg = ClampAngle(yDeg, yMinLimit - addedVAl, yMaxLimit);
            }
            else if (yDeg >= yMaxLimit && _gamePad.RightStick.Y > 0)
            {
                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit + addedVAl);
            }
            else if (yDeg <= yMinLimit || yDeg >= yMaxLimit && _gamePad.RightStick.Y == 0)
            {
                if (yDeg <= yMinLimit)
                {
                    yDeg = Mathf.Lerp(yDeg, yMinLimit, 0.5f * Time.deltaTime);
                }
                if (yDeg >= yMaxLimit)
                {
                    yDeg = Mathf.Lerp(yDeg, yMaxLimit, 0.5f * Time.deltaTime);
                }
            }
            else if (yDeg >= yMinLimit && yDeg <= yMaxLimit && _gamePad.RightStick.Y != 0)
            {
                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

            }
        }

        float zoomAmount = 0;
        if (_gamePad.R3.Pressed || _gamePad.R3.Held)
            zoomAmount -= 0.01f;
        if (_gamePad.L3.Pressed || _gamePad.L3.Held)
            zoomAmount += 0.01f;

        distance -= zoomAmount * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);

        Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0.0f);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
        
        transform.rotation = rotation; 
        transform.position = position;
    }
	
	void ChangeCameraVIew(){

        if (_gamePad.UP.Pressed || _gamePad.DOWN.Pressed)
        {
            if (_gamePad.UP.Pressed && cameraViewIndex < checkPoints.Length - 1)
            {
				cameraViewIndex++;
            }
            else if (_gamePad.DOWN.Pressed && cameraViewIndex > 0)
            {
				cameraViewIndex--;
			}
		}
		if(ViewingPosCheck(checkPoints[cameraViewIndex]))
			startCameraCheck = cameraViewIndex;
		else
			startCameraCheck = 0;
	}
	
	void ChangeCameraRotation(){
        if (_gamePad.RightStick.X != 0)
		{
            if (_gamePad.RightStick.X > 0)
				yDeg -= 1f;
			else
				yDeg +=1f;
			
			if(cameraViewIndex > 4) {
				cameraViewIndex = (checkPoints.Length-1)-cameraViewIndex;
				yDeg +=180;
			}
			// The standard position of the camera is the relative position of the camera from the target.
			Vector3 standardPos = new Vector3(0,0+yDeg,0);		
			// The abovePos is directly above the target at the same distance as the standard position.
			Vector3 abovePos = new Vector3(90,0+yDeg,0);			
			// The first is the standard position of the camera.
			checkPoints[0] = standardPos;		
			// The next three are 25%, 50% and 75% of the distance between the standard position and abovePos.
			checkPoints[1] = Vector3.Lerp(standardPos, abovePos, 0.25f);
			checkPoints[2] = Vector3.Lerp(standardPos, abovePos, 0.5f);
			checkPoints[3] = Vector3.Lerp(standardPos, abovePos, 0.75f);		
			// The last is the abovePos.
			checkPoints[4] = abovePos;
			Vector3 newAbovePos = new Vector3(90,180+yDeg,0);
			Vector3 newStandPos = new Vector3(0,180+yDeg,0);
			checkPoints[5] = newAbovePos;
			checkPoints[6] = Vector3.Lerp(newAbovePos, newStandPos, 0.25f);
			checkPoints[7] = Vector3.Lerp(newAbovePos, newStandPos, 0.5f);
			checkPoints[8] = Vector3.Lerp(newAbovePos, newStandPos, 0.75f);
			checkPoints[9] = newStandPos;
		}
	}
	
	void UpdateCameraView(){		
		// Run through the check points...
		for(int i = startCameraCheck; i < checkPoints.Length; i++)
		{
			// ... if the camera can see the target...
			if(ViewingPosCheck(checkPoints[i])) {
				// ... break from the loop.				
				cameraViewIndex = i;
				break;
			}
		}
		
		yDeg = checkPoints[cameraViewIndex].y;
		xDeg = checkPoints[cameraViewIndex].x;	
		////////OrbitAngle			
		//Clamp the vertical axis for the orbit
		xDeg = ClampAngle(xDeg, yMinLimit, yMaxLimit);
		// set camera rotation 
		desiredRotation = Quaternion.Euler(xDeg, yDeg, 0);
		currentRotation = transform.rotation;
		
		rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
		transform.rotation = rotation;
	}
	
	bool ViewingPosCheck (Vector3 checkPos)
	{
		RaycastHit hit;
		Quaternion childCurrentRot = transform.GetChild(0).rotation;
		float childXDeg  = checkPos.x;
		float childYDeg  = checkPos.y;			
		childXDeg = ClampAngle(childXDeg, yMinLimit, yMaxLimit);
		Quaternion childDesiredRot = Quaternion.Euler(childXDeg, childYDeg, 0);
		Quaternion rotationChild = Quaternion.Lerp(childCurrentRot, childDesiredRot,1f);
		transform.GetChild(0).rotation = rotationChild;

		// calculate position of the child based on the new currentDistance 
		Vector3 checkChildPos = target.position - (rotationChild * Vector3.forward * currentDistance + targetOffset);
		transform.GetChild(0).position = checkChildPos;
		// If a raycast from the check position to the target hits something...
		if(Physics.Raycast(checkChildPos, target.position - checkChildPos, out hit)) {
			
			// ... if it is not the target...
			/*if(!groupTarget) {
				if(hit.transform != target)
					// This position isn't appropriate.
					return false;
			} else {
				for(int k = 0; k < groupTargetTrans.parent.childCount; k++) {
					if(hit.transform != target || hit.transform != groupTargetTrans.parent.GetChild(k))
						Debug.Log("FOUND HIT "+k);
					// This position isn't appropriate.
					return false;
				}
			}*/
			// ... if it is not the target...
			if(hit.transform != target) {
				// This position isn't appropriate.
				Debug.Log(hit.transform.name);
				return false;
			}
		}		
		
		Debug.DrawRay(checkChildPos, target.position - checkChildPos, Color.green);
		Debug.DrawLine(transform.position, transform.GetChild(0).position, Color.green);
		Debug.DrawLine(transform.GetChild(0).position, target.position, Color.red);
		Debug.DrawLine(transform.position, target.position, Color.blue);
		// If we haven't hit anything or we've hit the target, this is an appropriate position.
		newPos = checkPos;
		return true;
	}

	IEnumerator Transition()
	{
		float t = 0.0f;
		Vector3 startingPos = transform.position;
		while (t < 1.0f)
		{
			t += Time.deltaTime * (Time.timeScale/transitionDuration);
			
			transform.position = Vector3.Lerp(startingPos, target2.position, t);
			yield return 0;
		}
		if(t > 0.99f && changeTarget) {
			target = target2;
			Init();
		}
		yield return null;
		changeTarget = false;
	}
	
	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
}