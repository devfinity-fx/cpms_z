﻿/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.codeplex.com/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * This software released under LGPLv3 license.
 * Author: Jing Lu <dujid0 at gmail.com>
 * 
 * Copyright (c) 2012-2014 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid;

namespace unvell.ReoGrid.TestCases
{
	class ConsoleRunner
	{
		private ReoGridControl grid = null;

		public ConsoleRunner()
		{
			TestCaseManager.Instance.StartTestSet += new EventHandler<TestSetEventArgs>(Instance_StartTestSet);
			TestCaseManager.Instance.FinishTestSet += new EventHandler<TestSetEventArgs>(Instance_FinishTestSet);
			TestCaseManager.Instance.BeforePerformTestCase += new EventHandler<TestCaseEventArgs>(Instance_BeforePerformTestCase);
			TestCaseManager.Instance.AfterPerformTestCase += new EventHandler<TestCaseEventArgs>(Instance_AfterPerformTestCase);
		}

		public bool Run()
		{
			try
			{
				unvell.Common.Logger.Off();

				Console.WriteLine("Current Memory Usage: " +
					(System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / 1024).
					ToString("###,###,###")+ " KB \r\n");

				bool rs = TestCaseManager.Instance.Run();

				Console.WriteLine(
					string.Format("  total {0} tests, {1} success\n",
					TestCaseManager.Instance.TestSets.Sum(ts => ts.Performes),
					TestCaseManager.Instance.TestSets.Sum(ts => ts.Successes)));

				return rs;
			}
			finally
			{
				unvell.Common.Logger.On();
			}

			//Console.WriteLine();
			//Console.WriteLine("Press any key to continue...");
			//Console.ReadKey();
		}

		void Instance_StartTestSet(object sender, TestSetEventArgs e)
		{
			Console.WriteLine("Test suite: " + e.TestSetInfo.Name);
	
			if (e.TestSetInfo.Instance is ReoGridTestSet)
			{
				if (this.grid == null)
				{
					this.grid = new ReoGridControl();
				}
				((ReoGridTestSet)e.TestSetInfo.Instance).Grid = this.grid;
			}
		}

		void Instance_BeforePerformTestCase(object sender, TestCaseEventArgs e)
		{
			Console.Write(string.Format("    {0,-38}... ", e.TestCaseInfo.Name));
		}

		void Instance_AfterPerformTestCase(object sender, TestCaseEventArgs e)
		{
			Console.WriteLine(string.Format("{0} ( {1,4} ms. {2,8} kb. )",
				(e.TestCaseInfo.Exception != null ? e.TestCaseInfo.ToString() : "passed"), 
				e.TestCaseInfo.ElapsedMilliseconds, (e.TestCaseInfo.MemoryUsage/1024).ToString("###,###,##0")));
			if (e.TestCaseInfo.Exception != null)
			{
				Console.WriteLine("    " + e.TestCaseInfo.Exception.ToString());
			}
		}

		void Instance_FinishTestSet(object sender, TestSetEventArgs e)
		{
			Console.WriteLine("\t{0} tests, {1} success\r\n", e.TestSetInfo.Count, e.TestSetInfo.Successes);
		}	
		
	}
}
