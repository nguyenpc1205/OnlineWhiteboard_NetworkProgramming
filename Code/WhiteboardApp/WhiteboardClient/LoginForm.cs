using System;
using System.Drawing;
using System.Windows.Forms;

namespace WhiteboardClient
{
    public partial class LoginForm : Form
    {
        private Label lblTitle;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblRoomID;
        private TextBox txtRoomID;
        private Button btnCreateRoom;
        private Button btnJoinRoom;

        public LoginForm()
        {
            // Cấu hình cửa sổ Login chính
            this.Text = "Đăng Nhập Hệ Thống Vẽ Nhóm";
            this.Size = new Size(380, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            // 1. Tiêu đề
            lblTitle = new Label();
            lblTitle.Text = "WHITEBOARD ONLINE";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(41, 128, 185);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(320, 35);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // 2. Ô nhập Tên người dùng
            lblUsername = new Label();
            lblUsername.Text = "Nhập tên của bạn:";
            lblUsername.Font = new Font("Segoe UI", 10F);
            lblUsername.Location = new Point(40, 75);
            lblUsername.Size = new Size(150, 20);

            txtUsername = new TextBox();
            txtUsername.Font = new Font("Segoe UI", 11F);
            txtUsername.Location = new Point(40, 98);
            txtUsername.Size = new Size(285, 27);

            // 3. Ô nhập Mã phòng (Dành cho người thứ 2 muốn join)
            lblRoomID = new Label();
            lblRoomID.Text = "Mã phòng (Nếu muốn Vào Phòng):";
            lblRoomID.Font = new Font("Segoe UI", 10F);
            lblRoomID.Location = new Point(40, 135);
            lblRoomID.Size = new Size(250, 20);

            txtRoomID = new TextBox();
            txtRoomID.Font = new Font("Segoe UI", 11F);
            txtRoomID.Location = new Point(40, 158);
            txtRoomID.Size = new Size(285, 27);

            // 4. Nút bấm: TẠO PHÒNG MỚI
            btnCreateRoom = new Button();
            btnCreateRoom.Text = "➕ Tạo Phòng Mới";
            btnCreateRoom.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCreateRoom.BackColor = Color.FromArgb(46, 204, 113);
            btnCreateRoom.ForeColor = Color.White;
            btnCreateRoom.FlatStyle = FlatStyle.Flat;
            btnCreateRoom.Location = new Point(40, 210);
            btnCreateRoom.Size = new Size(135, 38);
            btnCreateRoom.Cursor = Cursors.Hand;
            btnCreateRoom.Click += BtnCreateRoom_Click;

            // 5. Nút bấm: VÀO PHÒNG ĐÃ CÓ
            btnJoinRoom = new Button();
            btnJoinRoom.Text = "🚪 Vào Phòng";
            btnJoinRoom.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnJoinRoom.BackColor = Color.FromArgb(52, 152, 219);
            btnJoinRoom.ForeColor = Color.White;
            btnJoinRoom.FlatStyle = FlatStyle.Flat;
            btnJoinRoom.Location = new Point(190, 210);
            btnJoinRoom.Size = new Size(135, 38);
            btnJoinRoom.Cursor = Cursors.Hand;
            btnJoinRoom.Click += BtnJoinRoom_Click;

            // Đưa toàn bộ các ô nhập lên màn hình hiển thị
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblRoomID);
            this.Controls.Add(txtRoomID);
            this.Controls.Add(btnCreateRoom);
            this.Controls.Add(btnJoinRoom);
        }

        // Xử lý khi Người thứ nhất bấm TẠO PHÒNG MỚI
        private void BtnCreateRoom_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên trước khi tạo phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tự động sinh ngẫu nhiên một mã phòng gồm chữ ROOM + 5 số ngẫu nhiên
            string cleanName = txtUsername.Text.Trim();
            string generatedRoomID = "ROOM" + new Random().Next(10000, 99999).ToString();

            // Mở bảng vẽ chính và truyền thông tin đi
            Form1 canvasForm = new Form1(cleanName, generatedRoomID, "CREATE_ROOM");
            this.Hide();
            canvasForm.FormClosed += (s, args) => this.Close();
            canvasForm.Show();
        }

        // Xử lý khi Người thứ hai nhập mã phòng và bấm VÀO PHÒNG
        private void BtnJoinRoom_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng điền tên của bạn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtRoomID.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã phòng do người thứ nhất gửi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cleanName = txtUsername.Text.Trim();
            string targetRoomID = txtRoomID.Text.Trim().ToUpper(); // Tự động viết hoa mã phòng để tránh lệch ký tự

            // Tiến hành mở bảng vẽ chung
            Form1 canvasForm = new Form1(cleanName, targetRoomID, "JOIN_ROOM");
            this.Hide();
            canvasForm.FormClosed += (s, args) => this.Close();
            canvasForm.Show();
        }
    }
}