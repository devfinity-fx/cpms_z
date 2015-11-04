namespace CoopManagement
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.wrapper = new SwingWERX.Controls.SwxPanel();
            this.content = new SwingWERX.Controls.SwxPanel();
            this.contentManager = new SwingWERX.Controls.PanelManager();
            this.navigation = new SwingWERX.Controls.SwxPanel();
            this.stripButton0 = new SwingWERX.Controls.StripButton();
            this.stripButton6 = new SwingWERX.Controls.StripButton();
            this.stripButton7 = new SwingWERX.Controls.StripButton();
            this.stripButton8 = new SwingWERX.Controls.StripButton();
            this.stripButton5 = new SwingWERX.Controls.StripButton();
            this.stripButton4 = new SwingWERX.Controls.StripButton();
            this.stripButton3 = new SwingWERX.Controls.StripButton();
            this.stripButton2 = new SwingWERX.Controls.StripButton();
            this.stripButton1 = new SwingWERX.Controls.StripButton();
            this.title = new SwingWERX.Controls.SwxPanel();
            this.swxPanel1 = new SwingWERX.Controls.SwxPanel();
            this.p0_Home = new SwingWERX.Controls.ManagedPanel();
            this.p1_Verifier = new SwingWERX.Controls.ManagedPanel();
            this.p2_CashCollection = new SwingWERX.Controls.ManagedPanel();
            this.p3_CashDisbursement = new SwingWERX.Controls.ManagedPanel();
            this.p4_JournalVoucher = new SwingWERX.Controls.ManagedPanel();
            this.p5_GeneralLedger = new SwingWERX.Controls.ManagedPanel();
            this.p6_SubsidiaryLedger = new SwingWERX.Controls.ManagedPanel();
            this.p7_SetupComputations = new SwingWERX.Controls.ManagedPanel();
            this.p8_Utilities = new SwingWERX.Controls.ManagedPanel();
            this.wrapper.SuspendLayout();
            this.content.SuspendLayout();
            this.contentManager.SuspendLayout();
            this.navigation.SuspendLayout();
            this.SuspendLayout();
            // 
            // wrapper
            // 
            this.wrapper.BackColor = System.Drawing.Color.Transparent;
            this.wrapper.Controls.Add(this.content);
            this.wrapper.Controls.Add(this.navigation);
            this.wrapper.Controls.Add(this.title);
            this.wrapper.Controls.Add(this.swxPanel1);
            this.wrapper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wrapper.GradientColor1 = System.Drawing.Color.Snow;
            this.wrapper.GradientColor2 = System.Drawing.Color.Snow;
            this.wrapper.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.wrapper.Location = new System.Drawing.Point(0, 0);
            this.wrapper.Name = "wrapper";
            this.wrapper.Size = new System.Drawing.Size(800, 600);
            this.wrapper.TabIndex = 0;
            // 
            // content
            // 
            this.content.BackColor = System.Drawing.Color.Transparent;
            this.content.Controls.Add(this.contentManager);
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.GradientColor1 = System.Drawing.Color.Snow;
            this.content.GradientColor2 = System.Drawing.Color.Snow;
            this.content.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.content.Location = new System.Drawing.Point(200, 30);
            this.content.Name = "content";
            this.content.Size = new System.Drawing.Size(600, 540);
            this.content.TabIndex = 4;
            // 
            // contentManager
            // 
            this.contentManager.BackColor = System.Drawing.Color.Snow;
            this.contentManager.Controls.Add(this.p0_Home);
            this.contentManager.Controls.Add(this.p1_Verifier);
            this.contentManager.Controls.Add(this.p2_CashCollection);
            this.contentManager.Controls.Add(this.p3_CashDisbursement);
            this.contentManager.Controls.Add(this.p4_JournalVoucher);
            this.contentManager.Controls.Add(this.p5_GeneralLedger);
            this.contentManager.Controls.Add(this.p6_SubsidiaryLedger);
            this.contentManager.Controls.Add(this.p7_SetupComputations);
            this.contentManager.Controls.Add(this.p8_Utilities);
            this.contentManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentManager.Location = new System.Drawing.Point(0, 0);
            this.contentManager.Name = "contentManager";
            this.contentManager.SelectedIndex = 0;
            this.contentManager.SelectedPanel = this.p0_Home;
            this.contentManager.Size = new System.Drawing.Size(600, 540);
            this.contentManager.TabIndex = 0;
            // 
            // navigation
            // 
            this.navigation.BackColor = System.Drawing.Color.Transparent;
            this.navigation.Controls.Add(this.stripButton0);
            this.navigation.Controls.Add(this.stripButton6);
            this.navigation.Controls.Add(this.stripButton7);
            this.navigation.Controls.Add(this.stripButton8);
            this.navigation.Controls.Add(this.stripButton5);
            this.navigation.Controls.Add(this.stripButton4);
            this.navigation.Controls.Add(this.stripButton3);
            this.navigation.Controls.Add(this.stripButton2);
            this.navigation.Controls.Add(this.stripButton1);
            this.navigation.Dock = System.Windows.Forms.DockStyle.Left;
            this.navigation.GradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(37)))), ((int)(((byte)(48)))));
            this.navigation.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(37)))), ((int)(((byte)(48)))));
            this.navigation.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.navigation.Location = new System.Drawing.Point(0, 30);
            this.navigation.Name = "navigation";
            this.navigation.Size = new System.Drawing.Size(200, 540);
            this.navigation.TabIndex = 1;
            // 
            // stripButton0
            // 
            this.stripButton0.AutoSize = true;
            this.stripButton0.BackColor = System.Drawing.Color.Transparent;
            this.stripButton0.Checked = true;
            this.stripButton0.DefaultForeColor = System.Drawing.Color.Snow;
            this.stripButton0.DefaultImage = global::CoopManagement.Properties.Resources.whome;
            this.stripButton0.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.stripButton0.HoverBackColor = System.Drawing.Color.WhiteSmoke;
            this.stripButton0.HoverImage = global::CoopManagement.Properties.Resources.home;
            this.stripButton0.Image = global::CoopManagement.Properties.Resources.home;
            this.stripButton0.Location = new System.Drawing.Point(0, 41);
            this.stripButton0.MaximumSize = new System.Drawing.Size(200, 42);
            this.stripButton0.MinimumSize = new System.Drawing.Size(200, 42);
            this.stripButton0.Name = "stripButton0";
            this.stripButton0.PressedBackColor = System.Drawing.Color.Snow;
            this.stripButton0.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.stripButton0.PressedImage = global::CoopManagement.Properties.Resources.home;
            this.stripButton0.Size = new System.Drawing.Size(200, 42);
            this.stripButton0.TabIndex = 0;
            this.stripButton0.Text = "Home";
            this.stripButton0.UseCustomBackgroundColors = true;
            this.stripButton0.Click += new System.EventHandler(this.ClickEvent);
            // 
            // stripButton6
            // 
            this.stripButton6.AutoSize = true;
            this.stripButton6.BackColor = System.Drawing.Color.Transparent;
            this.stripButton6.Checked = false;
            this.stripButton6.DefaultForeColor = System.Drawing.Color.Snow;
            this.stripButton6.DefaultImage = global::CoopManagement.Properties.Resources.wtools;
            this.stripButton6.ForeColor = System.Drawing.Color.Snow;
            this.stripButton6.HoverBackColor = System.Drawing.Color.WhiteSmoke;
            this.stripButton6.HoverImage = global::CoopManagement.Properties.Resources.tools;
            this.stripButton6.Image = global::CoopManagement.Properties.Resources.wtools;
            this.stripButton6.Location = new System.Drawing.Point(0, 377);
            this.stripButton6.MaximumSize = new System.Drawing.Size(200, 42);
            this.stripButton6.MinimumSize = new System.Drawing.Size(200, 42);
            this.stripButton6.Name = "stripButton6";
            this.stripButton6.PressedBackColor = System.Drawing.Color.Snow;
            this.stripButton6.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.stripButton6.PressedImage = global::CoopManagement.Properties.Resources.tools;
            this.stripButton6.Size = new System.Drawing.Size(200, 42);
            this.stripButton6.TabIndex = 7;
            this.stripButton6.Text = "Utilities";
            this.stripButton6.UseCustomBackgroundColors = true;
            this.stripButton6.Click += new System.EventHandler(this.ClickEvent);
            // 
            // stripButton7
            // 
            this.stripButton7.AutoSize = true;
            this.stripButton7.BackColor = System.Drawing.Color.Transparent;
            this.stripButton7.Checked = false;
            this.stripButton7.DefaultForeColor = System.Drawing.Color.Snow;
            this.stripButton7.DefaultImage = global::CoopManagement.Properties.Resources.wsettings;
            this.stripButton7.ForeColor = System.Drawing.Color.Snow;
            this.stripButton7.HoverBackColor = System.Drawing.Color.WhiteSmoke;
            this.stripButton7.HoverImage = global::CoopManagement.Properties.Resources.settings;
            this.stripButton7.Image = global::CoopManagement.Properties.Resources.wsettings;
            this.stripButton7.Location = new System.Drawing.Point(0, 335);
            this.stripButton7.MaximumSize = new System.Drawing.Size(200, 42);
            this.stripButton7.MinimumSize = new System.Drawing.Size(200, 42);
            this.stripButton7.Name = "stripButton7";
            this.stripButton7.PressedBackColor = System.Drawing.Color.Snow;
            this.stripButton7.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.stripButton7.PressedImage = global::CoopManagement.Properties.Resources.settings;
            this.stripButton7.Size = new System.Drawing.Size(200, 42);
            this.stripButton7.TabIndex = 6;
            this.stripButton7.Text = "Setup & Computations";
            this.stripButton7.UseCustomBackgroundColors = true;
            this.stripButton7.Click += new System.EventHandler(this.ClickEvent);
            // 
            // stripButton8
            // 
            this.stripButton8.AutoSize = true;
            this.stripButton8.BackColor = System.Drawing.Color.Transparent;
            this.stripButton8.Checked = false;
            this.stripButton8.DefaultForeColor = System.Drawing.Color.Snow;
            this.stripButton8.DefaultImage = global::CoopManagement.Properties.Resources.wsubsidiary;
            this.stripButton8.ForeColor = System.Drawing.Color.Snow;
            this.stripButton8.HoverBackColor = System.Drawing.Color.WhiteSmoke;
            this.stripButton8.HoverImage = global::CoopManagement.Properties.Resources.subsidiary;
            this.stripButton8.Image = global::CoopManagement.Properties.Resources.wsubsidiary;
            this.stripButton8.Location = new System.Drawing.Point(0, 293);
            this.stripButton8.MaximumSize = new System.Drawing.Size(200, 42);
            this.stripButton8.MinimumSize = new System.Drawing.Size(200, 42);
            this.stripButton8.Name = "stripButton8";
            this.stripButton8.PressedBackColor = System.Drawing.Color.Snow;
            this.stripButton8.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.stripButton8.PressedImage = global::CoopManagement.Properties.Resources.subsidiary;
            this.stripButton8.Size = new System.Drawing.Size(200, 42);
            this.stripButton8.TabIndex = 5;
            this.stripButton8.Text = "Subsidiary Ledger";
            this.stripButton8.UseCustomBackgroundColors = true;
            this.stripButton8.Click += new System.EventHandler(this.ClickEvent);
            // 
            // stripButton5
            // 
            this.stripButton5.AutoSize = true;
            this.stripButton5.BackColor = System.Drawing.Color.Transparent;
            this.stripButton5.Checked = false;
            this.stripButton5.DefaultForeColor = System.Drawing.Color.Snow;
            this.stripButton5.DefaultImage = global::CoopManagement.Properties.Resources.wledger;
            this.stripButton5.ForeColor = System.Drawing.Color.Snow;
            this.stripButton5.HoverBackColor = System.Drawing.Color.WhiteSmoke;
            this.stripButton5.HoverImage = global::CoopManagement.Properties.Resources.ledger;
            this.stripButton5.Image = global::CoopManagement.Properties.Resources.wledger;
            this.stripButton5.Location = new System.Drawing.Point(0, 251);
            this.stripButton5.MaximumSize = new System.Drawing.Size(200, 42);
            this.stripButton5.MinimumSize = new System.Drawing.Size(200, 42);
            this.stripButton5.Name = "stripButton5";
            this.stripButton5.PressedBackColor = System.Drawing.Color.Snow;
            this.stripButton5.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.stripButton5.PressedImage = global::CoopManagement.Properties.Resources.ledger;
            this.stripButton5.Size = new System.Drawing.Size(200, 42);
            this.stripButton5.TabIndex = 4;
            this.stripButton5.Text = "General Ledger";
            this.stripButton5.UseCustomBackgroundColors = true;
            this.stripButton5.Click += new System.EventHandler(this.ClickEvent);
            // 
            // stripButton4
            // 
            this.stripButton4.AutoSize = true;
            this.stripButton4.BackColor = System.Drawing.Color.Transparent;
            this.stripButton4.Checked = false;
            this.stripButton4.DefaultForeColor = System.Drawing.Color.Snow;
            this.stripButton4.DefaultImage = global::CoopManagement.Properties.Resources.wticket;
            this.stripButton4.ForeColor = System.Drawing.Color.Snow;
            this.stripButton4.HoverBackColor = System.Drawing.Color.WhiteSmoke;
            this.stripButton4.HoverImage = global::CoopManagement.Properties.Resources.ticket;
            this.stripButton4.Image = global::CoopManagement.Properties.Resources.wticket;
            this.stripButton4.Location = new System.Drawing.Point(0, 209);
            this.stripButton4.MaximumSize = new System.Drawing.Size(200, 42);
            this.stripButton4.MinimumSize = new System.Drawing.Size(200, 42);
            this.stripButton4.Name = "stripButton4";
            this.stripButton4.PressedBackColor = System.Drawing.Color.Snow;
            this.stripButton4.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.stripButton4.PressedImage = global::CoopManagement.Properties.Resources.ticket;
            this.stripButton4.Size = new System.Drawing.Size(200, 42);
            this.stripButton4.TabIndex = 3;
            this.stripButton4.Text = "Journal Voucher";
            this.stripButton4.UseCustomBackgroundColors = true;
            this.stripButton4.Click += new System.EventHandler(this.ClickEvent);
            // 
            // stripButton3
            // 
            this.stripButton3.AutoSize = true;
            this.stripButton3.BackColor = System.Drawing.Color.Transparent;
            this.stripButton3.Checked = false;
            this.stripButton3.DefaultForeColor = System.Drawing.Color.Snow;
            this.stripButton3.DefaultImage = global::CoopManagement.Properties.Resources.wdisbursement;
            this.stripButton3.ForeColor = System.Drawing.Color.Snow;
            this.stripButton3.HoverBackColor = System.Drawing.Color.WhiteSmoke;
            this.stripButton3.HoverImage = global::CoopManagement.Properties.Resources.disbursement;
            this.stripButton3.Image = global::CoopManagement.Properties.Resources.wdisbursement;
            this.stripButton3.Location = new System.Drawing.Point(0, 167);
            this.stripButton3.MaximumSize = new System.Drawing.Size(200, 42);
            this.stripButton3.MinimumSize = new System.Drawing.Size(200, 42);
            this.stripButton3.Name = "stripButton3";
            this.stripButton3.PressedBackColor = System.Drawing.Color.Snow;
            this.stripButton3.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.stripButton3.PressedImage = global::CoopManagement.Properties.Resources.disbursement;
            this.stripButton3.Size = new System.Drawing.Size(200, 42);
            this.stripButton3.TabIndex = 2;
            this.stripButton3.Text = "Cash Disbursement";
            this.stripButton3.UseCustomBackgroundColors = true;
            this.stripButton3.Click += new System.EventHandler(this.ClickEvent);
            // 
            // stripButton2
            // 
            this.stripButton2.AutoSize = true;
            this.stripButton2.BackColor = System.Drawing.Color.Transparent;
            this.stripButton2.Checked = false;
            this.stripButton2.DefaultForeColor = System.Drawing.Color.Snow;
            this.stripButton2.DefaultImage = global::CoopManagement.Properties.Resources.wpo;
            this.stripButton2.ForeColor = System.Drawing.Color.Snow;
            this.stripButton2.HoverBackColor = System.Drawing.Color.WhiteSmoke;
            this.stripButton2.HoverImage = global::CoopManagement.Properties.Resources.po;
            this.stripButton2.Image = global::CoopManagement.Properties.Resources.wpo;
            this.stripButton2.Location = new System.Drawing.Point(0, 125);
            this.stripButton2.MaximumSize = new System.Drawing.Size(200, 42);
            this.stripButton2.MinimumSize = new System.Drawing.Size(200, 42);
            this.stripButton2.Name = "stripButton2";
            this.stripButton2.PressedBackColor = System.Drawing.Color.Snow;
            this.stripButton2.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.stripButton2.PressedImage = global::CoopManagement.Properties.Resources.po;
            this.stripButton2.Size = new System.Drawing.Size(200, 42);
            this.stripButton2.TabIndex = 1;
            this.stripButton2.Text = "Cash Collection";
            this.stripButton2.UseCustomBackgroundColors = true;
            this.stripButton2.Click += new System.EventHandler(this.ClickEvent);
            // 
            // stripButton1
            // 
            this.stripButton1.AutoSize = true;
            this.stripButton1.BackColor = System.Drawing.Color.Transparent;
            this.stripButton1.Checked = false;
            this.stripButton1.DefaultForeColor = System.Drawing.Color.Snow;
            this.stripButton1.DefaultImage = global::CoopManagement.Properties.Resources.wsearch;
            this.stripButton1.ForeColor = System.Drawing.Color.Snow;
            this.stripButton1.HoverBackColor = System.Drawing.Color.WhiteSmoke;
            this.stripButton1.HoverImage = global::CoopManagement.Properties.Resources.search;
            this.stripButton1.Image = global::CoopManagement.Properties.Resources.wsearch;
            this.stripButton1.Location = new System.Drawing.Point(0, 83);
            this.stripButton1.MaximumSize = new System.Drawing.Size(200, 42);
            this.stripButton1.MinimumSize = new System.Drawing.Size(200, 42);
            this.stripButton1.Name = "stripButton1";
            this.stripButton1.PressedBackColor = System.Drawing.Color.Snow;
            this.stripButton1.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.stripButton1.PressedImage = global::CoopManagement.Properties.Resources.search;
            this.stripButton1.Size = new System.Drawing.Size(200, 42);
            this.stripButton1.TabIndex = 0;
            this.stripButton1.Text = "Verifier";
            this.stripButton1.UseCustomBackgroundColors = true;
            this.stripButton1.Click += new System.EventHandler(this.ClickEvent);
            // 
            // title
            // 
            this.title.BackColor = System.Drawing.Color.Transparent;
            this.title.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.title.Font = new System.Drawing.Font("Segoe UI Light", 14F, System.Drawing.FontStyle.Bold);
            this.title.ForeColor = System.Drawing.Color.Snow;
            this.title.GradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.title.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.title.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.title.Location = new System.Drawing.Point(0, 570);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(800, 30);
            this.title.TabIndex = 0;
            this.title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // swxPanel1
            // 
            this.swxPanel1.BackColor = System.Drawing.Color.Transparent;
            this.swxPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.swxPanel1.Font = new System.Drawing.Font("Segoe UI Light", 14F, System.Drawing.FontStyle.Bold);
            this.swxPanel1.ForeColor = System.Drawing.Color.Snow;
            this.swxPanel1.GradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.swxPanel1.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            this.swxPanel1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.swxPanel1.Location = new System.Drawing.Point(0, 0);
            this.swxPanel1.Name = "swxPanel1";
            this.swxPanel1.Size = new System.Drawing.Size(800, 30);
            this.swxPanel1.TabIndex = 5;
            this.swxPanel1.Text = "     Coop Management System";
            this.swxPanel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // p0_Home
            // 
            this.p0_Home.Location = new System.Drawing.Point(0, 0);
            this.p0_Home.Name = "p0_Home";
            this.p0_Home.Size = new System.Drawing.Size(600, 540);
            // 
            // p1_Verifier
            // 
            this.p1_Verifier.Location = new System.Drawing.Point(0, 0);
            this.p1_Verifier.Name = "p1_Verifier";
            this.p1_Verifier.Size = new System.Drawing.Size(600, 540);
            this.p1_Verifier.Text = "managedPanel3";
            // 
            // p2_CashCollection
            // 
            this.p2_CashCollection.Location = new System.Drawing.Point(0, 0);
            this.p2_CashCollection.Name = "p2_CashCollection";
            this.p2_CashCollection.Size = new System.Drawing.Size(600, 540);
            this.p2_CashCollection.Text = "managedPanel3";
            // 
            // p3_CashDisbursement
            // 
            this.p3_CashDisbursement.Location = new System.Drawing.Point(0, 0);
            this.p3_CashDisbursement.Name = "p3_CashDisbursement";
            this.p3_CashDisbursement.Size = new System.Drawing.Size(600, 540);
            this.p3_CashDisbursement.Text = "managedPanel3";
            // 
            // p4_JournalVoucher
            // 
            this.p4_JournalVoucher.Location = new System.Drawing.Point(0, 0);
            this.p4_JournalVoucher.Name = "p4_JournalVoucher";
            this.p4_JournalVoucher.Size = new System.Drawing.Size(600, 540);
            this.p4_JournalVoucher.Text = "managedPanel3";
            // 
            // p5_GeneralLedger
            // 
            this.p5_GeneralLedger.Location = new System.Drawing.Point(0, 0);
            this.p5_GeneralLedger.Name = "p5_GeneralLedger";
            this.p5_GeneralLedger.Size = new System.Drawing.Size(600, 540);
            this.p5_GeneralLedger.Text = "managedPanel3";
            // 
            // p6_SubsidiaryLedger
            // 
            this.p6_SubsidiaryLedger.Location = new System.Drawing.Point(0, 0);
            this.p6_SubsidiaryLedger.Name = "p6_SubsidiaryLedger";
            this.p6_SubsidiaryLedger.Size = new System.Drawing.Size(600, 540);
            this.p6_SubsidiaryLedger.Text = "managedPanel3";
            // 
            // p7_SetupComputations
            // 
            this.p7_SetupComputations.Location = new System.Drawing.Point(0, 0);
            this.p7_SetupComputations.Name = "p7_SetupComputations";
            this.p7_SetupComputations.Size = new System.Drawing.Size(600, 540);
            this.p7_SetupComputations.Text = "managedPanel3";
            // 
            // p8_Utilities
            // 
            this.p8_Utilities.Location = new System.Drawing.Point(0, 0);
            this.p8_Utilities.Name = "p8_Utilities";
            this.p8_Utilities.Size = new System.Drawing.Size(600, 540);
            this.p8_Utilities.Text = "managedPanel3";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.wrapper);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cooperative Management System";
            this.Load += new System.EventHandler(this.LoadEvent);
            this.wrapper.ResumeLayout(false);
            this.content.ResumeLayout(false);
            this.contentManager.ResumeLayout(false);
            this.navigation.ResumeLayout(false);
            this.navigation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SwingWERX.Controls.SwxPanel wrapper;
        private SwingWERX.Controls.SwxPanel title;
        private SwingWERX.Controls.SwxPanel navigation;
        private SwingWERX.Controls.StripButton stripButton4;
        private SwingWERX.Controls.StripButton stripButton3;
        private SwingWERX.Controls.StripButton stripButton2;
        private SwingWERX.Controls.StripButton stripButton1;
        private SwingWERX.Controls.StripButton stripButton5;
        private SwingWERX.Controls.StripButton stripButton6;
        private SwingWERX.Controls.StripButton stripButton7;
        private SwingWERX.Controls.StripButton stripButton8;
        private SwingWERX.Controls.StripButton stripButton0;
        private SwingWERX.Controls.SwxPanel content;
        private SwingWERX.Controls.SwxPanel swxPanel1;
        private SwingWERX.Controls.PanelManager contentManager;
        private SwingWERX.Controls.ManagedPanel p0_Home;
        private SwingWERX.Controls.ManagedPanel p1_Verifier;
        private SwingWERX.Controls.ManagedPanel p2_CashCollection;
        private SwingWERX.Controls.ManagedPanel p3_CashDisbursement;
        private SwingWERX.Controls.ManagedPanel p4_JournalVoucher;
        private SwingWERX.Controls.ManagedPanel p5_GeneralLedger;
        private SwingWERX.Controls.ManagedPanel p6_SubsidiaryLedger;
        private SwingWERX.Controls.ManagedPanel p7_SetupComputations;
        private SwingWERX.Controls.ManagedPanel p8_Utilities;
    }
}

