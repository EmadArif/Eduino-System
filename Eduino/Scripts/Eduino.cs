using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eduino : EduinoBase {

	public static Eduino Instance{ get; private set;}
	public List<EduinoDataInput> inputs;
	private string lastMessage = "";
	private string readedMessaged;

	void Awake()
	{
		if (Instance != null && Instance != this)
			Destroy (gameObject);

		Instance = this;
	}

	protected override void OnEnable()
	{
		base.OnEnable ();

		serialPort.onUpdate += CalculateInputs;
	}

	public string ReadMessage()
	{
		string msg = readedMessaged;
		if (msg != null)
			lastMessage = msg;

		return lastMessage;
	}

	public void WriteMessage(string message)
	{
		serialPort.WriteMessage (message);
	}

	void CalculateInputs()
	{
		string data = (string)serialPort.ReadMessage ();
		readedMessaged = data;
		if (data != null) {
			string readedLine = data.ToString().Remove(0,1);
			string[] values = readedLine.Split(',');//BL, BD, BU, BR, BO, JY, JX,GX, GY, GZ
			int min = Mathf.Min(values.Length, inputs.Count);

			for (int i = 0; i < min; i++) {
				inputs[i].SetValue(values[i].ToString());
			}
		}
	}

	public EduinoDataInput FindInputByName(string inputName)
	{
		EduinoDataInput input = inputs.Find (t => t.name.Equals (inputName));
		if (input == null) {
			UnityEngine.Debug.LogError ("Input Name: " + inputName + "is not exists");
			return null;
		}
		return input;
	}

	public T GetValue<T>(string inputName)
	{
		EduinoDataInput input = inputs.Find (t => t.name.Equals (inputName));

		if (input == null) {
			return default(T);
		}

		return input.GetValue<T> ();
	}

	public bool GetTrigger(string inputName)
	{
		EduinoDataInput input = inputs.Find (t => t.name.Equals (inputName));;
		inputs.Find (t => t.name.Equals (inputName));


		if (input == null) {
			return false;
		}

		return input.GetTrigger();
	}
}
