using De01.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace De01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            db = new Model1();
        }
        private Model1 db;

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            loaddatatoCombobox();
        }
        private void LoadData()
        {
            try
            {
                var data = db.SINHVIENs.Select(c => new
                {
                    c.MaSV,
                    c.HoTenSV,
                    MaLop = c.MaLop ?? "Chưa có",
                    c.NgaySinh
                }).ToList();
                DtgSinhVien.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private void loaddatatoCombobox()
        {
            try
            {
                var classList = db.LOPs.Select(c => new
                {
                    c.MaLop,
                    c.TenLop // Cột tên lớp
                }).ToList();
                cmbLop.DataSource = classList;
                cmbLop.DisplayMember = "TenLop";
                cmbLop.ValueMember = "MaLop";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void DtgSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = DtgSinhVien.Rows[e.RowIndex];
                txtMaSV.Text = selectedRow.Cells[0].Value?.ToString();
                txtHoTen.Text = selectedRow.Cells[1].Value?.ToString();
                string cellValue = selectedRow.Cells[2].Value?.ToString();
                if (DateTime.TryParse(cellValue, out DateTime parsedDate))
                {
                    dtpNgaySinh.Value = parsedDate;
                }
                else
                {
                    MessageBox.Show("Dữ liệu ngày tháng không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtpNgaySinh.Value = DateTime.Now; // Đặt giá trị mặc định
                }
                cmbLop.SelectedItem = selectedRow.Cells[3].Value?.ToString();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                Model1 db = new Model1();
                if (db.SINHVIENs.FirstOrDefault(s => s.MaSV == txtMaSV.Text) != null)
                {
                    MessageBox.Show("Mã số sinh viên này đã tồn tại, vui lòng nhập mã khác.", "Thông báo !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtMaSV.Text))
                {
                    MessageBox.Show("Mã số sinh viên không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtHoTen.Text))
                {
                    MessageBox.Show("Họ tên sinh viên không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbLop.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn lớp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var newSinhVien = new SINHVIEN
                {
                    MaSV = txtMaSV.Text,
                    HoTenSV = txtHoTen.Text,
                    NgaySinh = dtpNgaySinh.Value,
                    MaLop = cmbLop.SelectedValue.ToString()
                };
                db.SINHVIENs.Add(newSinhVien);
                db.SaveChanges();
                LoadData();
                MessageBox.Show("Đã thêm thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        MessageBox.Show($"Thuộc tính: {validationError.PropertyName} - Lỗi: {validationError.ErrorMessage}",
                                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaSV.Text))
                {
                    MessageBox.Show("Mã số sinh viên không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var sinhVien = db.SINHVIENs.FirstOrDefault(s => s.MaSV == txtMaSV.Text);
                if (sinhVien == null)
                {
                    MessageBox.Show("Sinh viên không tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                sinhVien.HoTenSV = txtHoTen.Text;
                sinhVien.NgaySinh = dtpNgaySinh.Value;
                sinhVien.MaLop = cmbLop.SelectedValue?.ToString();
                db.SaveChanges();
                MessageBox.Show("Đã sửa thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sửa dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaSV.Text))
                {
                    MessageBox.Show("Vui lòng chọn sinh viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult kq = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?",
                                                             "Xác nhận xóa",
                                                             MessageBoxButtons.YesNo,
                                                             MessageBoxIcon.Question);
                if (kq == DialogResult.Yes)
                {
                    var sinhVien = db.SINHVIENs.FirstOrDefault(s => s.MaSV == txtMaSV.Text);
                    if (sinhVien == null)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    db.SINHVIENs.Remove(sinhVien);
                    db.SaveChanges();
                    LoadData();
                    MessageBox.Show("Đã xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            btnLuu.Enabled = false;
        }

        private void btnKluu_Click(object sender, EventArgs e)
        {
            btnKluu.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult kq = MessageBox.Show("Bạn có chắc chắn muốn đóng form này?",
                                                             "Xác nhận đóng",
                                                             MessageBoxButtons.YesNo,
                                                             MessageBoxIcon.Question);
            if (kq == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                string timTen = txtTim.Text.Trim(); 
                if (string.IsNullOrWhiteSpace(timTen))
                {
                    LoadData();
                }
                else
                {
                    var searchData = db.SINHVIENs.Where(c => c.HoTenSV.Contains(timTen)) 
                        .Select(c => new
                                        {
                                            c.MaSV,
                                            c.HoTenSV,
                                            MaLop = c.MaLop ?? "Chưa có",
                                            c.NgaySinh
                                        }).ToList();
                    DtgSinhVien.DataSource = searchData; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    }
}
