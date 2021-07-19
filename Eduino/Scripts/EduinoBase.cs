using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EduinoBase : MonoBehaviour {

	public string portName;
	public BaudRate buadRate;
	public int delay;
	public int readTimeout = 100;
	public int writeTimeout = 100;

	protected EduinoSerialPort serialPort;

	public SerialPortConnectionState ConnectionState{
		get{ 
			return serialPort.ConnectionState;
		}
	}

	public bool IsConnected{
		get{ 
			return serialPort.ConnectionState == SerialPortConnectionState.Connected;
		}
	}

	public bool IsDisconnected{
		get{ 
			return serialPort.ConnectionState == SerialPortConnectionState.Disconnected;
		}
	}

	protected virtual void OnEnable()
	{
		serialPort = new EduinoSerialPort (portName, buadRate, delay, readTimeout, writeTimeout);
		serialPort.Start ();

	}

	public static void AutoConnecteWithPort()
	{

	}

	protected virtual void OnDisable()
	{
		if(serialPort != null)
			serialPort.Disconnect ();
	}

	protected virtual void OnDestroy()
	{
		if(serialPort != null)
			serialPort.Disconnect ();
	}

	protected virtual void OnApplicationQuit()
	{
		if(serialPort != null)
			serialPort.Disconnect ();
	}
}
