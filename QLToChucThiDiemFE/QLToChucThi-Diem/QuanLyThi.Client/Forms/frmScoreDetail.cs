using QuanLyThi.Client.Models;
using QuanLyThi.Client.Services;
using System.Linq;

namespace QuanLyThi.Client.Forms
{
    public partial class frmScoreDetail : Form
    {
        private readonly ApiClient _apiClient;
        private readonly int? _scoreId;
        private readonly string? _sectionCode;
        private readonly int? _studentId;
        private TextBox txtAttendance, txtMidterm, txtFinal;
        private Label lblStudent, lblSubject;

        public frmScoreDetail(ApiClient apiClient, int? scoreId = null, string? sectionCode = null, int? studentId = null)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _scoreId = scoreId;
            _sectionCode = sectionCode;
            _studentId = studentId;
            InitializeControls();
            if (scoreId.HasValue || (sectionCode != null && studentId.HasValue))
                LoadScoreData();
        }

        private async void InitializeControls()
        {
            this.Text = _scoreId.HasValue ? "Sửa điểm" : "Nhập điểm";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            int y = 20;
            lblStudent = new Label { Location = new Point(20, y), Size = new Size(450, 25), AutoSize = true };
            panel.Controls.Add(lblStudent);
            y += 30;

            lblSubject = new Label { Location = new Point(20, y), Size = new Size(450, 25), AutoSize = true };
            panel.Controls.Add(lblSubject);
            y += 40;

            panel.Controls.Add(CreateLabel("Điểm chuyên cần (10%):", 20, y));
            txtAttendance = new TextBox { Location = new Point(200, y), Size = new Size(250, 30) };
            panel.Controls.Add(txtAttendance);
            y += 40;

            panel.Controls.Add(CreateLabel("Điểm giữa kỳ (30%):", 20, y));
            txtMidterm = new TextBox { Location = new Point(200, y), Size = new Size(250, 30) };
            panel.Controls.Add(txtMidterm);
            y += 40;

            panel.Controls.Add(CreateLabel("Điểm cuối kỳ (60%):", 20, y));
            txtFinal = new TextBox { Location = new Point(200, y), Size = new Size(250, 30) };
            panel.Controls.Add(txtFinal);

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
            return new Label { Text = text, Location = new Point(x, y + 5), Size = new Size(170, 20) };
        }

        private async void LoadScoreData()
        {
            try
            {
                var scores = await _apiClient.GetAsync<List<dynamic>>("Scores");
                dynamic? score = null;

                if (_scoreId.HasValue)
                {
                    score = scores?.FirstOrDefault(s => (int)s.id == _scoreId.Value);
                }
                else if (_sectionCode != null && _studentId.HasValue)
                {
                    score = scores?.FirstOrDefault(s => (string)s.sectionCode == _sectionCode && (int)s.studentId == _studentId.Value);
                }

                if (score != null)
                {
                    lblStudent.Text = $"Sinh viên: {score.studentName} ({score.studentCode})";
                    lblSubject.Text = $"Môn học: {score.subjectName}";
                    txtAttendance.Text = score.attendanceScore?.ToString() ?? "";
                    txtMidterm.Text = score.midtermScore?.ToString() ?? "";
                    txtFinal.Text = score.finalScore?.ToString() ?? "";
                }
            }
            catch { }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAttendance.Text, out decimal attendance) && !string.IsNullOrEmpty(txtAttendance.Text))
            {
                MessageBox.Show("Điểm chuyên cần không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!decimal.TryParse(txtMidterm.Text, out decimal midterm) && !string.IsNullOrEmpty(txtMidterm.Text))
            {
                MessageBox.Show("Điểm giữa kỳ không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!decimal.TryParse(txtFinal.Text, out decimal final) && !string.IsNullOrEmpty(txtFinal.Text))
            {
                MessageBox.Show("Điểm cuối kỳ không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (_scoreId.HasValue)
                {
                    // Get existing score to get sectionCode and studentId
                    var scores = await _apiClient.GetAsync<List<dynamic>>("Scores");
                    var score = scores?.FirstOrDefault(s => (int)s.id == _scoreId.Value);
                    if (score == null)
                    {
                        MessageBox.Show("Không tìm thấy điểm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var sectionCode = (string)score.sectionCode;
                    var studentId = (int)score.studentId;

                    var data = new
                    {
                        sectionCode = sectionCode,
                        scores = new[]
                        {
                            new
                            {
                                studentId = studentId,
                                attendanceScore = string.IsNullOrEmpty(txtAttendance.Text) ? (decimal?)null : attendance,
                                midtermScore = string.IsNullOrEmpty(txtMidterm.Text) ? (decimal?)null : midterm,
                                finalScore = string.IsNullOrEmpty(txtFinal.Text) ? (decimal?)null : final
                            }
                        }
                    };

                    await _apiClient.PostAsync<object>("Scores/bulk", data);
                }
                else if (_sectionCode != null && _studentId.HasValue)
                {
                    // Load student info
                    try
                    {
                        var users = await _apiClient.GetAsync<List<UserDto>>("Users");
                        var student = users?.FirstOrDefault(u => u.Id == _studentId.Value);
                        if (student != null)
                        {
                            lblStudent.Text = $"Sinh viên: {student.FullName} ({student.StudentId ?? student.Username})";
                        }

                        var sections = await _apiClient.GetAsync<List<dynamic>>("CourseSections");
                        var section = sections?.FirstOrDefault(s => (string)s.sectionCode == _sectionCode);
                        if (section != null)
                        {
                            lblSubject.Text = $"Môn học: {section.subjectName}";
                        }
                    }
                    catch { }

                    var data = new
                    {
                        sectionCode = _sectionCode,
                        scores = new[]
                        {
                            new
                            {
                                studentId = _studentId.Value,
                                attendanceScore = string.IsNullOrEmpty(txtAttendance.Text) ? (decimal?)null : attendance,
                                midtermScore = string.IsNullOrEmpty(txtMidterm.Text) ? (decimal?)null : midterm,
                                finalScore = string.IsNullOrEmpty(txtFinal.Text) ? (decimal?)null : final
                            }
                        }
                    };

                    await _apiClient.PostAsync<object>("Scores/bulk", data);
                }
                else
                {
                    MessageBox.Show("Thiếu thông tin để lưu điểm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Lưu điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    partial class frmScoreDetail
    {
        private void SetupCustomUI()
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
