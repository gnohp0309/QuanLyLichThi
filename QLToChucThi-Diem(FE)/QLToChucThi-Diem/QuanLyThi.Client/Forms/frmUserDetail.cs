using QuanLyThi.Client.Models;
using QuanLyThi.Client.Services;
using System.Linq;

namespace QuanLyThi.Client.Forms
{
    public partial class frmUserDetail : Form
    {
        private readonly ApiClient _apiClient;
        private readonly int? _userId;
        private TextBox txtUsername, txtPassword, txtFullName, txtStudentId, txtTeacherId, txtMajor, txtFaculty;
        private ComboBox cmbRole;

        public frmUserDetail(ApiClient apiClient, int? userId = null)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _userId = userId;
            InitializeControls();
            if (userId.HasValue)
                LoadUserData();
        }

        private void InitializeControls()
        {
            this.Text = _userId.HasValue ? "Sửa tài khoản" : "Thêm tài khoản mới";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterParent;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            int y = 20;
            panel.Controls.Add(CreateLabel("Tên đăng nhập:", 20, y));
            txtUsername = new TextBox { Location = new Point(150, y), Size = new Size(300, 30), Enabled = !_userId.HasValue };
            panel.Controls.Add(txtUsername);
            y += 40;

            panel.Controls.Add(CreateLabel("Mật khẩu:", 20, y));
            txtPassword = new TextBox { Location = new Point(150, y), Size = new Size(300, 30), PasswordChar = '*' };
            panel.Controls.Add(txtPassword);
            y += 40;

            panel.Controls.Add(CreateLabel("Họ và tên:", 20, y));
            txtFullName = new TextBox { Location = new Point(150, y), Size = new Size(300, 30) };
            panel.Controls.Add(txtFullName);
            y += 40;

            panel.Controls.Add(CreateLabel("Vai trò:", 20, y));
            cmbRole = new ComboBox { Location = new Point(150, y), Size = new Size(300, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new[] { "Admin", "Teacher", "Student" });
            panel.Controls.Add(cmbRole);
            y += 40;

            panel.Controls.Add(CreateLabel("Mã sinh viên:", 20, y));
            txtStudentId = new TextBox { Location = new Point(150, y), Size = new Size(300, 30) };
            panel.Controls.Add(txtStudentId);
            y += 40;

            panel.Controls.Add(CreateLabel("Mã giảng viên:", 20, y));
            txtTeacherId = new TextBox { Location = new Point(150, y), Size = new Size(300, 30) };
            panel.Controls.Add(txtTeacherId);
            y += 40;

            panel.Controls.Add(CreateLabel("Ngành:", 20, y));
            txtMajor = new TextBox { Location = new Point(150, y), Size = new Size(300, 30) };
            panel.Controls.Add(txtMajor);
            y += 40;

            panel.Controls.Add(CreateLabel("Khoa:", 20, y));
            txtFaculty = new TextBox { Location = new Point(150, y), Size = new Size(300, 30) };
            panel.Controls.Add(txtFaculty);

            var btnSave = new Button
            {
                Text = "Lưu",
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Size = new Size(100, 35),
                Location = new Point(250, y + 50),
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            panel.Controls.Add(btnSave);

            var btnCancel = new Button
            {
                Text = "Hủy",
                Size = new Size(100, 35),
                Location = new Point(360, y + 50),
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            panel.Controls.Add(btnCancel);
            this.Controls.Add(panel);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label { Text = text, Location = new Point(x, y + 5), Size = new Size(120, 20) };
        }

        private async void LoadUserData()
        {
            if (!_userId.HasValue) return;

            var user = await _apiClient.GetAsync<UserDto>($"Users/{_userId.Value}");
            if (user != null)
            {
                txtUsername.Text = user.Username;
                txtFullName.Text = user.FullName;
                cmbRole.SelectedItem = user.Role;
                txtStudentId.Text = user.StudentId ?? "";
                txtTeacherId.Text = user.TeacherId ?? "";
                txtMajor.Text = user.Major ?? "";
                txtFaculty.Text = user.Faculty ?? "";
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtFullName.Text) || cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_userId.HasValue)
                {
                    var data = new
                    {
                        username = txtUsername.Text,
                        password = string.IsNullOrEmpty(txtPassword.Text) ? null : txtPassword.Text,
                        fullName = txtFullName.Text,
                        role = cmbRole.SelectedItem.ToString(),
                        studentId = string.IsNullOrEmpty(txtStudentId.Text) ? null : txtStudentId.Text,
                        teacherId = string.IsNullOrEmpty(txtTeacherId.Text) ? null : txtTeacherId.Text,
                        major = string.IsNullOrEmpty(txtMajor.Text) ? null : txtMajor.Text,
                        faculty = string.IsNullOrEmpty(txtFaculty.Text) ? null : txtFaculty.Text
                    };
                    await _apiClient.PutAsync<object>($"Users/{_userId.Value}", data);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var data = new
                    {
                        username = txtUsername.Text,
                        password = txtPassword.Text,
                        fullName = txtFullName.Text,
                        role = cmbRole.SelectedItem.ToString(),
                        studentId = string.IsNullOrEmpty(txtStudentId.Text) ? null : txtStudentId.Text,
                        teacherId = string.IsNullOrEmpty(txtTeacherId.Text) ? null : txtTeacherId.Text,
                        major = string.IsNullOrEmpty(txtMajor.Text) ? null : txtMajor.Text,
                        faculty = string.IsNullOrEmpty(txtFaculty.Text) ? null : txtFaculty.Text
                    };
                    await _apiClient.PostAsync<object>("Users", data);
                }

                MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    partial class frmUserDetail
    {
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.ResumeLayout(false);
        }
    }
}
