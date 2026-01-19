using QuanLyThi.Client.Services;
using System.Linq;

namespace QuanLyThi.Client.Forms
{
    public partial class frmSelectStudent : Form
    {
        private readonly ApiClient _apiClient;
        private readonly string _sectionCode;
        private DataGridView grid;
        public int? SelectedStudentId { get; private set; }

        public frmSelectStudent(ApiClient apiClient, string sectionCode, List<dynamic> enrollments)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _sectionCode = sectionCode;
            InitializeControls(enrollments);
        }

        private void InitializeControls(List<dynamic> enrollments)
        {
            this.Text = "Chọn sinh viên để nhập điểm";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = $"Chọn sinh viên trong lớp {_sectionCode}",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);

            grid = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(panel.Width - 40, panel.Height - 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            if (enrollments != null)
            {
                grid.DataSource = enrollments.Select((e, i) => new
                {
                    STT = i + 1,
                    StudentCode = (string?)(e.studentCode ?? ""),
                    StudentName = (string)e.studentName,
                    StudentId = (int)e.studentId
                }).ToList();

                grid.Columns["StudentId"].Visible = false;
            }

            panel.Controls.Add(grid);

            var btnSelect = new Button
            {
                Text = "Chọn",
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Size = new Size(100, 35),
                Location = new Point(panel.Width - 240, panel.Height - 70),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat
            };
            btnSelect.Click += (s, e) =>
            {
                if (grid.SelectedRows.Count > 0)
                {
                    SelectedStudentId = (int)grid.SelectedRows[0].Cells["StudentId"].Value;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            panel.Controls.Add(btnSelect);

            var btnCancel = new Button
            {
                Text = "Hủy",
                Size = new Size(100, 35),
                Location = new Point(panel.Width - 130, panel.Height - 70),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            panel.Controls.Add(btnCancel);

            this.Controls.Add(panel);
        }
    }

    partial class frmSelectStudent
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
