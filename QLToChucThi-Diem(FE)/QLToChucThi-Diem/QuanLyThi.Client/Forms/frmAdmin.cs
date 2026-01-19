using QuanLyThi.Client.Models;
using QuanLyThi.Client.Services;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

namespace QuanLyThi.Client.Forms
{

    public partial class frmAdmin : Form
    {
        private readonly UserDto _currentUser;
        private readonly ApiClient _apiClient;
        private Button? btnSearchScores;
        private ComboBox? cmbClassScore;
        private DataGridView? _sectionsGrid;
        private DataGridView? _schedulesGrid;// <--- Bạn đang thiếu dòng này
        public frmAdmin(UserDto user, ApiClient apiClient)
        {
            InitializeComponent();
            _currentUser = user;

            // --- THÊM ĐOẠN NÀY ---
            // Nếu bên ngoài truyền vào null thì tự tạo mới
            if (apiClient == null)
            {
                _apiClient = new ApiClient();
            }
            else
            {
                _apiClient = apiClient;
            }
            // ---------------------

            LoadHome();
        }

        #region Home Dashboard
        private void LoadHome()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.FromArgb(245, 247, 250) };

            // Header banner
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(0, 102, 204)
            };
            var lblTitle = new Label
            {
                Text = $"Xin chào, {_currentUser.FullName}",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 20),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblTitle);

            var lblSub = new Label
            {
                Text = "Bảng điều khiển quản trị hệ thống",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                Location = new Point(30, 50),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblSub);
            panel.Controls.Add(headerPanel);

            // Stats cards
            LoadDashboardStats(panel);

            // Recent activities section
            LoadRecentActivities(panel);

            panelContent.Controls.Add(panel);
        }

        private void LoadRecentActivities(Panel parent)
        {
            var activitiesPanel = new Panel
            {
                Location = new Point(30, 260),
                Size = new Size(600, 300),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblActivities = new Label
            {
                Text = "Hoạt động gần đây",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };
            activitiesPanel.Controls.Add(lblActivities);

            var activities = new[]
            {
                new { Action = "Admin đã thêm tài khoản mới", Detail = "Tài khoản: gv2 - Nguyễn Thị C", Time = "2 giờ trước" },
                new { Action = "Cập nhật lịch thi cuối kỳ", Detail = "Môn Cơ sở dữ liệu - IT302", Time = "5 giờ trước" },
                new { Action = "Đăng thông báo mới", Detail = "Thông báo về quy chế thi cuối kỳ HK1", Time = "1 ngày trước" }
            };

            int y = 50;
            foreach (var activity in activities)
            {
                var lblAction = new Label
                {
                    Text = activity.Action,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Location = new Point(20, y),
                    AutoSize = true
                };
                activitiesPanel.Controls.Add(lblAction);
                y += 25;

                var lblDetail = new Label
                {
                    Text = activity.Detail,
                    Font = new Font("Segoe UI", 9F),
                    Location = new Point(40, y),
                    AutoSize = true,
                    ForeColor = Color.Gray
                };
                activitiesPanel.Controls.Add(lblDetail);
                y += 20;

                var lblTime = new Label
                {
                    Text = activity.Time,
                    Font = new Font("Segoe UI", 8F),
                    Location = new Point(40, y),
                    AutoSize = true,
                    ForeColor = Color.Gray
                };
                activitiesPanel.Controls.Add(lblTime);
                y += 35;
            }

            parent.Controls.Add(activitiesPanel);

            // System notice
            var systemPanel = new Panel
            {
                Location = new Point(650, 260),
                Size = new Size(400, 150),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblSystem = new Label
            {
                Text = "Hệ thống",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };
            systemPanel.Controls.Add(lblSystem);

            var noticePanel = new Panel
            {
                Location = new Point(20, 45),
                Size = new Size(360, 80),
                BackColor = Color.FromArgb(255, 243, 205),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblNotice = new Label
            {
                Text = "Lưu ý",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 152, 0),
                Location = new Point(30, 10),
                AutoSize = true
            };
            noticePanel.Controls.Add(lblNotice);

            var lblNoticeText = new Label
            {
                Text = "Sao lưu dữ liệu định kỳ để đảm bảo an toàn",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(30, 35),
                AutoSize = true
            };
            noticePanel.Controls.Add(lblNoticeText);
            systemPanel.Controls.Add(noticePanel);

            parent.Controls.Add(systemPanel);
        }

        private async void LoadDashboardStats(Panel parent)
        {
            try
            {
                var users = await _apiClient.GetAsync<List<UserDto>>("Users");
                var subjects = await _apiClient.GetAsync<List<dynamic>>("Subjects");
                var sections = await _apiClient.GetAsync<List<dynamic>>("CourseSections");

                int userCount = users?.Count ?? 0;
                int subjectCount = subjects?.Count ?? 0;
                int sectionCount = sections?.Count ?? 0;

                int y = 120;
                CreateStatCard(parent, "Tổng số tài khoản", userCount.ToString(), Color.FromArgb(0, 102, 204), 30, y);
                CreateStatCard(parent, "Tổng số môn học", subjectCount.ToString(), Color.FromArgb(40, 167, 69), 250, y);
                CreateStatCard(parent, "Tổng số lớp học phần", sectionCount.ToString(), Color.FromArgb(255, 193, 7), 470, y);
            }
            catch { }
        }

        private void CreateStatCard(Panel parent, string title, string value, Color color, int x, int y)
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
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(10, 40),
                AutoSize = true
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            parent.Controls.Add(card);
        }
        #endregion

        #region Users Management
        private async void LoadUsers()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "Quản lý tài khoản hệ thống",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);

            // Buttons
            var btnAdd = new Button
            {
                Text = "Thêm mới",
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Size = new Size(100, 35),
                Location = new Point(20, 60),
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.Click += BtnAddUser_Click;
            panel.Controls.Add(btnAdd);

            var btnEdit = new Button
            {
                Text = "Sửa",
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.White,
                Size = new Size(100, 35),
                Location = new Point(130, 60),
                FlatStyle = FlatStyle.Flat
            };
            btnEdit.Click += BtnEditUser_Click;
            panel.Controls.Add(btnEdit);

            var btnDelete = new Button
            {
                Text = "Xóa",
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Size = new Size(100, 35),
                Location = new Point(240, 60),
                FlatStyle = FlatStyle.Flat
            };
            btnDelete.Click += BtnDeleteUser_Click;
            panel.Controls.Add(btnDelete);

            var txtSearch = new TextBox
            {
                Size = new Size(300, 30),
                Location = new Point(360, 63),
                PlaceholderText = "Tìm kiếm..."
            };
            panel.Controls.Add(txtSearch);

            var btnSearch = new Button
            {
                Text = "Tìm kiếm",
                Size = new Size(100, 35),
                Location = new Point(670, 60),
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.Click += (s, e) => LoadUsersTable(panel, txtSearch.Text);
            panel.Controls.Add(btnSearch);

            // Data Grid
            await LoadUsersTable(panel, "");
            panelContent.Controls.Add(panel);
        }

        private DataGridView? _usersGrid;
        private async Task LoadUsersTable(Panel panel, string search)
        {
            if (_usersGrid != null)
                panel.Controls.Remove(_usersGrid);

            _usersGrid = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(panel.Width - 40, panel.Height - 130),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            try
            {
                var users = await _apiClient.GetAsync<List<UserDto>>("Users");
                if (users != null)
                {
                    var filtered = string.IsNullOrEmpty(search) 
                        ? users 
                        : users.Where(u => u.Username.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                    _usersGrid.DataSource = filtered.Select((u, i) => new
                    {
                        STT = i + 1,
                        Username = u.Username,
                        FullName = u.FullName,
                        Role = u.Role,
                        StudentId = u.StudentId ?? "",
                        TeacherId = u.TeacherId ?? "",
                        Id = u.Id
                    }).ToList();

                    _usersGrid.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            panel.Controls.Add(_usersGrid);
        }

        private void BtnAddUser_Click(object? sender, EventArgs e)
        {
            var form = new frmUserDetail(_apiClient);
            if (form.ShowDialog() == DialogResult.OK)
                LoadUsers();
        }

        private void BtnEditUser_Click(object? sender, EventArgs e)
        {
            if (_usersGrid?.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _usersGrid.SelectedRows[0];
            var id = (int)row.Cells["Id"].Value;
            var form = new frmUserDetail(_apiClient, id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadUsers();
        }

        private async void BtnDeleteUser_Click(object? sender, EventArgs e)
        {
            if (_usersGrid?.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var row = _usersGrid.SelectedRows[0];
                var id = (int)row.Cells["Id"].Value;
                
                var success = await _apiClient.DeleteAsync($"Users/{id}");
                if (success)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Subjects Management
        private async void LoadSubjects()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "Quản lý Môn học",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);

            var lblDesc = new Label
            {
                Text = "Thêm, sửa, xóa và xem danh sách các môn học trong hệ thống",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 50),
                AutoSize = true
            };
            panel.Controls.Add(lblDesc);

            var btnAdd = new Button
            {
                Text = "Thêm môn học",
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Size = new Size(120, 35),
                Location = new Point(20, 80),
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.Click += BtnAddSubject_Click;
            panel.Controls.Add(btnAdd);

            var btnEdit = new Button
            {
                Text = "Sửa",
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.White,
                Size = new Size(100, 35),
                Location = new Point(150, 80),
                FlatStyle = FlatStyle.Flat
            };
            btnEdit.Click += BtnEditSubject_Click;
            panel.Controls.Add(btnEdit);

            var btnDelete = new Button
            {
                Text = "Xóa",
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Size = new Size(100, 35),
                Location = new Point(260, 80),
                FlatStyle = FlatStyle.Flat
            };
            btnDelete.Click += BtnDeleteSubject_Click;
            panel.Controls.Add(btnDelete);

            var txtSearch = new TextBox
            {
                Size = new Size(300, 30),
                Location = new Point(380, 83),
                PlaceholderText = "Tìm kiếm..."
            };
            panel.Controls.Add(txtSearch);

            var btnSearch = new Button
            {
                Text = "Tìm",
                Size = new Size(80, 35),
                Location = new Point(690, 80),
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.Click += async (s, e) => await LoadSubjectsTable(panel, txtSearch.Text);
            panel.Controls.Add(btnSearch);

            await LoadSubjectsTable(panel, "");
            panelContent.Controls.Add(panel);
        }

        private DataGridView? _subjectsGrid;
        private async Task LoadSubjectsTable(Panel panel, string search)
        {
            if (_subjectsGrid != null)
                panel.Controls.Remove(_subjectsGrid);

            _subjectsGrid = new DataGridView
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
                var subjects = await _apiClient.GetAsync<List<dynamic>>($"Subjects?search={search}");
                if (subjects != null)
                {
                    _subjectsGrid.DataSource = subjects.Select((s, i) => new
                    {
                        STT = i + 1,
                        SubjectCode = (string)s.subjectCode,
                        SubjectName = (string)s.subjectName,
                        Credits = (int)s.credits,
                        Code = (string)s.subjectCode
                    }).ToList();

                    _subjectsGrid.Columns["Code"].Visible = false;
                }
            }
            catch { }

            panel.Controls.Add(_subjectsGrid);
        }

        private void BtnAddSubject_Click(object? sender, EventArgs e)
        {
            var form = new frmSubjectDetail(_apiClient);
            if (form.ShowDialog() == DialogResult.OK)
                LoadSubjects();
        }

        private void BtnEditSubject_Click(object? sender, EventArgs e)
        {
            if (_subjectsGrid?.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn môn học cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var code = _subjectsGrid.SelectedRows[0].Cells["Code"].Value.ToString() ?? "";
            var form = new frmSubjectDetail(_apiClient, code);
            if (form.ShowDialog() == DialogResult.OK)
                LoadSubjects();
        }

        private async void BtnDeleteSubject_Click(object? sender, EventArgs e)
        {
            if (_subjectsGrid?.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn môn học cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa môn học này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var code = _subjectsGrid.SelectedRows[0].Cells["Code"].Value.ToString() ?? "";
                var success = await _apiClient.DeleteAsync($"Subjects/{code}");
                if (success)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSubjects();
                }
            }
        }
        #endregion

        #region Course Sections Management

        private ComboBox _cmbSemesterFilter;
        private TextBox _txtSectionSearch;

        private async void LoadCourseSections()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label { Text = "Quản lý Lớp học phần", Font = new Font("Segoe UI", 16F, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };
            panel.Controls.Add(lblTitle);

            var btnAdd = new Button { Text = "+ Thêm lớp HP", BackColor = Color.FromArgb(0, 102, 204), ForeColor = Color.White, Size = new Size(130, 35), Location = new Point(20, 60), FlatStyle = FlatStyle.Flat };
            btnAdd.Click += BtnAddSection_Click;
            panel.Controls.Add(btnAdd);

            var btnView = new Button { Text = "Xem chi tiết", BackColor = Color.FromArgb(128, 0, 128), ForeColor = Color.White, Size = new Size(120, 35), Location = new Point(160, 60), FlatStyle = FlatStyle.Flat };
            btnView.Click += BtnViewSection_Click;
            panel.Controls.Add(btnView);

            // Xóa chữ 'var' để dùng biến toàn cục
            _cmbSemesterFilter = new ComboBox { Size = new Size(150, 30), Location = new Point(300, 63), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbSemesterFilter.Items.Add("Tất cả");
            _cmbSemesterFilter.Items.AddRange(new[] { "HK1/2025", "HK2/2024-2025", "HK1/2024", "HK2/2025-2026" });
            _cmbSemesterFilter.SelectedIndex = 0;
            panel.Controls.Add(_cmbSemesterFilter);

            // Xóa chữ 'var' để dùng biến toàn cục
            _txtSectionSearch = new TextBox { Size = new Size(250, 30), Location = new Point(470, 63), PlaceholderText = "Tìm kiếm..." };
            panel.Controls.Add(_txtSectionSearch);

            var btnSearch = new Button { Text = "Tìm", Size = new Size(80, 35), Location = new Point(730, 60), FlatStyle = FlatStyle.Flat };
            // Sự kiện Click dùng biến toàn cục
            btnSearch.Click += async (s, e) => await LoadSectionsTable(panel, _cmbSemesterFilter.Text, _txtSectionSearch.Text);
            panel.Controls.Add(btnSearch);

            await LoadSectionsTable(panel, "Tất cả", "");
            panelContent.Controls.Add(panel);
        }

        private async Task LoadSectionsTable(Panel panel, string semester, string search)
        {
            if (_sectionsGrid != null) panel.Controls.Remove(_sectionsGrid);

            _sectionsGrid = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(panel.Width - 40, panel.Height - 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(semester) && semester != "Tất cả") queryParams.Add($"semester={Uri.EscapeDataString(semester)}");
                if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

                // Gọi API
                var sections = await _apiClient.GetAsync<List<dynamic>>($"CourseSections{queryString}");

                if (sections != null)
                {
                    _sectionsGrid.DataSource = sections.Select((s, i) => new
                    {
                        STT = i + 1,
                        SectionCode = GetStringProperty(s, "sectionCode"),
                        SubjectName = GetStringProperty(s, "subjectName"),
                        TeacherName = GetStringProperty(s, "teacherName"),
                        Semester = GetStringProperty(s, "semester"),
                        DefaultRoom = GetStringProperty(s, "defaultRoom"),
                        Enrollment = GetIntProperty(s, "enrollmentCount"),
                        Code = GetStringProperty(s, "sectionCode")
                    }).ToList();

                    if (_sectionsGrid.Columns["Code"] != null) _sectionsGrid.Columns["Code"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải lớp học phần: {ex.Message}");
            }
            panel.Controls.Add(_sectionsGrid);
        }

        private string GetStringProperty(dynamic obj, string propertyName)
        {
            try
            {
                var type = obj.GetType();
                var prop = type.GetProperty(propertyName) ?? type.GetProperty(char.ToUpper(propertyName[0]) + propertyName.Substring(1));
                if (prop != null)
                {
                    var value = prop.GetValue(obj);
                    return value?.ToString() ?? "";
                }
                // Fallback cho dynamic
                if (obj is Newtonsoft.Json.Linq.JObject jobj)
                {
                    return jobj[propertyName]?.ToString() ?? jobj[char.ToUpper(propertyName[0]) + propertyName.Substring(1)]?.ToString() ?? "";
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        private int GetIntProperty(dynamic obj, string propertyName)
        {
            try
            {
                var type = obj.GetType();
                var prop = type.GetProperty(propertyName) ?? type.GetProperty(char.ToUpper(propertyName[0]) + propertyName.Substring(1));
                if (prop != null)
                {
                    var value = prop.GetValue(obj);
                    return value != null ? Convert.ToInt32(value) : 0;
                }
                // Fallback cho dynamic
                if (obj is Newtonsoft.Json.Linq.JObject jobj)
                {
                    var val = jobj[propertyName] ?? jobj[char.ToUpper(propertyName[0]) + propertyName.Substring(1)];
                    return val != null ? Convert.ToInt32(val) : 0;
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
                // Fallback cho dynamic
                if (obj is Newtonsoft.Json.Linq.JObject jobj)
                {
                    var val = jobj["examDate"] ?? jobj["ExamDate"];
                    if (val != null && DateTime.TryParse(val.ToString(), out DateTime parsedDate))
                        return parsedDate.ToString("dd/MM/yyyy");
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        private async void BtnAddSection_Click(object? sender, EventArgs e)
        {
            var form = new frmCourseSectionDetail(_apiClient);
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Tìm panel hiện tại trong panelContent
                Panel? currentPanel = null;
                foreach (Control control in panelContent.Controls)
                {
                    if (control is Panel pnl)
                    {
                        currentPanel = pnl;
                        break;
                    }
                }
                
                // Refresh lại bảng với bộ lọc hiện tại
                if (currentPanel != null && _cmbSemesterFilter != null && _txtSectionSearch != null)
                {
                    await LoadSectionsTable(currentPanel, _cmbSemesterFilter.Text, _txtSectionSearch.Text);
                }
                else
                {
                    // Nếu không tìm thấy panel, load lại từ đầu
                    LoadCourseSections();
                }
            }
        }

        private async void BtnViewSection_Click(object? sender, EventArgs e)
        {
            if (_sectionsGrid?.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn lớp học phần!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var code = _sectionsGrid.SelectedRows[0].Cells["Code"].Value.ToString() ?? "";
            var section = await _apiClient.GetAsync<dynamic>($"CourseSections/{code}");
            if (section != null)
            {
                var msg = $"Mã lớp: {section.sectionCode}\n" +
                         $"Môn học: {section.subjectName}\n" +
                         $"Giảng viên: {section.teacherName}\n" +
                         $"Học kỳ: {section.semester}\n" +
                         $"Phòng mặc định: {section.defaultRoom}";
                MessageBox.Show(msg, "Chi tiết lớp học phần", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Scores Management
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

            var btnEdit = new Button
            {
                Text = "Nhập/Sửa điểm",
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.White,
                Size = new Size(130, 35),
                Location = new Point(20, 80),
                FlatStyle = FlatStyle.Flat
            };
            btnEdit.Click += BtnEditScore_Click;
            panel.Controls.Add(btnEdit);

            var btnPublish = new Button
            {
                Text = "Công bố điểm",
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Size = new Size(130, 35),
                Location = new Point(160, 80),
                FlatStyle = FlatStyle.Flat
            };
            btnPublish.Click += BtnPublishScore_Click;
            panel.Controls.Add(btnPublish);

            cmbClassScore = new ComboBox
            {
                Size = new Size(200, 30),
                Location = new Point(310, 83),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbClassScore.Items.Add("Tất cả");
            
            // Load danh sách lớp học phần vào combobox
            try
            {
                var sections = await _apiClient.GetAsync<List<dynamic>>("CourseSections");
                if (sections != null)
                {
                    foreach (var s in sections)
                        cmbClassScore.Items.Add($"{GetStringProperty(s, "sectionCode")} - {GetStringProperty(s, "subjectName")}");
                }
            }
            catch { }
            
            cmbClassScore.SelectedIndex = 0; // Chọn mặc định cái đầu tiên
            panel.Controls.Add(cmbClassScore);

            btnSearchScores = new Button
            {
                Text = "Tìm",
                Size = new Size(80, 35),
                Location = new Point(520, 80),
                FlatStyle = FlatStyle.Flat
            };
            btnSearchScores.Click += async (s, e) => await LoadScoresTable(panel, cmbClassScore.Text);
            panel.Controls.Add(btnSearchScores);

            await LoadScoresTable(panel, "Tất cả");
            panelContent.Controls.Add(panel);
        }

        private DataGridView? _scoresGrid;
        private async Task LoadScoresTable(Panel panel, string sectionCode)
        {
            if (_scoresGrid != null)
                panel.Controls.Remove(_scoresGrid);

            _scoresGrid = new DataGridView
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
                List<dynamic> scores;
                if (string.IsNullOrEmpty(sectionCode) || sectionCode == "Tất cả")
                {
                    scores = await _apiClient.GetAsync<List<dynamic>>("Scores") ?? new List<dynamic>();
                }
                else
                {
                    var sectionCodeOnly = sectionCode.Contains(" - ") ? sectionCode.Split(" - ")[0] : sectionCode;
                    scores = await _apiClient.GetAsync<List<dynamic>>($"Scores?sectionCode={Uri.EscapeDataString(sectionCodeOnly)}") ?? new List<dynamic>();
                    
                    // Nếu chọn lớp cụ thể nhưng chưa có điểm, hiển thị danh sách sinh viên đã đăng ký
                    if (!scores.Any())
                    {
                        var enrollments = await _apiClient.GetAsync<List<dynamic>>($"Enrollments?sectionCode={Uri.EscapeDataString(sectionCodeOnly)}");
                        if (enrollments != null)
                        {
                            scores = enrollments.Select(e => (dynamic)new
                            {
                                id = 0,
                                studentId = (int)e.studentId,
                                studentCode = (string?)e.studentCode ?? "",
                                studentName = (string)e.studentName,
                                sectionCode = sectionCodeOnly,
                                subjectName = (string)e.subjectName,
                                attendanceScore = (decimal?)null,
                                midtermScore = (decimal?)null,
                                finalScore = (decimal?)null,
                                totalScore = (decimal?)null,
                                classification = (string?)null,
                                isPublished = false
                            }).ToList();
                        }
                    }
                }

                if (scores != null && scores.Any())
                {
                    _scoresGrid.DataSource = scores.Select((s, i) => new
                    {
                        STT = i + 1,
                        StudentCode = (string?)(s.studentCode ?? s.studentId?.ToString() ?? ""),
                        StudentName = (string)(s.studentName ?? ""),
                        AttendanceScore = s.attendanceScore?.ToString() ?? "Chưa có",
                        MidtermScore = s.midtermScore?.ToString() ?? "Chưa có",
                        FinalScore = s.finalScore?.ToString() ?? "Chưa có",
                        TotalScore = s.totalScore?.ToString() ?? "Chưa có",
                        Classification = (string?)(s.classification ?? "Chưa có"),
                        IsPublished = (bool)(s.isPublished ?? false),
                        Id = (int)(s.id ?? 0),
                        StudentId = (int)(s.studentId ?? 0),
                        SectionCode = (string?)(s.sectionCode ?? "")
                    }).ToList();
                    
                    if (_scoresGrid.Columns["Id"] != null)
                        _scoresGrid.Columns["Id"].Visible = false;
                    if (_scoresGrid.Columns["StudentId"] != null)
                        _scoresGrid.Columns["StudentId"].Visible = false;
                    if (_scoresGrid.Columns["SectionCode"] != null)
                        _scoresGrid.Columns["SectionCode"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải điểm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            panel.Controls.Add(_scoresGrid);
        }

        private async void BtnEditScore_Click(object? sender, EventArgs e)
        {
            // Nếu chưa chọn lớp thì phải chọn lớp trước
            if (cmbClassScore?.SelectedIndex == null || cmbClassScore.SelectedIndex <= 0 || cmbClassScore.Text == "Tất cả")
            {
                MessageBox.Show("Vui lòng chọn lớp học phần trước khi nhập điểm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sectionCode = cmbClassScore.Text.Contains(" - ") ? cmbClassScore.Text.Split(" - ")[0] : cmbClassScore.Text;

            // Hiển thị form chọn sinh viên hoặc mở form nhập điểm trực tiếp
            if (_scoresGrid?.SelectedRows.Count > 0)
            {
                // Nếu đã chọn dòng thì mở form sửa điểm
                var row = _scoresGrid.SelectedRows[0];
                int? scoreId = null;
                int studentId = 0;
                string? currentSectionCode = null;

                if (row.Cells["Id"]?.Value != null && (int)row.Cells["Id"].Value > 0)
                {
                    scoreId = (int)row.Cells["Id"].Value;
                }
                else
                {
                    // Nếu chưa có điểm, lấy thông tin từ row
                    if (row.Cells["StudentId"]?.Value != null)
                        studentId = (int)row.Cells["StudentId"].Value;
                    if (row.Cells["SectionCode"]?.Value != null)
                        currentSectionCode = row.Cells["SectionCode"].Value.ToString();
                }

                var form = scoreId.HasValue 
                    ? new frmScoreDetail(_apiClient, scoreId)
                    : new frmScoreDetail(_apiClient, null, currentSectionCode ?? sectionCode, studentId);
                    
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (btnSearchScores != null)
                        btnSearchScores.PerformClick();
                    else
                        LoadScores();
                }
            }
            else
            {
                // Hiển thị form chọn sinh viên để nhập điểm mới
                var enrollments = await _apiClient.GetAsync<List<dynamic>>($"Enrollments?sectionCode={Uri.EscapeDataString(sectionCode)}");
                if (enrollments == null || !enrollments.Any())
                {
                    MessageBox.Show("Lớp học phần này chưa có sinh viên đăng ký!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var frmSelectStudent = new frmSelectStudent(_apiClient, sectionCode, enrollments);
                if (frmSelectStudent.ShowDialog() == DialogResult.OK && frmSelectStudent.SelectedStudentId.HasValue)
                {
                    var form = new frmScoreDetail(_apiClient, null, sectionCode, frmSelectStudent.SelectedStudentId.Value);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        if (btnSearchScores != null)
                            btnSearchScores.PerformClick();
                        else
                            LoadScores();
                    }
                }
            }
        }

        private async void BtnPublishScore_Click(object? sender, EventArgs e)
        {
            if (cmbClassScore?.SelectedIndex == -1 || cmbClassScore.Text == "Tất cả")
            {
                MessageBox.Show("Vui lòng chọn lớp học phần cần công bố điểm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sectionCode = cmbClassScore.Text.Contains(" - ") ? cmbClassScore.Text.Split(" - ")[0] : cmbClassScore.Text;

            if (MessageBox.Show($"Bạn có chắc chắn muốn công bố điểm cho lớp {sectionCode}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await _apiClient.PostAsync<object>($"Scores/publish/{sectionCode}", null);
                    MessageBox.Show("Công bố điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (btnSearchScores != null)
                        btnSearchScores.PerformClick();
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

        #region Exam Schedules Management
        private ComboBox _cmbScheduleClass;
        private ComboBox _cmbScheduleYear;
        private ComboBox _cmbScheduleSemester;

        private async void LoadExamSchedules()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label { Text = "Quản lý Lịch thi", Font = new Font("Segoe UI", 16F, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };
            panel.Controls.Add(lblTitle);

            var btnAdd = new Button { Text = "Thêm lịch thi", BackColor = Color.FromArgb(0, 102, 204), ForeColor = Color.White, Size = new Size(120, 35), Location = new Point(20, 60), FlatStyle = FlatStyle.Flat };
            btnAdd.Click += BtnAddSchedule_Click;
            panel.Controls.Add(btnAdd);

            // Dùng biến toàn cục
            _cmbScheduleClass = new ComboBox { Size = new Size(150, 30), Location = new Point(160, 63), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbScheduleClass.Items.Add("Tất cả");
            // Load lớp học phần
            try
            {
                var sections = await _apiClient.GetAsync<List<dynamic>>("CourseSections");
                if (sections != null) foreach (var s in sections) _cmbScheduleClass.Items.Add($"{GetStringProperty(s, "sectionCode")}");
            }
            catch { }
            _cmbScheduleClass.SelectedIndex = 0;
            panel.Controls.Add(_cmbScheduleClass);

            _cmbScheduleYear = new ComboBox { Size = new Size(100, 30), Location = new Point(330, 63), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbScheduleYear.Items.Add("Tất cả");
            _cmbScheduleYear.Items.AddRange(new object[] { 2024, 2025, 2026 }); // Thêm năm vào
            _cmbScheduleYear.SelectedIndex = 0;
            panel.Controls.Add(_cmbScheduleYear);

            _cmbScheduleSemester = new ComboBox { Size = new Size(120, 30), Location = new Point(450, 63), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbScheduleSemester.Items.Add("Tất cả");
            _cmbScheduleSemester.Items.AddRange(new[] { "HK1/2025", "HK2/2024-2025" });
            _cmbScheduleSemester.SelectedIndex = 0;
            panel.Controls.Add(_cmbScheduleSemester);

            var btnSearch = new Button { Text = "Lọc", Size = new Size(80, 35), Location = new Point(590, 60), FlatStyle = FlatStyle.Flat };
            btnSearch.Click += async (s, e) => await LoadSchedulesTable(panel, _cmbScheduleClass.Text, _cmbScheduleYear.Text, _cmbScheduleSemester.Text);
            panel.Controls.Add(btnSearch);

            await LoadSchedulesTable(panel, "Tất cả", "Tất cả", "Tất cả");
            panelContent.Controls.Add(panel);
        }

        private async Task LoadSchedulesTable(Panel panel, string sectionCode, string year, string semester)
        {
            if (_schedulesGrid != null) panel.Controls.Remove(_schedulesGrid);

            _schedulesGrid = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(panel.Width - 40, panel.Height - 130),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            try
            {
                // Logic tạo Query String
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(sectionCode) && sectionCode != "Tất cả")
                    queryParams.Add($"sectionCode={Uri.EscapeDataString(sectionCode.Split(" - ")[0])}");

                if (!string.IsNullOrEmpty(year) && year != "Tất cả" && int.TryParse(year, out int y))
                    queryParams.Add($"year={y}");

                if (!string.IsNullOrEmpty(semester) && semester != "Tất cả")
                    queryParams.Add($"semester={Uri.EscapeDataString(semester)}");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

                var schedules = await _apiClient.GetAsync<List<dynamic>>($"ExamSchedules{queryString}");

                if (schedules != null)
                {
                    _schedulesGrid.DataSource = schedules.Select((s, i) => new
                    {
                        STT = i + 1,
                        SectionCode = GetStringProperty(s, "sectionCode"),
                        SubjectName = GetStringProperty(s, "subjectName"),
                        ExamDate = GetExamDateString(s),
                        ExamTime = GetStringProperty(s, "examTime"),
                        Room = GetStringProperty(s, "room"),
                        Location = GetStringProperty(s, "location"),
                        Id = GetIntProperty(s, "id")
                    }).ToList();

                    if (_schedulesGrid.Columns["Id"] != null) _schedulesGrid.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải lịch thi: {ex.Message}");
            }
            panel.Controls.Add(_schedulesGrid);
        }

        private async void BtnAddSchedule_Click(object? sender, EventArgs e)
        {
            var form = new frmExamScheduleDetail(_apiClient);
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Tìm panel hiện tại trong panelContent
                Panel? currentPanel = null;
                foreach (Control control in panelContent.Controls)
                {
                    if (control is Panel pnl)
                    {
                        currentPanel = pnl;
                        break;
                    }
                }
                
                // Refresh lại bảng với bộ lọc hiện tại
                if (currentPanel != null && _cmbScheduleClass != null && _cmbScheduleYear != null && _cmbScheduleSemester != null)
                {
                    await LoadSchedulesTable(currentPanel, _cmbScheduleClass.Text, _cmbScheduleYear.Text, _cmbScheduleSemester.Text);
                }
                else
                {
                    // Nếu không tìm thấy panel, load lại từ đầu
                    LoadExamSchedules();
                }
            }
        }
        #endregion

        #region Notifications
        private async void LoadNotifications()
        {
            panelContent.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "Quản lý Thông báo",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);

            var btnAdd = new Button
            {
                Text = "+ Thêm thông báo",
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Size = new Size(150, 35),
                Location = new Point(20, 60),
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.Click += BtnAddNotification_Click;
            panel.Controls.Add(btnAdd);

            var grid = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(panel.Width - 40, panel.Height - 130),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Mock data for notifications
            grid.DataSource = new[]
            {
                new { STT = 1, Title = "Thông báo về quy chế thi cuối kỳ HK1", Content = "Nhắc nhở sinh viên chuẩn bị tốt cho kỳ thi", CreatedAt = "1 ngày trước" },
                new { STT = 2, Title = "Cập nhật lịch thi cuối kỳ", Content = "Môn Cơ sở dữ liệu - IT302", CreatedAt = "5 giờ trước" },
                new { STT = 3, Title = "Admin đã thêm tài khoản mới", Content = "Tài khoản: gv2 - Nguyễn Thị C", CreatedAt = "2 giờ trước" }
            };

            panel.Controls.Add(grid);
            panelContent.Controls.Add(panel);
        }

        private void BtnAddNotification_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Chức năng thêm thông báo đang được phát triển", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        // Event handlers for menu buttons
        private void btnHome_Click(object sender, EventArgs e) => LoadHome();
        private void btnUsers_Click(object sender, EventArgs e) => LoadUsers();
        private void btnSubjects_Click(object sender, EventArgs e) => LoadSubjects();
        private void btnCourseSections_Click(object sender, EventArgs e) => LoadCourseSections();
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
