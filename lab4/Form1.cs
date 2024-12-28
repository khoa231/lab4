using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab4
{
    public partial class frmQuanLySV : Form
    {
        public frmQuanLySV()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load dữ liệu bảng Student vào DataGridView
            LoadStudentData();

            // Load danh sách khoa vào ComboBox
            using (SqlConnection conn = new SqlConnection("Data Source=NGUYEN_TUAN_VU\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT FacultyID, FacultyName FROM Faculty", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);
                cbbKhoa.DataSource = dt;
                cbbKhoa.DisplayMember = "FacultyName";
                cbbKhoa.ValueMember = "FacultyID";
            }
        }

        private void LoadStudentData()
        {
            using (SqlConnection conn = new SqlConnection("Data Source=NGUYEN_TUAN_VU\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True"))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Student", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvSV.DataSource = dt;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                using (SqlConnection conn = new SqlConnection("Data Source=NGUYEN_TUAN_VU\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True"))
                {
                    conn.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Student WHERE StudentID = @StudentID", conn);
                    checkCmd.Parameters.AddWithValue("@StudentID", txtMSSV.Text);

                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        MessageBox.Show("Mã số sinh viên đã tồn tại!");
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO Student (StudentID, FullName, AverageScore, FacultyID) VALUES (@StudentID, @FullName, @AverageScore, @FacultyID)", conn);
                        cmd.Parameters.AddWithValue("@StudentID", txtMSSV.Text);
                        cmd.Parameters.AddWithValue("@FullName", txtHoten.Text);
                        cmd.Parameters.AddWithValue("@AverageScore", float.Parse(txtDiemTB.Text));
                        cmd.Parameters.AddWithValue("@FacultyID", cbbKhoa.SelectedValue);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Thêm mới dữ liệu thành công!");
                        ResetInput();
                        LoadStudentData();
                    }
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                using (SqlConnection conn = new SqlConnection("Data Source=NGUYEN_TUAN_VU\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True"))
                {
                    conn.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Student WHERE StudentID = @StudentID", conn);
                    checkCmd.Parameters.AddWithValue("@StudentID", txtMSSV.Text);

                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        SqlCommand cmd = new SqlCommand("UPDATE Student SET FullName = @FullName, AverageScore = @AverageScore, FacultyID = @FacultyID WHERE StudentID = @StudentID", conn);
                        cmd.Parameters.AddWithValue("@StudentID", txtMSSV.Text);
                        cmd.Parameters.AddWithValue("@FullName", txtHoten.Text);
                        cmd.Parameters.AddWithValue("@AverageScore", float.Parse(txtDiemTB.Text));
                        cmd.Parameters.AddWithValue("@FacultyID", cbbKhoa.SelectedValue);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cập nhật dữ liệu thành công!");
                        ResetInput();
                        LoadStudentData();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy MSSV cần sửa!");
                    }
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtMSSV.Text) ||  txtMSSV.Text.Length != 10)
            {
                MessageBox.Show("Mã số sinh viên phải có 10 kí tự!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtHoten.Text) || string.IsNullOrWhiteSpace(txtDiemTB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return false;
            }

            if (!float.TryParse(txtDiemTB.Text, out float score) || score < 0 || score > 10)
            {
                MessageBox.Show("Điểm trung bình phải nằm trong khoảng 0-10!");
                return false;
            }

            return true;
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection("Data Source=NGUYEN_TUAN_VU\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True"))
            {
                conn.Open();

                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Student WHERE StudentID = @StudentID", conn);
                checkCmd.Parameters.AddWithValue("@StudentID", txtMSSV.Text);

                int exists = (int)checkCmd.ExecuteScalar();

                if (exists == 0)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần xóa!");
                }
                else
                {
                    DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        SqlCommand cmd = new SqlCommand("DELETE FROM Student WHERE StudentID = @StudentID", conn);
                        cmd.Parameters.AddWithValue("@StudentID", txtMSSV.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Xóa sinh viên thành công!");
                        ResetInput();
                        LoadStudentData();
                    }
                }
            }
        }

        private void ResetInput()
        {
            txtMSSV.Clear();
            txtHoten.Clear();
            txtDiemTB.Clear();
            cbbKhoa.SelectedIndex = 0;
        }

        private void dgvSV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSV.Rows[e.RowIndex];

                txtMSSV.Text = row.Cells["StudentID"].Value.ToString();
                txtHoten.Text = row.Cells["FullName"].Value.ToString();
                txtDiemTB.Text = row.Cells["AverageScore"].Value.ToString();
                cbbKhoa.SelectedValue = row.Cells["FacultyID"].Value;
            }
        }
    }


}
