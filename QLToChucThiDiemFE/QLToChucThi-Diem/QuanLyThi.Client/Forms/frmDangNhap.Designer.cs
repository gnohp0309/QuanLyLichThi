namespace QuanLyThi.Client
{
    partial class frmDangNhap
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            btnDangKy = new Button();
            btnQuenMatKhau = new Button();
            btnDangNhap = new Button();
            comboBox1 = new ComboBox();
            label5 = new Label();
            textBox2 = new TextBox();
            label4 = new Label();
            textBox1 = new TextBox();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(btnDangKy);
            panel1.Controls.Add(btnQuenMatKhau);
            panel1.Controls.Add(btnDangNhap);
            panel1.Controls.Add(comboBox1);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(textBox2);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(37, 43);
            panel1.Name = "panel1";
            panel1.Size = new Size(564, 463);
            panel1.TabIndex = 0;
            // 
            // btnDangKy
            // 
            btnDangKy.BackColor = Color.Blue;
            btnDangKy.FlatStyle = FlatStyle.Flat;
            btnDangKy.ForeColor = SystemColors.ButtonHighlight;
            btnDangKy.Location = new Point(295, 361);
            btnDangKy.Name = "btnDangKy";
            btnDangKy.Size = new Size(127, 43);
            btnDangKy.TabIndex = 11;
            btnDangKy.Text = "Đăng ký";
            btnDangKy.UseVisualStyleBackColor = false;
            btnDangKy.Click += btnDangKy_Click;
            // 
            // btnQuenMatKhau
            // 
            btnQuenMatKhau.BackColor = Color.Blue;
            btnQuenMatKhau.FlatStyle = FlatStyle.Flat;
            btnQuenMatKhau.ForeColor = SystemColors.ButtonHighlight;
            btnQuenMatKhau.Location = new Point(206, 410);
            btnQuenMatKhau.Name = "btnQuenMatKhau";
            btnQuenMatKhau.Size = new Size(127, 29);
            btnQuenMatKhau.TabIndex = 10;
            btnQuenMatKhau.Text = "Quên mật khẩu";
            btnQuenMatKhau.UseVisualStyleBackColor = false;
            btnQuenMatKhau.Click += btnQuenMatKhau_Click;
            // 
            // btnDangNhap
            // 
            btnDangNhap.BackColor = Color.Blue;
            btnDangNhap.FlatStyle = FlatStyle.Flat;
            btnDangNhap.ForeColor = SystemColors.ButtonHighlight;
            btnDangNhap.Location = new Point(124, 361);
            btnDangNhap.Name = "btnDangNhap";
            btnDangNhap.Size = new Size(127, 43);
            btnDangNhap.TabIndex = 8;
            btnDangNhap.Text = "Đăng nhập";
            btnDangNhap.UseVisualStyleBackColor = false;
            btnDangNhap.Click += btnDangNhap_Click;
            // 
            // comboBox1
            // 
            comboBox1.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(40, 314);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(474, 25);
            comboBox1.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(28, 291);
            label5.Name = "label5";
            label5.Size = new Size(55, 20);
            label5.TabIndex = 6;
            label5.Text = "Vai trò";
            // 
            // textBox2
            // 
            textBox2.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox2.Location = new Point(40, 246);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(474, 30);
            textBox2.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(28, 223);
            label4.Name = "label4";
            label4.Size = new Size(75, 20);
            label4.TabIndex = 4;
            label4.Text = "Mật khẩu";
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(40, 180);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(474, 30);
            textBox1.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(28, 157);
            label3.Name = "label3";
            label3.Size = new Size(112, 20);
            label3.TabIndex = 2;
            label3.Text = "Tên đăng nhập";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Blue;
            label2.Location = new Point(206, 117);
            label2.Name = "label2";
            label2.Size = new Size(122, 20);
            label2.TabIndex = 1;
            label2.Text = "OU UNIVERSITY";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(28, 25);
            label1.Name = "label1";
            label1.Size = new Size(511, 92);
            label1.TabIndex = 0;
            label1.Text = "HỆ THỐNG QUẢN LÝ TỔ CHỨC\r\n                THI - ĐIỂM";
            // 
            // frmDangNhap
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gainsboro;
            ClientSize = new Size(647, 557);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "frmDangNhap";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private TextBox textBox2;
        private Label label4;
        private TextBox textBox1;
        private Label label3;
        private Label label2;
        private Button btnDangKy;
        private Button btnQuenMatKhau;
        private Button btnDangNhap;
        private ComboBox comboBox1;
        private Label label5;

    }
}
