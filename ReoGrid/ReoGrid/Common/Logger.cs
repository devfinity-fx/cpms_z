﻿/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.codeplex.com/
 * 
 * Common Logger Component
 * 
 * - Common log framework to support .NET Applications output information.
 * 
 * - By implementing and specifying ILogWritter interface to write log 
 *   to different destination, such as console, stream and file.
 *   
 * This software released under LGPLv3 license.
 * Author: Jing Lu <dujid0 at gmail.com>
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 * 
 * Copyright (c) 2012-2014 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace unvell.Common
{
	public enum LogLevel : byte
	{
		All = 0,
		Trace = 1,
		Debug = 2,
		Info = 3,
		Warn = 4,
		Error = 5,
		Fatal = 6,
	}

	public interface ILogWritter
	{
		void Log(string cat, string msg);
	}
	
	public class Logger
	{
		private static readonly Logger instance = new Logger();
		internal static Logger Instance { get { return instance; } }

		private List<ILogWritter> writters = new List<ILogWritter>();

		private Logger() {
			writters.Add(new ConsoleLogger());
#if LOG_TO_FILE
			writters.Add(new DebugFileLogger());
#endif
		}

		public static void RegisterWritter(ILogWritter writter)
		{
			instance.writters.Add(writter);
		}

		private bool turnSwitch = true;

		public static void Off()
		{
			instance.turnSwitch = false;
		}

		public static void On()
		{
			instance.turnSwitch = true;
		}

		public static void Log(string cat, string format, params object[] args)
		{
			Log(cat, string.Format(format, args));
		}
		public static void Log(string cat, string msg)
		{
			instance.WriteLog(cat, msg);
		}
		public void WriteLog(string cat, string msg)
		{
			if(turnSwitch) writters.ForEach(w => w.Log(cat, msg));
		}
	}

	class ConsoleLogger : ILogWritter
	{
		#region ILogWritter Members

		public void Log(string cat, string msg)
		{
			Console.WriteLine(string.Format("[{0}] {1}: {2}",
				DateTime.Now.ToString(), cat, msg));
		}

		#endregion
	}

#if DEBUG1
	class DebugFileLogger : ILogWritter
	{
		private StreamWriter sw;
		public DebugFileLogger()
		{
			string path = Path.Combine(
			Path.GetDirectoryName(Application.ExecutablePath),
			"debug.log");
			try
			{
				sw = new StreamWriter(new FileStream(path, FileMode.Append));
			}
			catch { }
		}

		~DebugFileLogger()
		{
			try
			{
				if (sw != null && sw.BaseStream.CanWrite)
				{
					sw.Flush();
					sw.Dispose();
				}
			}
			catch { }
		}

		public void Log(string cat, string msg)
		{
			if (sw != null)
			{
				sw.WriteLine("[{0}] {1}: {2}", DateTime.Now.ToString(), cat, msg);
				sw.Flush();
			}
		}
	}
#endif
}
