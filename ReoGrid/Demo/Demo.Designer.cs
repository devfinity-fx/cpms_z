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

namespace unvell.ReoGrid.Demo
{
	partial class Demo
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.demoPanel = new System.Windows.Forms.Panel();
			this.demoTree = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// demoPanel
			// 
			this.demoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.demoPanel.Location = new System.Drawing.Point(258, 0);
			this.demoPanel.Name = "demoPanel";
			this.demoPanel.Size = new System.Drawing.Size(698, 657);
			this.demoPanel.TabIndex = 0;
			// 
			// demoTree
			// 
			this.demoTree.Dock = System.Windows.Forms.DockStyle.Left;
			this.demoTree.Location = new System.Drawing.Point(0, 0);
			this.demoTree.Name = "demoTree";
			this.demoTree.Size = new System.Drawing.Size(258, 657);
			this.demoTree.TabIndex = 1;
			// 
			// Demo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(956, 657);
			this.Controls.Add(this.demoPanel);
			this.Controls.Add(this.demoTree);
			this.Name = "Demo";
			this.Text = "Demo";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel demoPanel;
		private System.Windows.Forms.TreeView demoTree;
	}
}