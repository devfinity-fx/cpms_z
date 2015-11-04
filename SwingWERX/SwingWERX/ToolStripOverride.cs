using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SwingWERX
{
    public class ToolStripOverride : ToolStripSystemRenderer
    {
        public Color BackgroundColor { get; set; }

        public ToolStripOverride() { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {

        }

        protected override void InitializePanel(ToolStripPanel toolStripPanel)
        {
            base.InitializePanel(toolStripPanel);
            toolStripPanel.BackColor = BackgroundColor;
        }
    }
}
