using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EduinoSerialPort : EduinoThread {
	
	public event System.Action onUpdate;
	public event System.Action onConnected;
	public event System.Action onConnectionFail;
	public event System.Action onThreadStop;
	public event System.Action onTryConnection;

	public EduinoSerialPort(string portName, BaudRate buadRate, int delay, int readTimeout = 100, int writeTimeout = 100) 
		: base(portName, buadRate, delay, readTimeout = 100, writeTimeout = 100)
	{
	
	}
		

	protected override void OnUpdate ()
	{
		if (onUpdate != null)
			onUpdate.Invoke ();
	}

	protected override void OnConnected ()
	{
		if (onConnected != null)
			onConnected.Invoke ();
	}

	protected override void OnConnectionFail ()
	{
		if (onConnectionFail != null)
			onConnectionFail.Invoke ();
	}

	protected override void OnThreadStop ()
	{
		if (onThreadStop != null)
			onThreadStop.Invoke ();
	}

	protected override void OnTryConnection ()
	{
		base.OnTryConnection ();
		if (onTryConnection != null)
			onTryConnection.Invoke ();
	}

	protected override void OnDisconnected ()
	{
		onUpdate = null;
		onConnected = null;
		onConnectionFail = null;
		onThreadStop = null;
		onTryConnection = null;
	}
}
