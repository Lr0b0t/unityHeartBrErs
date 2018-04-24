using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPhysicalJoint : MonoBehaviour {

	public GameObject connectedObject;
	public bool IsHingedJoint;
	Vector3 prevConnectedObjectPosition;
	Vector3 prevCurrentObjectPosition;
	Vector2 delta;
	public void SetConnectedObject(GameObject connectTo) {								//function is used to add connected object 					
		connectedObject = connectTo;													
		delta = connectTo.transform.position - transform.position;						//current object's and conncted object's positions's difference
	}

	bool isPaused = false;

	public void Pause() {																//Function is used to move current and connected objects together 
		isPaused = true;
	}

	public void Unpause() {																//Function is used to move current and connected objects separately 
		isPaused = false;
		SetConnectedObject(connectedObject);
	}

	void Update() {
		if(isPaused) {																	//if it is allowed to move separately
			return;																		//do not do other following actions
		}

		if(transform.position != prevCurrentObjectPosition) {							//if current object has been moved 
			connectedObject.transform.position = transform.position + (Vector3)delta;	//move connected object too
		}
		else if(connectedObject.transform.position != prevConnectedObjectPosition)		//if connected object has been moved 
		{
			transform.position = connectedObject.transform.position + -(Vector3)delta;	//move current object too
		}

		prevConnectedObjectPosition = connectedObject.transform.position;				//previous position = currebt position 
		prevCurrentObjectPosition = transform.position;
	}
}
