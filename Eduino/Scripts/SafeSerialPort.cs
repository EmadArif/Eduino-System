﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using System;

public class SafeSerialPort : SerialPort {

	private Stream theBaseStream;

	public SafeSerialPort(string portName, int baudRate) : base(portName, baudRate)
	{

	}

	public new void Open()
	{
		try
		{
			base.Open();
			theBaseStream = BaseStream;
			GC.SuppressFinalize(BaseStream);
		}
		catch
		{

		}
	}

	public new void Dispose()
	{
		Dispose(true);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && (base.Container != null))
		{
			base.Container.Dispose();               
		}
		try
		{
			if (theBaseStream.CanRead)
			{
				theBaseStream.Close();
				GC.ReRegisterForFinalize(theBaseStream);
			}
		}
		catch
		{
			// ignore exception - bug with USB - serial adapters.
		}
		base.Dispose(disposing);
	}
}
