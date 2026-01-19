using QuanLyThi.Client.Models;
using QuanLyThi.Client.Services;
using System.Linq;

namespace QuanLyThi.Client.Forms
{
    public partial class frmStudent : Form
    {
        private readonly UserDto _currentUser;
        private readonly ApiClient _apiClient;

        public frmStudent(UserDto user, ApiClient apiClient)
        {
            InitializeComponent();
            _currentUser = user;
            _apiClient = apiClient;
            LoadHome();
        }

        #region Home Dashboard
        private async void LoadHome()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = $"Xin chào, {_currentUser.FullName}",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(30, 30),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);

            var lblInfo = new Label
            {
                Text = $"MSSV: {_currentUser.StudentId} | Ngành: {_currentUser.Major}",
                Font = new Font("Segoe UI", 12F),
                Location = new Point(30, 70),
                AutoSize = true
            };
            panel.Controls.Add(lblInfo);

            // Quick action cards
            LoadQuickActionCards(panel);

            // Load stats
            await LoadStudentStats(panel);

            // Load grade table
            await LoadGradeTable(panel);

            panelContent.Controls.Add(panel);
        }

        private void LoadQuickActionCards(Panel parent)
        {
            int y = 120;
            
            // Course Registration card
            var card1 = new Panel
            {
                Size = new Size(250, 120),
                Location = new Point(30, y),
                BackColor = Color.FromArgb(212, 237, 218),
                BorderStyle = BorderStyle.FixedSingle
            };
            var lblCard1 = new Label
            {
                Text = "Đăng ký học phần",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            var lblCard1Desc = new Label
            {
                Text = "Chọn lớp học phần để đăng ký",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(10, 40),
                AutoSize = true,
                ForeColor = Color.Gray
            };
            card1.Controls.Add(lblCard1);
            card1.Controls.Add(lblCard1Desc);
            parent.Controls.Add(card1);

            // Schedule card
            var card2 = new Panel
            {
                Size = new Size(250, 120),
                Location = new Point(300, y),
                BackColor = Color.FromArgb(255, 243, 205),
                BorderStyle = BorderStyle.FixedSingle
            };
            var lblCard2 = new Label
            {
                Text = "Lịch học & Lịch thi",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            var lblCard2Desc = new Label
            {
                Text = "Xem lịch học, lịch thi của bạn",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(10, 40),
                AutoSize = true,
                ForeColor = Color.Gray
            };
            card2.Controls.Add(lblCard2);
            card2.Controls.Add(lblCard2Desc);
            parent.Controls.Add(card2);
        }

        private async Task LoadStudentStats(Panel parent)
        {
            try
            {
                var scores = await _apiClient.GetAsync<List<dynamic>>($"Scores?studentId={_currentUser.Id}");
                var publishedScores = scores?.Where(s => s.isPublished == true).ToList() ?? new List<dynamic>();

                decimal avgScore = 0;
                if (publishedScores.Any())
                {
                    avgScore = publishedScores.Select(s => (decimal?)s.totalScore ?? 0).Average();
                }

                string classification = "Chưa có";
                if (avgScore >= 9.0m) classification = "Xuất sắc";
                else if (avgScore >= 8.0m) classification = "Giỏi";
                else if (avgScore >= 7.0m) classification = "Khá";
                else if (avgScore >= 5.0m) classification = "Trung bình";

                int y = 260; // Move down after quick action cards
                CreateStatCard(parent, "GPA tích lũy", avgScore.ToString("F2"), "Thang điểm 4", Color.FromArgb(40, 167, 69), 30, y);
                CreateStatCard(parent, "Tín chỉ", $"{publishedScores.Count}/120", "Đã hoàn thành", Color.FromArgb(255, 193, 7), 250, y);
                CreateStatCard(parent, "Xếp loại", classification, "Học lực", Color.FromArgb(128, 0, 128), 470, y);
                CreateStatCard(parent, "Học kỳ này", publishedScores.Count.ToString(), "Môn học", Color.FromArgb(0, 102, 204), 690, y);
            }
            catch { }
        }

        private void CreateStatCard(Panel parent, string title, string value, string subtitle, Color color, int x, int y)
        {
            var card = new Panel
            {
                Size = new Size(200, 120),
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
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(10, 35),
                AutoSize = true
            };

            var lblSub = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(10, 70),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblSub);
            parent.Controls.Add(card);
        }

        private async Task LoadGradeTable(Panel parent)
        {
            var lblTitle = new Label
            {
                Text = "Bảng điểm học kỳ",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(30, 270),
                AutoSize = true
            };
            parent.Controls.Add(lblTitle);

            var cmbSemester = new ComboBox
            {
                Size = new Size(120, 30),
                Location = new Point(parent.Width - 150, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSemester.Items.Add("Tất cả");
            parent.Controls.Add(cmbSemester);

            var grid = new DataGridView
            {
                Location = new Point(30, 440),
                Size = new Size(parent.Width - 60, 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            try
            {
                var scores = await _apiClient.GetAsync<List<dynamic>>($"Scores?studentId={_currentUser.Id}");
                if (scores != null)
                {
                    grid.DataSource = scores.Where(s => s.isPublished == true).Select((s, i) => new
                    {
                        STT = i + 1,
                        SubjectCode = (string)s.sectionCode,
                        SubjectName = (string)s.subjectName,
                        Credits = "3",
                        AttendanceScore = s.attendanceScore?.ToString() ?? "",
                        MidtermScore = s.midtermScore?.ToString() ?? "",
                        FinalScore = s.finalScore?.ToString() ?? "",
                        TotalScore = s.totalScore?.ToString("F2") ?? "",
                        Grade = (string?)s.grade ?? "",
                        Status = "Đạt"
                    }).ToList();
                }
            }
            catch { }

            parent.Controls.Add(grid);

            // Buttons below table
            var btnCumulative = new Button
            {
                Text = "Bảng điểm tích lũy",
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Size = new Size(150, 35),
                Location = new Point(30, grid.Location.Y + grid.Height + 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                FlatStyle = FlatStyle.Flat
            };
            parent.Controls.Add(btnCumulative);

            var btnLookup = new Button
            {
                Text = "Tra cứu điểm",
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Size = new Size(150, 35),
                Location = new Point(190, grid.Location.Y + grid.Height + 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                FlatStyle = FlatStyle.Flat
            };
            parent.Controls.Add(btnLookup);
        }
        #endregion

        #region Course Registration
        private async void LoadCourseRegistration()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "Đăng ký học phần",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);

            var lblInfo = new Label
            {
                Text = $"Sinh viên: {_currentUser.FullName} - MSSV: {_currentUser.StudentId}",
                Font = new Font("Segoe UI", 12F),
                Location = new Point(20, 50),
                AutoSize = true
            };
            panel.Controls.Add(lblInfo);

            // Tabs
            var tabControl = new TabControl
            {
                Location = new Point(20, 90),
                Size = new Size(panel.Width - 40, panel.Height - 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            var tabAvailable = new TabPage("Lớp học phần");
            var tabRegistered = new TabPage("Đã đăng ký");
            tabControl.TabPages.Add(tabAvailable);
            tabControl.TabPages.Add(tabRegistered);

            await LoadAvailableClasses(tabAvailable);
            await LoadRegisteredClasses(tabRegistered);

            panel.Controls.Add(tabControl);
            panelContent.Controls.Add(panel);
        }

        private async Task LoadAvailableClasses(TabPage tab)
        {
            tab.Controls.Clear();

            var lblInfo = new Label
            {
                Text = "Chọn lớp học phần để đăng ký",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 10),
                AutoSize = true
            };
            tab.Controls.Add(lblInfo);

            var grid = new DataGridView
            {
                Location = new Point(20, 45),
                Size = new Size(tab.Width - 40, tab.Height - 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            var btnRegister = new Button
            {
                Text = "Đăng ký",
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Size = new Size(100, 35),
                Location = new Point(tab.Width - 120, tab.Height - 45),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat
            };

            btnRegister.Click += async (s, e) =>
            {
                if (grid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn lớp học phần!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var sectionCode = grid.SelectedRows[0].Cells["SectionCode"].Value.ToString() ?? "";
                try
                {
                    var data = new { studentId = _currentUser.Id, sectionCode = sectionCode };
                    var result = await _apiClient.PostAsync<object>("Enrollments", data);
                    MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadAvailableClasses(tab);
                    var parentTabControl = (TabControl)tab.Parent;
                    await LoadRegisteredClasses(parentTabControl.TabPages[1]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            try
            {
                var sections = await _apiClient.GetAsync<List<dynamic>>("CourseSections");
                var enrollments = await _apiClient.GetAsync<List<dynamic>>($"Enrollments?studentId={_currentUser.Id}");
                var enrolledCodes = enrollments?.Select(e => GetPropertyValue(e, "sectionCode")).ToList() ?? new List<string>();

                if (sections != null && sections.Any())
                {
                    var available = sections.Where(s => !enrolledCodes.Contains(GetPropertyValue(s, "sectionCode"))).ToList();
                    if (available.Any())
                    {
                        grid.DataSource = available.Select((s, i) => new
                        {
                            STT = i + 1,
                            SectionCode = GetPropertyValue(s, "sectionCode"),
                            SubjectName = GetPropertyValue(s, "subjectName"),
                            TeacherName = GetPropertyValue(s, "teacherName"),
                            Semester = GetPropertyValue(s, "semester"),
                            Room = GetPropertyValue(s, "defaultRoom", "")
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            tab.Controls.Add(btnRegister);
            tab.Controls.Add(grid);
        }

        private async Task LoadRegisteredClasses(TabPage tab)
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            try
            {
                var enrollments = await _apiClient.GetAsync<List<dynamic>>($"Enrollments?studentId={_currentUser.Id}");
                if (enrollments != null)
                {
                    grid.DataSource = enrollments.Select((e, i) => new
                    {
                        STT = i + 1,
                        SectionCode = GetPropertyValue(e, "sectionCode"),
                        SubjectName = GetPropertyValue(e, "subjectName"),
                        StudentId = _currentUser.StudentId
                    }).ToList();
                }
            }
            catch { }

            tab.Controls.Add(grid);
        }
        #endregion

        #region Scores
        private async void LoadScores()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "Bảng điểm của tôi",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);

            var lblInfo = new Label
            {
                Text = $"Sinh viên: {_currentUser.FullName} - MSSV: {_currentUser.StudentId}",
                Font = new Font("Segoe UI", 12F),
                Location = new Point(20, 50),
                AutoSize = true
            };
            panel.Controls.Add(lblInfo);

            var lblFormula = new Label
            {
                Text = "Công thức tính điểm: 10% Chuyên cần + 30% Giữa kỳ + 60% Cuối kỳ",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 80),
                AutoSize = true
            };
            panel.Controls.Add(lblFormula);

            var grid = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(panel.Width - 40, 350),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            try
            {
                var scores = await _apiClient.GetAsync<List<dynamic>>($"Scores?studentId={_currentUser.Id}");
                if (scores != null)
                {
                    var published = scores.Where(s => s.isPublished == true).ToList();
                    grid.DataSource = published.Select((s, i) => new
                    {
                        STT = i + 1,
                        SectionCode = (string)s.sectionCode,
                        SubjectName = (string)s.subjectName,
                        AttendanceScore = s.attendanceScore?.ToString("F2") ?? "",
                        MidtermScore = s.midtermScore?.ToString("F2") ?? "",
                        FinalScore = s.finalScore?.ToString("F2") ?? "",
                        TotalScore = s.totalScore?.ToString("F2") ?? "",
                        Classification = (string?)s.classification ?? "",
                        Result = "Đạt"
                    }).ToList();
                }
            }
            catch { }

            panel.Controls.Add(grid);

            // Summary Panel
            try
            {
                var scores = await _apiClient.GetAsync<List<dynamic>>($"Scores?studentId={_currentUser.Id}");
                var published = scores?.Where(s => s.isPublished == true).ToList() ?? new List<dynamic>();
                var avgScore = published.Any() ? published.Select(s => (decimal?)s.totalScore ?? 0).Average() : 0;
                var passedCount = published.Count(s => (decimal?)(s.totalScore ?? 0) >= 5.0m);

                string classification = "Chưa có";
                if (avgScore >= 9.0m) classification = "Xuất sắc";
                else if (avgScore >= 8.0m) classification = "Giỏi";
                else if (avgScore >= 7.0m) classification = "Khá";
                else if (avgScore >= 5.0m) classification = "Trung bình";
                else if (avgScore > 0) classification = "Yếu";

                var summaryPanel = new Panel
                {
                    Location = new Point(20, 480),
                    Size = new Size(panel.Width - 40, 80),
                    BackColor = Color.FromArgb(230, 240, 255),
                    BorderStyle = BorderStyle.FixedSingle,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

                summaryPanel.Controls.Add(new Label 
                { 
                    Text = $"Tổng số môn: {published.Count}", 
                    Location = new Point(20, 10), 
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                });
                summaryPanel.Controls.Add(new Label 
                { 
                    Text = $"Môn đạt: {passedCount}", 
                    Location = new Point(200, 10), 
                    AutoSize = true, 
                    ForeColor = Color.Green,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                });
                summaryPanel.Controls.Add(new Label 
                { 
                    Text = $"Điểm TB: {avgScore:F2}", 
                    Location = new Point(350, 10), 
                    AutoSize = true, 
                    ForeColor = Color.Purple,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                });
                summaryPanel.Controls.Add(new Label 
                { 
                    Text = $"Xếp loại: {classification}", 
                    Location = new Point(500, 10), 
                    AutoSize = true, 
                    ForeColor = Color.Red,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                });

                panel.Controls.Add(summaryPanel);
            }
            catch { }

            panelContent.Controls.Add(panel);
        }
        #endregion

        #region Exam Schedules
        private async void LoadExamSchedules()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "Lịch học & Lịch thi",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);

            var lblInfo = new Label
            {
                Text = $"Sinh viên: {_currentUser.FullName} - MSSV: {_currentUser.StudentId}",
                Font = new Font("Segoe UI", 12F),
                Location = new Point(20, 50),
                AutoSize = true
            };
            panel.Controls.Add(lblInfo);

            var tabControl = new TabControl
            {
                Location = new Point(20, 90),
                Size = new Size(panel.Width - 40, panel.Height - 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            var tabStudy = new TabPage("Lịch học");
            var tabExam = new TabPage("Lịch thi");
            tabControl.TabPages.Add(tabStudy);
            tabControl.TabPages.Add(tabExam);

            // Load study schedule (empty for now)
            var lblStudy = new Label
            {
                Text = "Danh sách lịch học học kỳ hiện tại",
                Location = new Point(20, 20),
                AutoSize = true
            };
            tabStudy.Controls.Add(lblStudy);

            // Load exam schedule
            await LoadStudentExamSchedule(tabExam);

            panel.Controls.Add(tabControl);
            panelContent.Controls.Add(panel);
        }

        private async Task LoadStudentExamSchedule(TabPage tab)
        {
            tab.Controls.Clear();

            var grid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(tab.Width - 40, tab.Height - 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            try
            {
                var schedules = await _apiClient.GetAsync<List<dynamic>>($"ExamSchedules?studentId={_currentUser.Id}");

                if (schedules != null && schedules.Any())
                {
                    grid.DataSource = schedules.Select((s, i) => new
                    {
                        STT = i + 1,
                        Môn_Học = GetPropertyValue(s, "subjectName", ""),
                        Lớp_HP = GetPropertyValue(s, "sectionCode", ""),
                        Ngày_Thi = GetExamDateString(s),
                        Giờ = GetPropertyValue(s, "examTime", ""),
                        Phòng = GetPropertyValue(s, "room", ""),
                        Địa_Điểm = GetPropertyValue(s, "location", ""),
                        Loại = GetPropertyValue(s, "examType", "")
                    }).ToList();
                    tab.Controls.Add(grid);
                }
                else
                {
                    var lblNoData = new Label
                    {
                        Text = "Chưa có lịch thi nào hoặc bạn chưa đăng ký môn học.",
                        Location = new Point(20, 60),
                        AutoSize = true,
                        ForeColor = Color.Red,
                        Font = new Font("Segoe UI", 11F, FontStyle.Italic)
                    };
                    tab.Controls.Add(lblNoData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải lịch thi: {ex.Message}");
            }

            tab.Controls.Add(grid);
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
                new { STT = 3, Title = "Thông báo mới", Content = "Vui lòng kiểm tra lịch thi và chuẩn bị đầy đủ", CreatedAt = "2 giờ trước" }
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
        private void btnRegistration_Click(object sender, EventArgs e) => LoadCourseRegistration();
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
