using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using UnityEngine.Events;
using System.Linq;
using System.Threading;

public enum BaudRate
{
	m_300,
	m_1200,
	m_2400,
	m_4800,
	m_9600,
	m_19200,
	m_38400,
	m_57600,
}
