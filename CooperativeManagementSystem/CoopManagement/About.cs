using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using MaterialSkin;
using MaterialSkin.Controls;

using unvell.ReoGrid;

namespace CoopManagement
{
    public partial class About : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public About()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            richInputBox1.BackColor = Color.FromKnownColor(KnownColor.Snow);
            
            reoGridControl1.Load("license.rgf");
            reoGridControl1.SetSettings(ReoGridSettings.View_ShowRowHeader |
                ReoGridSettings.View_ShowColumnHeader, false);
            reoGridControl1.SetSettings(ReoGridSettings.Readonly, true);
            reoGridControl1.SelectionMode = ReoGridSelectionMode.None;
            reoGridControl1.SetSettings(ReoGridSettings.View_ShowScrolls, false);
            

        }

        private async void Devs_Click(object sender, EventArgs e)
        {
            await Task.Delay(250);
            panelManager.SelectedPanel = p1_Dev;
        }

        private async void License_Click(object sender, EventArgs e)
        {
            await Task.Delay(250);
            panelManager.SelectedPanel = p2_License;
        }

        private async void Help_Click(object sender, EventArgs e)
        {
            await Task.Delay(250);
            panelManager.SelectedPanel = p3_Help;
        }

        private async void Back_Event(object sender, EventArgs e)
        {
            await Task.Delay(250);
            panelManager.SelectedPanel = p0_Index;
        }
    }
}
