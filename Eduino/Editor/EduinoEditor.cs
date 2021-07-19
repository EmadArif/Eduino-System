using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Eduino))]
public class EduinoEditor : EduinoBaseEditor {

	private Eduino connector;
	private SerializedProperty inputsProp;

	protected override void OnEnable()
	{
		base.OnEnable ();
		inputsProp = serializedObject.FindProperty ("inputs");

		connector = target as Eduino;
	}

	public override void OnInspectorGUI ()
	{
		DrawBanner ();

		serializedObject.Update ();

		DrawDefaultProperties ();
		DrawAdvancedProperties (connector);
		DrawInputProperties ();

		serializedObject.ApplyModifiedProperties ();
	}

	void DrawInputProperties()
	{
		EditorGUI.indentLevel++;
		EditorGUILayout.BeginVertical (GUI.skin.box);
		inputsProp.isExpanded = EditorGUILayout.Foldout (inputsProp.isExpanded, "Inputs", true);
		if (inputsProp.isExpanded) {

			for (int i = 0; i < inputsProp.arraySize; i++) {
				
				EditorGUILayout.BeginVertical (GUI.skin.box);

				EditorGUILayout.BeginHorizontal ();

				EditorGUILayout.PropertyField (inputsProp.GetArrayElementAtIndex (i), true);

				GUI.color = Color.red;
				if (GUILayout.Button ("X", GUILayout.MaxWidth(30))) {
					connector.inputs.RemoveAt (i);
					break;
				}
				GUI.color = Color.white;

				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.EndVertical ();
			}

			if (GUILayout.Button ("Add New Input"))
				connector.inputs.Add (new EduinoDataInput ());
		}

		EditorGUILayout.EndVertical ();
		EditorGUI.indentLevel--;
	}
}
