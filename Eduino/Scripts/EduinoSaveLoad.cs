using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EduinoSaveLoad {

	public static void Save(EduinoDataInput[] inputs, string path)
	{
		string json = "";
		for (int i = 0; i < inputs.Length; i++) {
			json += JsonUtility.ToJson (inputs [i]);

			if (i < inputs.Length - 1)
				json += "@";
		}


		File.WriteAllText (path, json);
	}

	public static List<EduinoDataInput> Load(string path)
	{
		string content = File.ReadAllText (path);
		string[] data = content.Split (new char[]{ '@' });
		List<EduinoDataInput> inputs = new List<EduinoDataInput> ();
		for (int i = 0; i < data.Length; i++) {
			inputs.Add (JsonUtility.FromJson<EduinoDataInput> (data [i]));
		}

		return inputs;
	}
}
