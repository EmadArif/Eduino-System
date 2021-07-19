using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EduinoBaseEditor : Editor {

	protected List<SerializedProperty> generalProps = new List<SerializedProperty>();
	protected bool portsFound = false;
	protected bool connectedSucceeded = false;
	protected bool advanceSettings = false;
	private Texture centerImg, leftImag, rightImg, titleImg;
	private Eduino eduino;

	[MenuItem("Eduino/Create Instance")]
	private static void CreateEduinoInstance()
	{
		GameObject obj = new GameObject ("Eduino System");
		obj.AddComponent<Eduino> ();
	}

	protected virtual void OnEnable()
	{
		eduino = target as Eduino;
		generalProps.Add (serializedObject.FindProperty ("portName"));
		generalProps.Add (serializedObject.FindProperty ("buadRate"));
		generalProps.Add (serializedObject.FindProperty ("delay"));
		generalProps.Add (serializedObject.FindProperty ("readTimeout"));
		generalProps.Add (serializedObject.FindProperty ("writeTimeout"));

		centerImg = Resources.Load<Texture> ("center");
		leftImag = Resources.Load<Texture> ("left");
		rightImg = Resources.Load<Texture> ("right2");
		titleImg = Resources.Load<Texture> ("title");
	}

	protected void DrawBanner()
	{
		GUILayout.Space (10);
		Rect r =  EditorGUILayout.GetControlRect ();

		float textureHeight = centerImg.height / 2;

		float rightWidth = rightImg.width / 2;
		float leftWidth = leftImag.width / 2;
		float textWidth = titleImg.width / 2;

		GUI.DrawTexture (new Rect(r.position, new Vector2(leftWidth, textureHeight)), leftImag);
		GUI.DrawTexture (new Rect(r.position + Vector2.right * (r.width - (rightWidth)), new Vector2(rightWidth, textureHeight)), rightImg);
		GUI.DrawTexture (new Rect(r.position + Vector2.right * (leftWidth), new Vector2(r.width - (rightWidth + leftWidth) , textureHeight)), centerImg);
		GUI.DrawTexture (new Rect(r.position + Vector2.right * ((r.width / 2) - rightWidth), new Vector2(textWidth,textureHeight)), titleImg);

		GUILayout.Space (textureHeight - 15);
	}
		

	protected virtual void DrawDefaultProperties()
	{

		EditorGUILayout.BeginVertical (GUI.skin.box);
		EditorGUILayout.LabelField ("General Settings");

		EditorGUILayout.Separator ();

		for (int i = 0; i < generalProps.Count; i++) {
			EditorGUILayout.PropertyField (generalProps[i], true);
			EditorGUILayout.Separator ();
		}

		EditorGUILayout.EndVertical ();
	}

	protected void DrawAdvancedProperties(EduinoBase b)
	{
		EditorGUILayout.BeginVertical (GUI.skin.box);

		EditorGUI.indentLevel++;
		advanceSettings = EditorGUILayout.Foldout (advanceSettings, "Advanced Settings");


		bool correctPort = false;
		connectedSucceeded = false;

		string[] ports = EduinoSerialPort.GetAvailablePorts ();

		for (int i = 0; i < ports.Length; i++) {
			if (eduino.portName == ports [i]) {
				correctPort = true;
				connectedSucceeded = true;
				break;
			}
		}

		if (advanceSettings) {

			if (!correctPort) {
			
				if (GUILayout.Button ("Test Connection")) {

					for (int i = 0; i < ports.Length; i++) {

						connectedSucceeded = EduinoThread.TestConnection (ports [i], eduino.buadRate);


						if (connectedSucceeded) {

							eduino.portName = ports [i];

							break;
						}
					}
				}
			}

			if (connectedSucceeded) {
				GUI.backgroundColor = Color.green;
				EditorGUILayout.HelpBox ("Connection Succeeded", MessageType.Info);
				GUI.backgroundColor = Color.white;

			} else {
				EditorGUILayout.HelpBox ("Connection Fails", MessageType.Error);
			}

			/*
			EditorGUILayout.Separator ();

			if (GUILayout.Button ("Search For Ports")) {
				portsFound = ports.Length > 0;
			}

			if (portsFound && ports != null) {
				EditorGUILayout.BeginVertical (GUI.skin.box);
				for (int i = 0; i < ports.Length; i++) {
					EditorGUILayout.LabelField (ports [i]);
				}
				EditorGUILayout.EndVertical ();
			} else if (ports != null && ports.Length > 0) {
				EditorGUILayout.HelpBox ("There are ports avaliable", MessageType.Info);
			} else {
				GUI.backgroundColor = Color.red;
				EditorGUILayout.HelpBox ("No ports avaliable", MessageType.Error);
				GUI.backgroundColor = Color.white;

			}
			*/
			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Save Settings")) {
				string path = EditorUtility.SaveFilePanel ("Save Eduino Settings", "Assets", "New Eduino Settings", "sav");

				if(path != null && path != string.Empty)
					EduinoSaveLoad.Save (eduino.inputs.ToArray (), path);
			}

			if (GUILayout.Button ("Load Settings")) {
				string path = EditorUtility.OpenFilePanel ("Load Eduino Settings", "Assets", "sav");

				if(path != null && path != string.Empty)
					eduino.inputs = EduinoSaveLoad.Load (path);
			}
			EditorGUILayout.EndHorizontal ();

		}

		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical ();
	}
}
