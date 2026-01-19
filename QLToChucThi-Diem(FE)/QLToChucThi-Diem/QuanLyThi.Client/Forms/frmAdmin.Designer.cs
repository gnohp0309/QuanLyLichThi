namespace QuanLyThi.Client.Forms
{
    partial class frmAdmin
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelMenu;
        private Panel panelContent;
        private Button btnHome;
        private Button btnUsers;
        private Button btnSubjects;
        private Button btnCourseSections;
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
            panelMenu = new Panel();
            btnLogout = new Button();
            btnNotifications = new Button();
            btnExamSchedules = new Button();
            btnScores = new Button();
            btnCourseSections = new Button();
            btnSubjects = new Button();
            btnUsers = new Button();
            btnHome = new Button();
            lblWelcome = new Label();
            panelContent = new Panel();
            panelMenu.SuspendLayout();
            SuspendLayout();
            // 
            // panelMenu
            // 
            panelMenu.BackColor = Color.FromArgb(240, 240, 240);
            panelMenu.Controls.Add(btnLogout);
            panelMenu.Controls.Add(btnNotifications);
            panelMenu.Controls.Add(btnExamSchedules);
            panelMenu.Controls.Add(btnScores);
            panelMenu.Controls.Add(btnCourseSections);
            panelMenu.Controls.Add(btnSubjects);
            panelMenu.Controls.Add(btnUsers);
            panelMenu.Controls.Add(btnHome);
            panelMenu.Controls.Add(lblWelcome);
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Location = new Point(0, 0);
            panelMenu.Name = "panelMenu";
            panelMenu.Size = new Size(200, 800);
            panelMenu.TabIndex = 0;
            // 
            // btnLogout
            // 
            btnLogout.BackColor = Color.FromArgb(220, 53, 69);
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.ForeColor = Color.White;
            btnLogout.Location = new Point(0, 750);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(200, 50);
            btnLogout.TabIndex = 0;
            btnLogout.Text = "Đăng xuất";
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;
            // 
            // btnNotifications
            // 
            btnNotifications.Dock = DockStyle.Top;
            btnNotifications.FlatAppearance.BorderSize = 0;
            btnNotifications.FlatStyle = FlatStyle.Flat;
            btnNotifications.Location = new Point(0, 350);
            btnNotifications.Name = "btnNotifications";
            btnNotifications.Padding = new Padding(20, 0, 0, 0);
            btnNotifications.Size = new Size(200, 50);
            btnNotifications.TabIndex = 1;
            btnNotifications.Text = "Thông báo";
            btnNotifications.TextAlign = ContentAlignment.MiddleLeft;
            btnNotifications.Click += btnNotifications_Click;
            // 
            // btnExamSchedules
            // 
            btnExamSchedules.Dock = DockStyle.Top;
            btnExamSchedules.FlatAppearance.BorderSize = 0;
            btnExamSchedules.FlatStyle = FlatStyle.Flat;
            btnExamSchedules.Location = new Point(0, 300);
            btnExamSchedules.Name = "btnExamSchedules";
            btnExamSchedules.Padding = new Padding(20, 0, 0, 0);
            btnExamSchedules.Size = new Size(200, 50);
            btnExamSchedules.TabIndex = 2;
            btnExamSchedules.Text = "Lịch thi";
            btnExamSchedules.TextAlign = ContentAlignment.MiddleLeft;
            btnExamSchedules.Click += btnExamSchedules_Click;
            // 
            // btnScores
            // 
            btnScores.Dock = DockStyle.Top;
            btnScores.FlatAppearance.BorderSize = 0;
            btnScores.FlatStyle = FlatStyle.Flat;
            btnScores.Location = new Point(0, 250);
            btnScores.Name = "btnScores";
            btnScores.Padding = new Padding(20, 0, 0, 0);
            btnScores.Size = new Size(200, 50);
            btnScores.TabIndex = 3;
            btnScores.Text = "Điểm thi";
            btnScores.TextAlign = ContentAlignment.MiddleLeft;
            btnScores.Click += btnScores_Click;
            // 
            // btnCourseSections
            // 
            btnCourseSections.Dock = DockStyle.Top;
            btnCourseSections.FlatAppearance.BorderSize = 0;
            btnCourseSections.FlatStyle = FlatStyle.Flat;
            btnCourseSections.Location = new Point(0, 200);
            btnCourseSections.Name = "btnCourseSections";
            btnCourseSections.Padding = new Padding(20, 0, 0, 0);
            btnCourseSections.Size = new Size(200, 50);
            btnCourseSections.TabIndex = 4;
            btnCourseSections.Text = "Lớp học phần";
            btnCourseSections.TextAlign = ContentAlignment.MiddleLeft;
            btnCourseSections.Click += btnCourseSections_Click;
            // 
            // btnSubjects
            // 
            btnSubjects.Dock = DockStyle.Top;
            btnSubjects.FlatAppearance.BorderSize = 0;
            btnSubjects.FlatStyle = FlatStyle.Flat;
            btnSubjects.Location = new Point(0, 150);
            btnSubjects.Name = "btnSubjects";
            btnSubjects.Padding = new Padding(20, 0, 0, 0);
            btnSubjects.Size = new Size(200, 50);
            btnSubjects.TabIndex = 5;
            btnSubjects.Text = "Môn học";
            btnSubjects.TextAlign = ContentAlignment.MiddleLeft;
            btnSubjects.Click += btnSubjects_Click;
            // 
            // btnUsers
            // 
            btnUsers.Dock = DockStyle.Top;
            btnUsers.FlatAppearance.BorderSize = 0;
            btnUsers.FlatStyle = FlatStyle.Flat;
            btnUsers.Location = new Point(0, 100);
            btnUsers.Name = "btnUsers";
            btnUsers.Padding = new Padding(20, 0, 0, 0);
            btnUsers.Size = new Size(200, 50);
            btnUsers.TabIndex = 6;
            btnUsers.Text = "Tài khoản";
            btnUsers.TextAlign = ContentAlignment.MiddleLeft;
            btnUsers.Click += btnUsers_Click;
            // 
            // btnHome
            // 
            btnHome.Dock = DockStyle.Top;
            btnHome.FlatAppearance.BorderSize = 0;
            btnHome.FlatStyle = FlatStyle.Flat;
            btnHome.Location = new Point(0, 50);
            btnHome.Name = "btnHome";
            btnHome.Padding = new Padding(20, 0, 0, 0);
            btnHome.Size = new Size(200, 50);
            btnHome.TabIndex = 7;
            btnHome.Text = "Trang chủ";
            btnHome.TextAlign = ContentAlignment.MiddleLeft;
            btnHome.Click += btnHome_Click;
            // 
            // lblWelcome
            // 
            lblWelcome.BackColor = Color.FromArgb(0, 102, 204);
            lblWelcome.Dock = DockStyle.Top;
            lblWelcome.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Location = new Point(0, 0);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Padding = new Padding(5);
            lblWelcome.Size = new Size(200, 50);
            lblWelcome.TabIndex = 8;
            lblWelcome.Text = "QUẢN LÝ THI & ĐIỂM";
            lblWelcome.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelContent
            // 
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(200, 0);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(1000, 800);
            panelContent.TabIndex = 1;
            // 
            // frmAdmin
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 800);
            Controls.Add(panelContent);
            Controls.Add(panelMenu);
            Name = "frmAdmin";
            Text = "Quản lý Thi & Điểm - Admin";
            WindowState = FormWindowState.Maximized;
            panelMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

    }
}
