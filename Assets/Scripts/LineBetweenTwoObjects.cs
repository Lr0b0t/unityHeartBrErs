using UnityEngine;
using UnityEngine.UI;
public class LineBetweenTwoObjects : MonoBehaviour {
	LineRenderer attachedRenderer;
	GameObject canvasChild;
	public Material materialToSet;

	public NonPhysicalJoint attachedJoint;

	public int connectedPort = -1;
	public bool isMotorConnected;

	public GameObject object1, object2;
	void Start() {
		attachedRenderer = gameObject.AddComponent<LineRenderer>();
		canvasChild = transform.GetChild(0).gameObject;
		attachedRenderer.startWidth = 0.03f;
		attachedRenderer.endWidth = 0.03f;
		attachedRenderer.material = materialToSet;
		attachedRenderer.sortingOrder = -1;
		canvasChild.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = materialToSet.color;
		attachedRenderer.SetPositions(new Vector3[] { object1.transform.position, object2.transform.position });
	}

	public void DestroyJoint() {																			//function is used, when joint is destoyed
		if(connectedPort != -1) {																			//if object is connected to some of the ports 
			if(isMotorConnected) {																			//if motor is connected 
				GameObject.Find("Brain").GetComponent<BrainData>().connectedMotors[connectedPort] = null;	//destroy connected motor in array of brain's ports
			}
			else 																							//if sensor is connected 
			{
				GameObject.Find("Brain").GetComponent<BrainData>().connectedSensors[connectedPort] = null; 	//destroy connected sensor in array of brain's ports
			}
			object2.GetComponent<ExportObjectData>().connectedPort = -1;									//object is not connected to something  
		}
		if (attachedJoint != null)																			//if there is attached joint 
		{
			Destroy(attachedJoint);																			//destroy this joint 
		}
		Destroy(gameObject);																				//destoroy gameobject
	}

	void Update() {
		if(object1 == null || object2 == null) {															//if two objects are existing
			Destroy(gameObject);
		}
		else
		{
			attachedRenderer.SetPosition(0, object1.transform.position);									//set line's start as first object's position
			attachedRenderer.SetPosition(1, object2.transform.position);									//set line's end as second object's position
			canvasChild.transform.position = (object1.transform.position + object2.transform.position) / 2f;
		}
	}
}