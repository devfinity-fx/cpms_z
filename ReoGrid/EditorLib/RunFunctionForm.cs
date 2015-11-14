/*****************************************************************************
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#if EX_SCRIPT
using unvell.ReoScript;
using unvell.ReoScript.Reflection;
#endif

namespace unvell.ReoGrid.Editor
{
	public partial class RunFunctionForm : Form
	{
#if EX_SCRIPT
		public ScriptRunningMachine Srm { get; set; }

		public CompiledScript Script { get; set; }
#endif

		public RunFunctionForm()
		{
			InitializeComponent();

			btnRun.Enabled = false;

#if EX_SCRIPT
			functionList.SelectedIndexChanged += (s, e) =>
			{
				btnRun.Enabled = functionList.SelectedIndex >= 0 && Srm != null;
			};
#endif
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

#if EX_SCRIPT
			if (Script != null)
			{
				foreach (var fun in Script.DeclaredFunctions)
				{
					functionList.Items.Add(new FunctionListItem(fun));
				}
			}
#endif
		}

#if EX_SCRIPT
		class FunctionListItem
		{
			public FunctionInfo Function { get; set; }

			public FunctionListItem(FunctionInfo function)
			{
				this.Function = function;
			}

			public override string ToString()
			{
				return Function.Name;
			}
		}
#endif

		private void btnRun_Click(object sender, EventArgs e)
		{
#if EX_SCRIPT
			Srm.RunCompiledScript(Script);
			Srm.InvokeFunctionIfExisted(((FunctionListItem)functionList.SelectedItem).Function.Name);
#endif
			if (chkCloseAfterRun.Checked) Close();
		}
	}
}