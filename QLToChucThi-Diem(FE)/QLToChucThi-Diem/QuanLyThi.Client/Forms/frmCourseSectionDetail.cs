using QuanLyThi.Client.Models;
using QuanLyThi.Client.Services;
using System.Linq;

namespace QuanLyThi.Client.Forms
{
    public partial class frmCourseSectionDetail : Form
    {
        private readonly ApiClient _apiClient;
        private TextBox txtCode, txtRoom;
        private ComboBox cmbSubject, cmbTeacher, cmbSemester;

        public frmCourseSectionDetail(ApiClient apiClient)
        {
            InitializeComponent();
            _apiClient = apiClient;
            InitializeControls();
            LoadData();
        }

        private async void LoadData()
        {
            var subjects = await _apiClient.GetAsync<List<dynamic>>("Subjects");
            if (subjects != null)
            {
                cmbSubject.Items.Clear();
                foreach (var s in subjects)
                {
                    var code = GetProperty(s, "subjectCode");
                    var name = GetProperty(s, "subjectName");
                    cmbSubject.Items.Add($"{code} - {name}");
                }
            }

            var teachers = await _apiClient.GetAsync<List<UserDto>>("Users");
            if (teachers != null)
            {
                cmbTeacher.Items.Clear();
                foreach (var t in teachers.Where(u => u.Role == "Teacher"))
                    cmbTeacher.Items.Add($"{t.TeacherId} - {t.FullName}");
            }

            cmbSemester.Items.AddRange(new[] { "HK1/2025", "HK2/2024-2025", "HK1/2024", "HK2/2025-2026" });
            cmbSemester.SelectedIndex = 0;
        }

        private void InitializeControls()
        {
            this.Text = "Thêm lớp học phần mới";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            int y = 20;
            panel.Controls.Add(CreateLabel("Mã lớp học phần:", 20, y));
            txtCode = new TextBox { Location = new Point(150, y), Size = new Size(300, 30) };
            panel.Controls.Add(txtCode);
            y += 40;

            panel.Controls.Add(CreateLabel("Môn học:", 20, y));
            cmbSubject = new ComboBox { Location = new Point(150, y), Size = new Size(300, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbSubject);
            y += 40;

            panel.Controls.Add(CreateLabel("Giảng viên:", 20, y));
            cmbTeacher = new ComboBox { Location = new Point(150, y), Size = new Size(300, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbTeacher);
            y += 40;

            panel.Controls.Add(CreateLabel("Học kỳ:", 20, y));
            cmbSemester = new ComboBox { Location = new Point(150, y), Size = new Size(300, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbSemester);
            y += 40;

            panel.Controls.Add(CreateLabel("Phòng học mặc định:", 20, y));
            txtRoom = new TextBox { Location = new Point(150, y), Size = new Size(300, 30) };
            panel.Controls.Add(txtRoom);

            var btnSave = new Button
            {
                Text = "Tạo lớp",
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
            return new Label { Text = text, Location = new Point(x, y + 5), Size = new Size(130, 20) };
        }

        private string GetProperty(dynamic obj, string propertyName)
        {
            try
            {
                if (obj is Newtonsoft.Json.Linq.JObject jobj)
                {
                    var val = jobj[propertyName] ?? jobj[char.ToUpper(propertyName[0]) + propertyName.Substring(1)];
                    return val?.ToString() ?? "";
                }
                var type = obj.GetType();
                var prop = type.GetProperty(propertyName) ?? type.GetProperty(char.ToUpper(propertyName[0]) + propertyName.Substring(1));
                return prop?.GetValue(obj)?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || cmbSubject.SelectedIndex == -1 || cmbTeacher.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var subjectCode = cmbSubject.Text.Split(" - ")[0];
                var teacherId = cmbTeacher.Text.Split(" - ")[0];
                var teachers = await _apiClient.GetAsync<List<UserDto>>("Users");
                var teacher = teachers?.FirstOrDefault(t => t.TeacherId == teacherId || t.Username == teacherId);

                var data = new
                {
                    sectionCode = txtCode.Text,
                    subjectCode = subjectCode,
                    teacherId = teacher?.Id ?? 0,
                    semester = cmbSemester.Text,
                    defaultRoom = txtRoom.Text
                };

                await _apiClient.PostAsync<object>("CourseSections", data);
                MessageBox.Show("Tạo lớp học phần thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    partial class frmCourseSectionDetail
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
