using QuanLyThi.Client.Services;

namespace QuanLyThi.Client.Forms
{
    public partial class frmExamScheduleDetail : Form
    {
        private readonly ApiClient _apiClient;
        private ComboBox cmbSection;
        private DateTimePicker dtpDate;
        private TextBox txtTime, txtRoom, txtLocation;
        private ComboBox cmbType;

        public frmExamScheduleDetail(ApiClient apiClient)
        {
            InitializeComponent();
            _apiClient = apiClient;
            InitializeControls();
            LoadSections();
        }

        private async void LoadSections()
        {
            var sections = await _apiClient.GetAsync<List<dynamic>>("CourseSections");
            if (sections != null)
            {
                cmbSection.Items.Clear();
                foreach (var s in sections)
                {
                    var code = GetProperty(s, "sectionCode");
                    var name = GetProperty(s, "subjectName");
                    cmbSection.Items.Add($"{code} - {name}");
                }
            }
        }

        private void InitializeControls()
        {
            this.Text = "Thêm lịch thi mới";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            int y = 20;
            panel.Controls.Add(CreateLabel("Lớp học phần:", 20, y));
            cmbSection = new ComboBox { Location = new Point(150, y), Size = new Size(300, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cmbSection);
            y += 40;

            panel.Controls.Add(CreateLabel("Ngày thi:", 20, y));
            dtpDate = new DateTimePicker { Location = new Point(150, y), Size = new Size(300, 30), Format = DateTimePickerFormat.Short };
            panel.Controls.Add(dtpDate);
            y += 40;

            panel.Controls.Add(CreateLabel("Giờ thi:", 20, y));
            txtTime = new TextBox { Location = new Point(150, y), Size = new Size(300, 30), PlaceholderText = "VD: 07:00 - 09:00" };
            panel.Controls.Add(txtTime);
            y += 40;

            panel.Controls.Add(CreateLabel("Phòng:", 20, y));
            txtRoom = new TextBox { Location = new Point(150, y), Size = new Size(300, 30) };
            panel.Controls.Add(txtRoom);
            y += 40;

            panel.Controls.Add(CreateLabel("Địa điểm:", 20, y));
            txtLocation = new TextBox { Location = new Point(150, y), Size = new Size(300, 30) };
            panel.Controls.Add(txtLocation);
            y += 40;

            panel.Controls.Add(CreateLabel("Loại:", 20, y));
            cmbType = new ComboBox { Location = new Point(150, y), Size = new Size(300, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new[] { "Exam", "Midterm", "Final" });
            cmbType.SelectedIndex = 0;
            panel.Controls.Add(cmbType);

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
            if (cmbSection.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtTime.Text) || string.IsNullOrWhiteSpace(txtRoom.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var sectionCode = cmbSection.Text.Split(" - ")[0];
                var data = new
                {
                    sectionCode = sectionCode,
                    examDate = dtpDate.Value,
                    examTime = txtTime.Text,
                    room = txtRoom.Text,
                    location = txtLocation.Text,
                    examType = cmbType.Text
                };

                await _apiClient.PostAsync<object>("ExamSchedules", data);
                MessageBox.Show("Tạo lịch thi thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    partial class frmExamScheduleDetail
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
