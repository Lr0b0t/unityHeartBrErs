using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class ExportObjectData : MonoBehaviour {
	public int objectIndex;
	public int objectPrefabIndex;

	public GameObject hingedObject;

	public bool canBeElectricallyConnected = false;
	public bool isMotorPort = false;
	public bool isSensorPort = false;
	public int connectedPort = -1;
	public float motorSpeed = 100f;

	public override string ToString() { // ToSrting method is used in order to provide information about our object with string 
		List<NonPhysicalJoint> joints = new List<NonPhysicalJoint>(GetComponents<NonPhysicalJoint>()); // ctreate a list of joints 

		string builder = string.Format("{0}|{1}|{2}|{3}|", objectIndex, objectPrefabIndex, Utils.DeserializeVector2(transform.position), Utils.DeserializeQuaternion(transform.rotation)); // information about object

		foreach(NonPhysicalJoint joint in joints) { //go for each joint one by one 
			builder += joint.connectedObject.GetComponent<ExportObjectData>().objectIndex + "." + (joint.IsHingedJoint? "h":"f") + ","; //add information about joint to the builder string 
		}

		builder = builder.Remove(builder.Length - 1);

		return builder;
	}
}
