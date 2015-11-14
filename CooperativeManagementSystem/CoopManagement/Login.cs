using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MaterialSkin;
using MaterialSkin.Controls;
namespace CoopManagement
{
    public partial class Login : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public Login()
        {
            InitializeComponent();

            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

        }

        private void Login_Load(object sender, EventArgs e)
        {
            this.ActiveControl = btnLogin;
        }
    }
}
