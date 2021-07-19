using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using UnityEngine.Events;
using System.Linq;
using System.Threading;

public enum SerialPortConnectionState{
	Connected,
	Disconnected,
	Stopped
}
