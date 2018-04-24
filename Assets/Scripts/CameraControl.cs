using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	void Start() {

	}

	[SerializeField]
	float movingSensitivity = 0.166f;

	[SerializeField]
	float orthoZoomSpeed;
	[SerializeField]
	float maxOrthoZoom;

	float minimumY = -4.5f;
	public void UpdateCamera() {
		HandleMovement();
		HandleZoom();
	}

	void HandleMovement() {																				//function is used ti camera's following the movement of the finger 
		if (Input.touchCount >= 1)																		//if something is touched 
		{
			Vector3 delta = (Vector3)Input.GetTouch(0).deltaPosition;									//get delta position 
			delta *= -movingSensitivity * 2f * Camera.main.orthographicSize / Camera.main.pixelHeight;	//doing camera's movement depended on screen's size
			if(transform.position.y + delta.y - Camera.main.orthographicSize < minimumY) {				//if camera is lower than the floor
				delta.y = 0f;																			//set delta y to zero
			}
			transform.position += delta;																//add to current posotion of the object delta, so that camer will move
		}
	}


	void HandleZoom() {																					//function is used for zooming 
		if (Input.touchCount >= 2)
		{
			
			Touch touchZero = Input.GetTouch(0);														//Store first touch													
			Touch touchOne = Input.GetTouch(1);															//Store second touch


			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;					// Find the position in the previous frame of the first touch.
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;						// Find the position in the previous frame of the second touch.


			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;					// Find the magnitude of the vector (the distance) between the touches in each frame
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;


			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;								// Find the difference in the distances between each frame

			float prevOrthoSize = Camera.main.orthographicSize;											

			Camera.main.orthographicSize += deltaMagnitudeDiff * 2f * Camera.main.orthographicSize / Camera.main.pixelHeight; //change the orthographic size based on the change in distance between the touches.

			if(Camera.main.transform.position.y - Camera.main.orthographicSize < minimumY) {			//Check if the camera goes over the lower OY bound 
				Vector3 pos = Camera.main.transform.position;
				pos.y += minimumY - (Camera.main.transform.position.y - Camera.main.orthographicSize);	//Change Y position 
				Camera.main.transform.position = pos;
			}

			Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, 0.1f);				// Make sure the orthographic size never drops below zero

			Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, maxOrthoZoom);		// Make sure the orthographic size is always below maximum value
		}
	}
}
