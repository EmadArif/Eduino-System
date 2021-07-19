using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EduinoDataInput{
	public enum InputDataType{
		String,
		Boolean,
		Float,
		Int
	}

	[System.Serializable]
	public class InputOptions
	{
		public float triggerThreshold = 0;
		public bool flipValue = false;
	}

	public string Value { get; private set;}
	public string name;
	public InputDataType type;
	public InputOptions options;

	private bool isChanged = false;

	public void SetValue(string _value)
	{
		Value = _value;
	}

	public T GetValue<T>()
	{
		return (T)GetValue ();
	}

	private object GetValue()
	{
		switch (type) {
		case InputDataType.Boolean:
			bool btn = (Value == "1") ? true : false;
			return options.flipValue ? !btn : btn;

		case InputDataType.Float:

			float fres = 0;
			float.TryParse (Value, out fres);
			return options.flipValue ? -fres : fres;

		case InputDataType.Int:
			int ires = 0;
			int.TryParse (Value, out ires);
			return options.flipValue ? -ires : ires;

		case InputDataType.String:
			return Value.ToString ();
		}

		return Value;
	}
	public bool GetTrigger()
	{
		switch (type) {
		case InputDataType.Boolean:

			bool btn = false;
			if (!isChanged) {
				btn = (Value == "1") ? true : false;
				isChanged = true;
			}

			if (Value == "0")
				isChanged = false;

			return btn;

		case InputDataType.Float:

			bool fstate = false;
			float fres = 0;
			float.TryParse (Value, out fres);

			if (!isChanged) {
				if (Mathf.Abs(fres) >= options.triggerThreshold) {
					isChanged = true;
					fstate = true;
				}
			}

			if (Mathf.Abs(fres) < options.triggerThreshold)
				isChanged = false;

			return fstate;

		}

		return false;
	}
}

