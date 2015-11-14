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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

using unvell.ReoGrid.Properties;

#if EX_SCRIPT
using unvell.ReoScript;
using unvell.ReoGrid.Script;
#endif

namespace unvell.ReoGrid
{
	public partial class ReoGridControl
	{
		#region Script
		private string script;

		/// <summary>
		/// Script content for this control
		/// </summary>
		public string Script { get { return script; } set { script = value; } }

#if EX_SCRIPT
		// TODO: references of cells and ranges should be managed by ReferenceManager
		//       create a new class ReferenceManager
		private Dictionary<ReoGridCell, List<ReoGridCell>> formulaCells;
		private Dictionary<ReoGridCell, List<ReferenceRange>> formulaRanges;

		/// <summary>
		/// When the cell is referenced by another cell which using formula,
		/// It's need notify the latter to recalculate the formula.
		/// 
		///             A              B
		/// 1           5             =A1            
		/// 2
		/// -------------------------------------------
		///             A              B
		/// 1         Cell       Formula-Cells            
		/// 2
		/// 
		/// This function find what formula-cells should refreshed and
		/// perform the recalculating.
		/// 
		/// When any value of cells changed, this method should be called once.
		/// 
		/// </summary>
		/// <param name="cell">cell may referenced by other formula-cells</param>
		internal void UpdateReferencedFormulaCells(ReoGridCell cell)
		{
			List<ReoGridCell> dirtyCells = new List<ReoGridCell>();

			// cells
			foreach (var formulaCell in formulaCells)
			{
				if (formulaCell.Value.Any(c => c == cell))
				{
					if (!dirtyCells.Contains(formulaCell.Key))
					{
						dirtyCells.Add(formulaCell.Key);
					}
				}
			}

			// ranges
			foreach (var range in formulaRanges)
			{
				if (range.Value.Any(r => r.Contains(cell)))
				{
					if (!dirtyCells.Contains(range.Key))
					{
						dirtyCells.Add(range.Key);
					}
				}
			}

			foreach (var dirtyCell in dirtyCells)
			{
				SetCellData(dirtyCell, dirtyCell.Formula);
			}
		}

		private void ClearReferenceListForCell(ReoGridCell cell)
		{
			if (formulaCells.ContainsKey(cell))
				formulaCells.Remove(cell);
			if (formulaRanges.ContainsKey(cell))
				formulaRanges.Remove(cell);
		}

		/// <summary>
		/// ReoScript Runtime Machine 
		/// </summary>
		private ScriptRunningMachine srm;

		/// <summary>
		/// ReoScript Runtime Machine
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public ScriptRunningMachine Srm
		{
			get { return srm; }
			//set { srm = value; }
		}

		/// <summary>
		/// Run script belongs to this control
		/// </summary>
		/// <returns>Last value returned from script execution</returns>
		public object RunScript()
		{
			return RunScript(this.script);
		}

		/// <summary>
		/// Run specified script
		/// </summary>
		/// <param name="script">Script to be executed</param>
		/// <returns>Last value returned from script execution</returns>
		public object RunScript(string script)
		{
			if (srm == null)
			{
				InitSRM();
			}

			return srm == null ? null : srm.Run(script);
		}

		private RSGridObject gridObj;

		private static Regex cellIdRegex;

		/// <summary>
		/// Initial or reset Script Running Machine
		/// </summary>
		private void InitSRM()
		{
#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif

			if (srm == null)
			{
				// create ReoScript instance
				srm = new ScriptRunningMachine();

				// reinit instance when SRM is reset
				srm.Resetted += (s, e) => InitSRM();
			}

			// set control instance into script's context
			if (gridObj == null)
			{
				gridObj = new RSGridObject(this);
			}

			srm["grid"] = gridObj;

			if (formulaCells == null)
			{
				formulaCells = new Dictionary<ReoGridCell, List<ReoGridCell>>();
			}
			else
			{
				formulaCells.Clear();
			}

			if (formulaRanges == null)
			{
				formulaRanges = new Dictionary<ReoGridCell, List<ReferenceRange>>();
			}
			else
			{
				formulaRanges.Clear();
			}

			if (cellIdRegex == null) cellIdRegex = new Regex(@"([A-Z]+)([0-9]+)");

			// setup built-in functions
			RSFunctions.SetupBuiltinFunctions(this, srm);

			// load core library
			using (MemoryStream ms = new MemoryStream(Resources.base_lib))
			{
				srm.Load(ms);
			}

#if DEBUG
			sw.Stop();
			Debug.WriteLine("init srm takes " + sw.ElapsedMilliseconds + " ms.");
#endif
		}

		/// <summary>
		/// Fire predefined event to call function of script
		/// </summary>
		/// <param name="eventName">Event name (global function name in script)</param>
		public object RaiseScriptEvent(string eventName)
		{
			return RaiseScriptEvent(eventName, null);
		}
		/// <summary>
		/// Fire predefined event to call function of script
		/// </summary>
		/// <param name="eventName">Event name (global function name in script)</param>
		/// <param name="eventArg">Event arguments</param>
		public object RaiseScriptEvent(string eventName, ObjectValue eventArg)
		{
			if (srm == null)
			{
				InitSRM();
			}

			return (gridObj != null) ? srm.InvokeFunctionIfExisted(gridObj, eventName, eventArg) : null;
		}

		/// <summary>
		/// Recalculate and get the value of formula stored in the specified cell.
		/// </summary>
		/// <param name="pos">Position of cell to be recalculated</param>
		public void RecalcCell(ReoGridPos pos)
		{
			RecalcCell(pos.Row, pos.Col);
		}

		/// <summary>
		/// Recalculate and get the value of formula stored in the specified cell.
		/// </summary>
		/// <param name="row">Index of row of cell</param>
		/// <param name="col">Index of column of cell</param>
		public void RecalcCell(int row, int col)
		{
			ReoGridCell cell = GetCell(row, col);
			if (cell == null) return;
			RecalcCell(cell);
		}

		/// <summary>
		/// Recalculate and get the value of formula stored in the specified cell.
		/// </summary>
		/// <param name="cell">Instance of cell to be recalculated</param>
		internal void RecalcCell(ReoGridCell cell)
		{
			SetCellFormula(cell, cell.Formula);
		}
#endif // EX_SCRIPT

		#endregion
	}
}
