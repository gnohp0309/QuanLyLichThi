using QuanLyThi.Client.Services;

namespace QuanLyThi.Client.Forms
{
    public partial class frmSubjectDetail : Form
    {
        private readonly ApiClient _apiClient;
        private readonly string? _subjectCode;
        private TextBox txtCode, txtName, txtCredits;

        public frmSubjectDetail(ApiClient apiClient, string? subjectCode = null)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _subjectCode = subjectCode;
            InitializeControls();
            if (!string.IsNullOrEmpty(subjectCode))
                LoadSubjectData();
        }

        private void InitializeControls()
        {
            this.Text = _subjectCode == null ? "Thêm môn học mới" : "Sửa môn học";
            this.Size = new Size(500, 250);
            this.StartPosition = FormStartPosition.CenterParent;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            int y = 20;
            panel.Controls.Add(CreateLabel("Mã môn:", 20, y));
            txtCode = new TextBox { Location = new Point(150, y), Size = new Size(300, 30), Enabled = _subjectCode == null };
            panel.Controls.Add(txtCode);
            y += 40;

            panel.Controls.Add(CreateLabel("Tên môn học:", 20, y));
            txtName = new TextBox { Location = new Point(150, y), Size = new Size(300, 30) };
            panel.Controls.Add(txtName);
            y += 40;

            panel.Controls.Add(CreateLabel("Số tín chỉ:", 20, y));
            txtCredits = new TextBox { Location = new Point(150, y), Size = new Size(300, 30) };
            panel.Controls.Add(txtCredits);

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

        private async void LoadSubjectData()
        {
            if (string.IsNullOrEmpty(_subjectCode)) return;

            var subject = await _apiClient.GetAsync<dynamic>($"Subjects/{_subjectCode}");
            if (subject != null)
            {
                txtCode.Text = (string)subject.subjectCode;
                txtName.Text = (string)subject.subjectName;
                txtCredits.Text = ((int)subject.credits).ToString();
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtName.Text) || !int.TryParse(txtCredits.Text, out int credits))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var data = new
                {
                    subjectCode = txtCode.Text,
                    subjectName = txtName.Text,
                    credits = credits
                };

                if (_subjectCode == null)
                    await _apiClient.PostAsync<object>("Subjects", data);
                else
                    await _apiClient.PutAsync<object>($"Subjects/{_subjectCode}", data);

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

    partial class frmSubjectDetail
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
