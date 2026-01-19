using QuanLyThi.Client.Models;
using QuanLyThi.Client.Services;

namespace QuanLyThi.Client
{
    public partial class frmDangNhap : Form
    {
        private readonly ApiClient _apiClient;

        public frmDangNhap()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            comboBox1.Items.AddRange(new[] { "Admin", "Teacher", "Student" });
            comboBox1.SelectedIndex = 0;
            textBox2.PasswordChar = '*';
        }

        private async void btnDangNhap_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || 
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnDangNhap.Enabled = false;
            btnDangNhap.Text = "Đang đăng nhập...";

            try
            {
                var request = new LoginRequest
                {
                    Username = textBox1.Text,
                    Password = textBox2.Text,
                    Role = comboBox1.SelectedItem.ToString() ?? ""
                };

                var response = await _apiClient.PostAsync<LoginResponse>("Auth/login", request);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    _apiClient.SetToken(response.Token);
                    
                    // Open appropriate dashboard based on role
                    Form? dashboard = response.User.Role switch
                    {
                        "Admin" => new Forms.frmAdmin(response.User, _apiClient),
                        "Teacher" => new Forms.frmTeacher(response.User, _apiClient),
                        "Student" => new Forms.frmStudent(response.User, _apiClient),
                        _ => null
                    };

                    if (dashboard != null)
                    {
                        this.Hide();
                        dashboard.ShowDialog();
                        this.Show();
                        textBox1.Clear();
                        textBox2.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập, mật khẩu hoặc vai trò không đúng!", 
                        "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}\n\nVui lòng kiểm tra API đã chạy chưa!", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDangNhap.Enabled = true;
                btnDangNhap.Text = "Đăng nhập";
            }
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng đăng ký chưa được triển khai.\nVui lòng liên hệ quản trị viên!", 
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnQuenMatKhau_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng quên mật khẩu chưa được triển khai.\nVui lòng liên hệ quản trị viên!", 
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
