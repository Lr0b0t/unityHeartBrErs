using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BrainData : MonoBehaviour {
	//array of connected motors to ports 	
	public GameObject[] connectedMotors = new GameObject[8];		
	public GameObject[] connectedSensors = new GameObject[8];
	//function is needed to convert connected to the ports motors'es to the string 
	public string ToStringMotors() {													
		StringBuilder builder = new StringBuilder();				
		//consider connected motors one by one 
		foreach(GameObject motor in connectedMotors) {		
			//if motor does exist 
			if (motor != null)															
			{ 
				//add to builder object's index 
				builder.Append(motor.GetComponent<ExportObjectData>().objectIndex);		
			}
			//if it is not a motor 
			else {						
				//add value, which can't be used as object's index
				builder.Append(-1);												}		
			
			builder.Append(",");
		}
		builder.Remove(builder.Length - 1, 1);

		return builder.ToString();
	}
	//function is needed to convert connected to the ports sensors'es to the string 		
	public string ToStringSensors()																								
	{
		StringBuilder builder = new StringBuilder();
		//consider connected sensors one by one 
		foreach (GameObject sensor in connectedSensors)									
		{
			//if it is a sensor 
			if (sensor != null)															
			{
				//add to builder object's index 
				builder.Append(sensor.GetComponent<ExportObjectData>().objectIndex); 	
			} 
			//if it is not a motor 
			else 																		
					
			{
				//add value, which can't be used as object's index
				builder.Append(-1);														
			}
			builder.Append(",");
		}
		builder.Remove(builder.Length - 1, 1);

		return builder.ToString();
	}
}
