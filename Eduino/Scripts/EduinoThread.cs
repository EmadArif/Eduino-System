using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using UnityEngine.Events;
using System.Linq;
using System.Threading;


public abstract class EduinoThread {

	protected string portName;
	protected int delay;
	protected int readTimeout;
	protected int writeTimeout;
	public SafeSerialPort port{ get; protected set;}
	protected bool isRunning = true;
	protected int baudRateValue;
	private BaudRate baudRate;
	public SerialPortConnectionState ConnectionState{ get; private set;}

	protected Thread portThread;

	private Queue inputQueue, outputQueue;

	public EduinoThread(string portName, BaudRate buadRate, int delay, int readTimeout = 100, int writeTimeout = 100)
	{
		this.portName = portName;
		this.delay = delay;
		this.readTimeout = readTimeout;
		this.writeTimeout = writeTimeout;
		this.baudRateValue = int.Parse(buadRate.ToString ().Replace("m_", ""));
		this.baudRate = baudRate;

		inputQueue = Queue.Synchronized (new Queue ());
		outputQueue = Queue.Synchronized (new Queue ());
		SetState (SerialPortConnectionState.Disconnected);
	}

	public static string[] GetAvailablePorts()
	{
		return SafeSerialPort.GetPortNames ();
	}

	public SerialPort GetSerialPort()
	{
		return port;
	}

	public void Start()
	{
		lock(this)
			isRunning = true;
		
		portThread = new Thread (new ThreadStart(RunThread));
		portThread.Name = "Eduino Serial Port Thread";
		portThread.Start ();
	}


	public void Stop()
	{
		lock(this)
			isRunning = false;
	}

	void RunThread()
	{
		SetState (SerialPortConnectionState.Disconnected);

		while(IsRunning())
		{
			try {
				if(ConnectionState == SerialPortConnectionState.Disconnected)
					InitilizeConnection ();

				SetState (SerialPortConnectionState.Connected);

				if(ConnectionState == SerialPortConnectionState.Connected)
				{
					if(!port.IsOpen)
						port.Open();

					if (port.IsOpen)
					{
						ReadWrite();
						OnUpdate();
					}else
					{
						SetState (SerialPortConnectionState.Disconnected);
					}
				}

			} catch (IOException ex) {
				port.Dispose();
				OnConnectionFail();
				SetState (SerialPortConnectionState.Disconnected);

			}

			Debug.Log ("Port Connection : " + port.IsOpen);

			Thread.Sleep(delay);
		}

		SetState(SerialPortConnectionState.Stopped);
		OnThreadStop ();
	}

	public object ReadMessage()
	{
		if (inputQueue.Count == 0 || inputQueue == null)
			return null;
		
		return (object)inputQueue.Dequeue ();
	}

	public void WriteMessage(string message)
	{
		outputQueue.Enqueue (message);
	}

	void SetState(SerialPortConnectionState state)
	{
		lock (this)
			ConnectionState = state;
	}

	void InitilizeConnection()
	{
		while (ConnectionState == SerialPortConnectionState.Disconnected) {
			try {
				SetState(SerialPortConnectionState.Disconnected);
				OnTryConnection();

				string[] ports = GetAvailablePorts ();
				bool portFound = false;
				for (int i = 0; i < ports.Length; i++) {

					portName = ports[i];
					port = new SafeSerialPort (portName, baudRateValue);
					port.Open();
					port.ReadByte();
					OnConnected();
					portFound = true;
					break;
					Thread.Sleep(1000);
				}

				if(portFound)
					break;

				Thread.Sleep(1000);

			} catch (System.Exception ex) {
				SetState(SerialPortConnectionState.Disconnected);
				port.Dispose ();
				if(port.IsOpen)
					port.Close();
			}

			Thread.Sleep(1000);
		}

		Debug.Log(ConnectionState);

	}

	public static bool TestConnection(string _portName, BaudRate _buadRate)
	{
		int testBaudRateValue = int.Parse(_buadRate.ToString ().Replace("m_", ""));
		SafeSerialPort testPort = new SafeSerialPort (_portName, testBaudRateValue);

		try {
			if(!testPort.IsOpen)
				testPort.Open();
		} catch (System.Exception ex) {
			Debug.Log ("error");
			testPort.Close ();
		}

		return testPort.IsOpen;
	}
		
	public bool IsRunning()
	{
		lock(this)
			return isRunning;
	}

	public void Disconnect()
	{
		if (!IsRunning ())
			return;

		Stop();

		if(port != null)
		{
			if(port.IsOpen)
				port.Close();
		}

		portThread.Join ();
		Debug.Log (portThread.ThreadState);

		OnDisconnected ();
	}

	void ReadWrite()
	{
		string readedString = port.ReadLine ();

		if (inputQueue.Count > 5) {
			inputQueue.Dequeue ();
		}

		inputQueue.Enqueue (readedString);


		if (outputQueue.Count > 0) {
			port.WriteLine ((string)outputQueue.Dequeue ());
		}
	}

	protected virtual void OnTryConnection()
	{
		Debug.Log ("Try Connection...");

	}

	protected virtual void OnConnected()
	{
		
		Debug.Log ("Connected Successfully");
	}

	protected virtual void OnUpdate()
	{

	}

	protected virtual void OnConnectionFail()
	{
		Debug.Log ("Connection Fails");
	}

	protected virtual void OnThreadStop()
	{
		Debug.Log ("Thread has been stopped");
	}

	protected virtual void OnDisconnected()
	{
		Debug.Log ("Disconnected");
	}
}
