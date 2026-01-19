using QuanLyThi.Client.Models;
using QuanLyThi.Client.Services;
using System.Linq;

namespace QuanLyThi.Client.Forms
{
    public partial class frmTeacher : Form
    {
        private readonly UserDto _currentUser;
        private readonly ApiClient _apiClient;

        public frmTeacher(UserDto user, ApiClient apiClient)
        {
            InitializeComponent();
            _currentUser = user;
            _apiClient = apiClient;
            LoadHome();
        }
        private void BtnEnterScore_Click(object? sender, EventArgs e)
        {
            // Vì đang ở trang chủ, bấm nút này sẽ chuyển sang tab Quản lý điểm
            LoadScores();
        }

        #region Home Dashboard
        private async void LoadHome()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.FromArgb(245, 247, 250) };

            // Header banner
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(0, 102, 204)
            };
            var lblTitle = new Label
            {
                Text = $"Xin Chào, {_currentUser.FullName}",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 25),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblTitle);

            var lblInfo = new Label
            {
                Text = $"Mã GV: {_currentUser.TeacherId} | Khoa: {_currentUser.Faculty}",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                Location = new Point(30, 60),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblInfo);
            panel.Controls.Add(headerPanel);

            // Load stats
            await LoadTeacherStats(panel);

            // Load teaching classes
            await LoadTeachingClasses(panel);

            panelContent.Controls.Add(panel);
        }

        private async Task LoadTeacherStats(Panel parent)
        {
            try
            {
                var sections = await _apiClient.GetAsync<List<dynamic>>($"CourseSections?teacherId={_currentUser.Id}");
                int classCount = sections?.Count ?? 0;

                int y = 120;
                var card1 = CreateStatCard("Lớp giảng dạy", classCount.ToString(), "Học kỳ này", Color.FromArgb(0, 102, 204), 30, y);
                parent.Controls.Add(card1);

                var card2 = CreateStatCard("Tiết học tuần này", "12", "Tiết", Color.FromArgb(128, 0, 128), 250, y);
                parent.Controls.Add(card2);
            }
            catch { }
        }

        private Panel CreateStatCard(string title, string value, string subtitle, Color color, int x, int y)
        {
            var card = new Panel
            {
                Size = new Size(200, 150),
                Location = new Point(x, y),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 32F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(10, 35),
                AutoSize = true
            };

            var lblSub = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(10, 90),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblSub);
            return card;
        }

        private async Task LoadTeachingClasses(Panel parent)
        {
            var lblTitle = new Label
            {
                Text = "Lớp học đang giảng dạy",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(30, 300),
                AutoSize = true
            };
            parent.Controls.Add(lblTitle);

            var btnEnterScore = new Button
            {
                Text = "+ Nhập điểm",
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Size = new Size(120, 35),
                Location = new Point(parent.Width - 150, 295),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat
            };
            btnEnterScore.Click += BtnEnterScore_Click;
            parent.Controls.Add(btnEnterScore);

            var grid = new DataGridView
            {
                Location = new Point(30, 340),
                Size = new Size(parent.Width - 60, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            try
            {
                var sections = await _apiClient.GetAsync<List<dynamic>>($"CourseSections?teacherId={_currentUser.Id}");
                if (sections != null && sections.Any())
                {
                    grid.DataSource = sections.Select((s, i) => new
                    {
                        STT = i + 1,
                        ClassCode = GetPropertyValue(s, "sectionCode"),
                        ClassName = GetPropertyValue(s, "subjectName"),
                        Subject = GetPropertyValue(s, "subjectName"),
                        Enrollment = GetIntProperty(s, "enrollmentCount"),
                        Room = GetPropertyValue(s, "defaultRoom", ""),
                        SectionCode = GetPropertyValue(s, "sectionCode")
                    }).ToList();

                    if (grid.Columns["SectionCode"] != null)
                        grid.Columns["SectionCode"].Visible = false;
                }
                else
                {
                    var lblNoData = new Label
                    {
                        Text = "Bạn chưa được phân công lớp học phần nào",
                        Location = new Point(30, 370),
                        AutoSize = true,
                        ForeColor = Color.Gray,
                        Font = new Font("Segoe UI", 12F)
                    };
                    parent.Controls.Add(lblNoData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            parent.Controls.Add(grid);
        }
        #endregion

        #region Scores Management
        private ComboBox? _teacherClassCombo;
        private DataGridView? _teacherScoresGrid;
        private Button? _teacherBtnSearch;
        private string? _currentTeacherSectionCode;

        private async void LoadScores()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "Quản lý Điểm thi",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);

            var lblDesc = new Label
            {
                Text = "Nhập điểm, cập nhật và công bố điểm cho sinh viên (10% CC + 30% GK + 60% CK)",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 50),
                AutoSize = true
            };
            panel.Controls.Add(lblDesc);

            _teacherClassCombo = new ComboBox
            {
                Size = new Size(250, 30),
                Location = new Point(20, 80),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            try
            {
                var sections = await _apiClient.GetAsync<List<dynamic>>($"CourseSections?teacherId={_currentUser.Id}");
                if (sections != null && sections.Any())
                {
                    _teacherClassCombo.Items.Add("-- Chọn lớp --");
                    foreach (var s in sections)
                        _teacherClassCombo.Items.Add($"{GetPropertyValue(s, "sectionCode")} - {GetPropertyValue(s, "subjectName")}");
                    _teacherClassCombo.SelectedIndex = 0;
                    _teacherClassCombo.SelectedIndexChanged += async (s, e) =>
                    {
                        if (_teacherClassCombo.SelectedIndex > 0)
                        {
                            _currentTeacherSectionCode = _teacherClassCombo.Text;
                            await LoadScoresForClass(panel, _teacherClassCombo.Text);
                        }
                    };
                }
            }
            catch { }
            panel.Controls.Add(_teacherClassCombo);

            var btnEnterScore = new Button
            {
                Text = "Nhập/Sửa điểm",
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.White,
                Size = new Size(130, 35),
                Location = new Point(290, 78),
                FlatStyle = FlatStyle.Flat
            };
            btnEnterScore.Click += BtnTeacherEnterScore_Click;
            panel.Controls.Add(btnEnterScore);

            var btnPublish = new Button
            {
                Text = "Công bố điểm",
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Size = new Size(130, 35),
                Location = new Point(430, 78),
                FlatStyle = FlatStyle.Flat
            };
            btnPublish.Click += BtnPublishScore_Click;
            panel.Controls.Add(btnPublish);

            _teacherBtnSearch = new Button
            {
                Text = "Làm mới",
                Size = new Size(100, 35),
                Location = new Point(570, 78),
                FlatStyle = FlatStyle.Flat
            };
            _teacherBtnSearch.Click += async (s, e) =>
            {
                if (_teacherClassCombo.SelectedIndex > 0)
                    await LoadScoresForClass(panel, _teacherClassCombo.Text);
            };
            panel.Controls.Add(_teacherBtnSearch);

            panelContent.Controls.Add(panel);
        }

        private async Task LoadScoresForClass(Panel panel, string classText)
        {
            if (string.IsNullOrEmpty(classText) || classText == "-- Chọn lớp --")
                return;

            _currentTeacherSectionCode = classText.Split(" - ")[0];

            if (_teacherScoresGrid != null)
                panel.Controls.Remove(_teacherScoresGrid);

            _teacherScoresGrid = new DataGridView
            {
                Location = new Point(20, 130),
                Size = new Size(panel.Width - 40, panel.Height - 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            try
            {
                var scores = await _apiClient.GetAsync<List<dynamic>>($"Scores?sectionCode={Uri.EscapeDataString(_currentTeacherSectionCode)}");
                
                // Nếu chưa có điểm, lấy danh sách sinh viên đã đăng ký
                if (scores == null || !scores.Any())
                {
                    var enrollments = await _apiClient.GetAsync<List<dynamic>>($"Enrollments?sectionCode={Uri.EscapeDataString(_currentTeacherSectionCode)}");
                    if (enrollments != null && enrollments.Any())
                    {
                        _teacherScoresGrid.DataSource = enrollments.Select((e, i) => new
                        {
                            STT = i + 1,
                            StudentCode = (string?)(e.studentCode ?? ""),
                            StudentName = (string)e.studentName,
                            AttendanceScore = " ",
                            MidtermScore = " ",
                            FinalScore = " ",
                            TotalScore = " ",
                            Classification = "Chưa nhập",
                            IsPublished = false,
                            StudentId = (int)e.studentId,
                            Id = 0,
                            SectionCode = _currentTeacherSectionCode
                        }).ToList();
                        
                        if (_teacherScoresGrid.Columns["StudentId"] != null)
                            _teacherScoresGrid.Columns["StudentId"].Visible = false;
                        if (_teacherScoresGrid.Columns["SectionCode"] != null)
                            _teacherScoresGrid.Columns["SectionCode"].Visible = false;
                    }
                }
                else
                {
                    _teacherScoresGrid.DataSource = scores.Select((s, i) => new
                    {
                        STT = i + 1,
                        StudentCode = (string?)s.studentCode ,
                        StudentName = (string)s.studentName,
                        AttendanceScore = s.attendanceScore?.ToString() ,
                        MidtermScore = s.midtermScore?.ToString() ,
                        FinalScore = s.finalScore?.ToString() ,
                        TotalScore = s.totalScore?.ToString() ,
                        Classification = (string?)s.classification,
                        IsPublished = (bool)(s.isPublished ?? false),
                        StudentId = (int)s.studentId,
                        SectionCode = (string)s.sectionCode,
                        Id = (int)s.id
                    }).ToList();

                    if (_teacherScoresGrid.Columns["StudentId"] != null)
                        _teacherScoresGrid.Columns["StudentId"].Visible = false;
                    if (_teacherScoresGrid.Columns["SectionCode"] != null)
                        _teacherScoresGrid.Columns["SectionCode"].Visible = false;
                    if (_teacherScoresGrid.Columns["Id"] != null)
                        _teacherScoresGrid.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải điểm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            panel.Controls.Add(_teacherScoresGrid);
        }

        private async void BtnTeacherEnterScore_Click(object? sender, EventArgs e)
        {
            if (_teacherClassCombo?.SelectedIndex == null || _teacherClassCombo.SelectedIndex <= 0)
            {
                MessageBox.Show("Vui lòng chọn lớp học phần trước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_teacherScoresGrid?.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần nhập điểm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _teacherScoresGrid.SelectedRows[0];
            int? scoreId = null;
            if (row.Cells["Id"]?.Value != null)
                scoreId = (int)row.Cells["Id"].Value;

            var studentId = (int)row.Cells["StudentId"].Value;
            var sectionCode = _currentTeacherSectionCode ?? (string)row.Cells["SectionCode"].Value;

            var form = new frmScoreDetail(_apiClient, scoreId, sectionCode, studentId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (_teacherBtnSearch != null)
                    _teacherBtnSearch.PerformClick();
                else
                    LoadScores();
            }
        }

        private async void BtnPublishScore_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentTeacherSectionCode) || _teacherClassCombo?.SelectedIndex <= 0)
            {
                MessageBox.Show("Vui lòng chọn lớp học phần!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn công bố điểm cho lớp {_currentTeacherSectionCode}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await _apiClient.PostAsync<object>($"Scores/publish/{Uri.EscapeDataString(_currentTeacherSectionCode)}", null);
                    MessageBox.Show("Công bố điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (_teacherBtnSearch != null)
                        _teacherBtnSearch.PerformClick();
                    else
                        LoadScores();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Exam Schedules
        private async void LoadExamSchedules()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "Lịch thi",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);

            var grid = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(panel.Width - 40, panel.Height - 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            try
            {
                var sections = await _apiClient.GetAsync<List<dynamic>>($"CourseSections?teacherId={_currentUser.Id}");
                var allSchedules = new List<dynamic>();

                if (sections != null && sections.Any())
                {
                    foreach (var section in sections)
                    {
                        var sectionCode = (string)section.sectionCode;
                        var schedules = await _apiClient.GetAsync<List<dynamic>>($"ExamSchedules?sectionCode={Uri.EscapeDataString(sectionCode)}");
                        if (schedules != null && schedules.Any())
                            allSchedules.AddRange(schedules);
                    }
                }

                if (allSchedules.Any())
                {
                    grid.DataSource = allSchedules.Select((s, i) => new
                    {
                        STT = i + 1,
                        SectionCode = GetPropertyValue(s, "sectionCode"),
                        SubjectName = GetPropertyValue(s, "subjectName"),
                        ExamDate = GetExamDateString(s),
                        ExamTime = GetPropertyValue(s, "examTime", ""),
                        Room = GetPropertyValue(s, "room", ""),
                        Location = GetPropertyValue(s, "location", "")
                    }).ToList();
                }
                else
                {
                    var lblNoData = new Label
                    {
                        Text = "Chưa có lịch thi cho các lớp học phần đang giảng dạy",
                        Location = new Point(20, 100),
                        AutoSize = true,
                        ForeColor = Color.Gray,
                        Font = new Font("Segoe UI", 12F)
                    };
                    panel.Controls.Add(lblNoData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải lịch thi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            panel.Controls.Add(grid);
            panelContent.Controls.Add(panel);
        }
        #endregion

        #region Notifications
        private void LoadNotifications()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "Thông báo",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);

            var grid = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(panel.Width - 40, panel.Height - 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            // Mock data for notifications
            grid.DataSource = new[]
            {
                new { STT = 1, Title = "Thông báo về quy chế thi cuối kỳ HK1", Content = "Nhắc nhở sinh viên chuẩn bị tốt cho kỳ thi", CreatedAt = "1 ngày trước" },
                new { STT = 2, Title = "Cập nhật lịch thi cuối kỳ", Content = "Môn Cơ sở dữ liệu - IT302", CreatedAt = "5 giờ trước" },
                new { STT = 3, Title = "Thông báo mới", Content = "Vui lòng hoàn thành nhập điểm trước ngày công bố", CreatedAt = "2 giờ trước" }
            };

            panel.Controls.Add(grid);
            panelContent.Controls.Add(panel);
        }

        private string GetPropertyValue(dynamic obj, string propertyName, string defaultValue = "")
        {
            try
            {
                if (obj is Newtonsoft.Json.Linq.JObject jobj)
                {
                    var val = jobj[propertyName] ?? jobj[char.ToUpper(propertyName[0]) + propertyName.Substring(1)];
                    return val?.ToString() ?? defaultValue;
                }
                var type = obj.GetType();
                var prop = type.GetProperty(propertyName) ?? type.GetProperty(char.ToUpper(propertyName[0]) + propertyName.Substring(1));
                return prop?.GetValue(obj)?.ToString() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private int GetIntProperty(dynamic obj, string propertyName)
        {
            try
            {
                if (obj is Newtonsoft.Json.Linq.JObject jobj)
                {
                    var val = jobj[propertyName] ?? jobj[char.ToUpper(propertyName[0]) + propertyName.Substring(1)];
                    return val != null ? Convert.ToInt32(val) : 0;
                }
                var type = obj.GetType();
                var prop = type.GetProperty(propertyName) ?? type.GetProperty(char.ToUpper(propertyName[0]) + propertyName.Substring(1));
                if (prop != null)
                {
                    var value = prop.GetValue(obj);
                    return value != null ? Convert.ToInt32(value) : 0;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private string GetExamDateString(dynamic obj)
        {
            try
            {
                if (obj is Newtonsoft.Json.Linq.JObject jobj)
                {
                    var val = jobj["examDate"] ?? jobj["ExamDate"];
                    if (val != null && DateTime.TryParse(val.ToString(), out DateTime parsedDate))
                        return parsedDate.ToString("dd/MM/yyyy");
                }
                var type = obj.GetType();
                var prop = type.GetProperty("examDate") ?? type.GetProperty("ExamDate");
                if (prop != null)
                {
                    var value = prop.GetValue(obj);
                    if (value is DateTime dt)
                        return dt.ToString("dd/MM/yyyy");
                    if (DateTime.TryParse(value?.ToString(), out DateTime parsedDate))
                        return parsedDate.ToString("dd/MM/yyyy");
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
        #endregion

        // Event handlers
        private void btnHome_Click(object sender, EventArgs e) => LoadHome();
        private void btnScores_Click(object sender, EventArgs e) => LoadScores();
        private void btnExamSchedules_Click(object sender, EventArgs e) => LoadExamSchedules();
        private void btnNotifications_Click(object sender, EventArgs e) => LoadNotifications();
        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _apiClient.ClearToken();
                this.Close();
            }
        }
    }
}
