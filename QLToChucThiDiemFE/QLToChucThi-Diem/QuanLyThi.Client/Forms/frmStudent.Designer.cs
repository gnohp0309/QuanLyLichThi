namespace QuanLyThi.Client.Forms
{
    partial class frmStudent
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelMenu;
        private Panel panelContent;
        private Button btnHome;
        private Button btnRegistration;
        private Button btnScores;
        private Button btnExamSchedules;
        private Button btnNotifications;
        private Button btnLogout;
        private Label lblWelcome;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelMenu = new Panel();
            this.btnLogout = new Button();
            this.btnNotifications = new Button();
            this.btnExamSchedules = new Button();
            this.btnScores = new Button();
            this.btnRegistration = new Button();
            this.btnHome = new Button();
            this.lblWelcome = new Label();
            this.panelContent = new Panel();
            this.panelMenu.SuspendLayout();
            this.SuspendLayout();
            
            // panelMenu
            this.panelMenu.BackColor = Color.FromArgb(240, 240, 240);
            this.panelMenu.Controls.Add(this.btnLogout);
            this.panelMenu.Controls.Add(this.btnNotifications);
            this.panelMenu.Controls.Add(this.btnExamSchedules);
            this.panelMenu.Controls.Add(this.btnScores);
            this.panelMenu.Controls.Add(this.btnRegistration);
            this.panelMenu.Controls.Add(this.btnHome);
            this.panelMenu.Controls.Add(this.lblWelcome);
            this.panelMenu.Dock = DockStyle.Left;
            this.panelMenu.Size = new Size(200, 800);
            
            // lblWelcome
            this.lblWelcome.BackColor = Color.FromArgb(0, 102, 204);
            this.lblWelcome.ForeColor = Color.White;
            this.lblWelcome.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblWelcome.Text = "SINH VIÊN";
            this.lblWelcome.Dock = DockStyle.Top;
            this.lblWelcome.TextAlign = ContentAlignment.MiddleCenter;
            this.lblWelcome.Height = 50;
            
            // btnHome
            this.btnHome.Dock = DockStyle.Top;
            this.btnHome.Text = "Trang chủ";
            this.btnHome.FlatStyle = FlatStyle.Flat;
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.Click += btnHome_Click;
            this.btnHome.Height = 50;
            this.btnHome.TextAlign = ContentAlignment.MiddleLeft;
            this.btnHome.Padding = new Padding(20, 0, 0, 0);
            
            // btnRegistration
            this.btnRegistration.Dock = DockStyle.Top;
            this.btnRegistration.Text = "Đăng ký học phần";
            this.btnRegistration.FlatStyle = FlatStyle.Flat;
            this.btnRegistration.FlatAppearance.BorderSize = 0;
            this.btnRegistration.Click += btnRegistration_Click;
            this.btnRegistration.Height = 50;
            this.btnRegistration.TextAlign = ContentAlignment.MiddleLeft;
            this.btnRegistration.Padding = new Padding(20, 0, 0, 0);
            
            // btnScores
            this.btnScores.Dock = DockStyle.Top;
            this.btnScores.Text = "Điểm thi";
            this.btnScores.FlatStyle = FlatStyle.Flat;
            this.btnScores.FlatAppearance.BorderSize = 0;
            this.btnScores.Click += btnScores_Click;
            this.btnScores.Height = 50;
            this.btnScores.TextAlign = ContentAlignment.MiddleLeft;
            this.btnScores.Padding = new Padding(20, 0, 0, 0);
            
            // btnExamSchedules
            this.btnExamSchedules.Dock = DockStyle.Top;
            this.btnExamSchedules.Text = "Lịch thi";
            this.btnExamSchedules.FlatStyle = FlatStyle.Flat;
            this.btnExamSchedules.FlatAppearance.BorderSize = 0;
            this.btnExamSchedules.Click += btnExamSchedules_Click;
            this.btnExamSchedules.Height = 50;
            this.btnExamSchedules.TextAlign = ContentAlignment.MiddleLeft;
            this.btnExamSchedules.Padding = new Padding(20, 0, 0, 0);
            
            // btnNotifications
            this.btnNotifications.Dock = DockStyle.Top;
            this.btnNotifications.Text = "Thông báo";
            this.btnNotifications.FlatStyle = FlatStyle.Flat;
            this.btnNotifications.FlatAppearance.BorderSize = 0;
            this.btnNotifications.Click += btnNotifications_Click;
            this.btnNotifications.Height = 50;
            this.btnNotifications.TextAlign = ContentAlignment.MiddleLeft;
            this.btnNotifications.Padding = new Padding(20, 0, 0, 0);
            
            // btnLogout
            this.btnLogout.BackColor = Color.FromArgb(220, 53, 69);
            this.btnLogout.ForeColor = Color.White;
            this.btnLogout.Dock = DockStyle.Bottom;
            this.btnLogout.Text = "Đăng xuất";
            this.btnLogout.FlatStyle = FlatStyle.Flat;
            this.btnLogout.Height = 50;
            this.btnLogout.Click += btnLogout_Click;
            
            // panelContent
            this.panelContent.Dock = DockStyle.Fill;
            
            // frmStudent
            this.ClientSize = new Size(1200, 800);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelMenu);
            this.Text = "Quản lý Thi & Điểm - Sinh viên";
            this.WindowState = FormWindowState.Maximized;
            this.panelMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
