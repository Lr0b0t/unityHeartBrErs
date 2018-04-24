using System.Collections;
using System;
using System.Text;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Linq;
using MoonSharp.Interpreter;

public class CorePlay : MonoBehaviour {

	public static string RobotName;
	public static string robotData;
	public static string brainData;
	public static string codeData;
	[SerializeField]
	GameObject[] objects;

	GameObject[] motors = new GameObject[8];
	GameObject[] sensors = new GameObject[8];

	public List<GameObject> instantiatedObjectList = new List<GameObject>();
	public List<int> instantiatedObjectPrefabIndex = new List<int>();

	public void Serialize(string data) {												//function is used to serialise data 
		foreach(string line in data.Split('#')) {										//devide data into lines 
			string[] objectParameters = line.Split('|');								//devide line into object's parameters 

			int objectIndex = int.Parse(objectParameters[0]);							
			int objectPrefabIndex = int.Parse(objectParameters[1]);
			Vector2 position = Utils.SerializeVector2(objectParameters[2]);				
			Quaternion rotation = Utils.SerializeQuaternion(objectParameters[3]);

			GameObject go = Instantiate(objects[objectPrefabIndex], position, rotation); //create a new objects with parameters
			instantiatedObjectList.Add(go);												//add to the list of instantiated Objects
			instantiatedObjectList[objectIndex].AddComponent<Rigidbody2D>();			//add rigidBody 
			instantiatedObjectPrefabIndex.Add(objectPrefabIndex);						//add object's prefab's index 
			//string[] jointIndexes = objectParameters[4].Split(',');
		}

		//is used for objects with joints
		foreach (string line in data.Split('#'))										//devide data into lines 
		{
			string[] objectParameters = line.Split('|');								//devide line into object's parameters 

			int objectIndex = int.Parse(objectParameters[0]);												
			int objectPrefabIndex = int.Parse(objectParameters[1]);
			Vector2 position = Utils.SerializeVector2(objectParameters[2]);
			Quaternion rotation = Utils.SerializeQuaternion(objectParameters[3]);

			try 																		//is used to check if there are some joints
			{
				string[] jointIndexes = objectParameters[4].Split(',');					//get array of joint's parameters 	

				foreach (string jointData in jointIndexes)								//check all joints
				{
					int indexInt = int.Parse(jointData.Split('.')[0]);					
					bool isHinged = jointData.Split('.')[1] == "h";						//is linked, as hinge or not 

					if(isHinged) {														//if object sis connected as hinge
						instantiatedObjectList[objectIndex].AddComponent<HingeJoint2D>().connectedBody = instantiatedObjectList[indexInt].GetComponent<Rigidbody2D>();
						instantiatedObjectList[objectIndex].GetComponent<HingeJoint2D>().useMotor = true;
					}
					else
					{
						instantiatedObjectList[objectIndex].AddComponent<FixedJoint2D>().connectedBody = instantiatedObjectList[indexInt].GetComponent<Rigidbody2D>();
					}
				}
			}
			catch {																		//if there is no joint
				continue;
			}
		}
	}

	void Print(string to) {																//function "Print" for programming with get data to be printed
		Debug.Log(to);																	//Show sting in debugging window 
	}

	void SetMotor(int pin, float speed)													//function "SetMotor" for programming with get data of the connected pin and needed string
	{
		try
		{
			if (motors[pin]) 															//if there is some connected motor to the written port 
			{
				speed = Mathf.Clamp(speed, -1f, 1f);									//smooth increase in speed

				JointMotor2D motor = motors[pin].GetComponent<HingeJoint2D>().motor;	//get motor's hinge joint 
				motor.motorSpeed = speed * motors[pin].GetComponent<ExportObjectData>().motorSpeed; //change speed of the motor 
				motors[pin].GetComponent<HingeJoint2D>().motor = motor;					//get hinge joint 
			}
		}
		catch {
			return;
		}
	}

	void SetServo(int pin, float angle) {

	}

	[SerializeField]
	VirtualJoystick leftJoystick, rightJoystick;									//Joystick's data

	float GetSensorValue(int pin)													//function "GetSensorValue" for programming with get data - pin and returned value 
	{
		try {																		//if there is some connected sensor to the written port 
			if(sensors[pin]) {														
				return sensors[pin].GetComponent<Sensor>().GetSensorValue();		//return sensor's data
			}
			return -1;		
		}
		catch {
			return -1;
		}
	}

	float GetLeftJoystickX() {														//float that returns data - left joystik's X value										
		return -leftJoystick.InputVector.x;
	}
	float GetLeftJoystickY()														//float that returns data - left joystik's Z value		
	{
		return -leftJoystick.InputVector.z;
	}
	float GetRightJoystickX()
	{
		return -rightJoystick.InputVector.x;										//floar that returns data - right joystik's X value		
	}
	float GetRightJoystickY()
	{
		return -rightJoystick.InputVector.z;										//float that returns data - right joystik's z value	
	}

	Script brainScript;													
	void Start() {
		robotData = PlayerPrefs.GetString("robotData" + RobotName);					//Get robotData from previous scene
		codeData = PlayerPrefs.GetString("codeData" + RobotName);					//Get sode Data from previous scene
		brainData = PlayerPrefs.GetString("brainData" + RobotName);					//Get brain Data from previous scene


		/*codeData = @"
		function start()
		end

		function loop()
			SetMotor(0, GetLeftJoystickX());
			SetMotor(1, GetLeftJoystickX());
		end";*/

		Debug.Log(robotData);														//print robot's data 
		Serialize(robotData);														//serialize robot's data 

		int i = 0;
		foreach(string objIndexStr in brainData.Split(' ')[0].Split(',')) {			//Consider motors one by one 
			int ind = int.Parse(objIndexStr);										//Convert pin's number to integer 
			if (ind != -1)															//if is not connected 
			{
				motors[i] = instantiatedObjectList[ind];							//add current motor's object to motors list 
			}
			i++;																	//add 1 to i for loop
		}

		i = 0;
		foreach (string objIndexStr in brainData.Split(' ')[1].Split(','))			//Consider sensors one by one
		{
			int ind = int.Parse(objIndexStr);										//Сonvert pin's number to integer 
			if (ind != -1)
			{
				sensors[i] = instantiatedObjectList[ind];							//add current sensor's object to sensors list 
			}
			i++;																	//add 1 to i for loop
		}

		brainScript = new Script();													//create new script 
		brainScript.DoString(codeData);												//written code is added to the script 

		brainScript.Globals["SetMotor"] = (Action<int, float>)SetMotor;				//create global action, that could be called to set motor's speed  
		brainScript.Globals["GetSensorValue"] = (Func<int, float>)GetSensorValue;	//create globalfunction, that returns sensor's value 
		brainScript.Globals["Print"] = (Action<string>)Print;						//create global action for printing the value
		brainScript.Globals["GetLeftJoystickX"] = (Func<float>)GetLeftJoystickX;	//create global function, that returns left joystic's OX position
		brainScript.Globals["GetLeftJoystickY"] = (Func<float>)GetLeftJoystickY;	//create global function, that returns left joystic's OY position
		brainScript.Globals["GetRightJoystickX"] = (Func<float>)GetRightJoystickX;	//create global function, that returns right joystic's OX position
		brainScript.Globals["GetRightJoystickY"] = (Func<float>)GetRightJoystickY;	//create global function, that returns right joystic's OY position

		brainScript.Call(brainScript.Globals["start"]);								//call "start" function
	}

	void Update()
	{
		brainScript.Call(brainScript.Globals["loop"]);								//call "loop" function by every update 
	}


	private bool IsPointerOverUIObject()											//UI object was touched 
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);			//detect current touch 
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);	//detect current touch's position 
		List<RaycastResult> results = new List<RaycastResult>();										//create a new list of raycast results
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;																		//return if result's amount is greater than 0
	}

	public void Back() {																				//function is used to call previous scene
		SceneManager.LoadScene(0);																		//call scene of robot's creation
	}
}
