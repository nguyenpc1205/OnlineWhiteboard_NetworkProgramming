using System;
using System.Windows.Forms;

namespace WhiteboardClient
{
    public partial class FormRoomSelection : Form
    {
        public FormRoomSelection()
        {
            InitializeComponent();
        }
        private void FormRoomSelection_Load(object sender, EventArgs e)
        {
        }
        private void label2_Click(object sender, EventArgs e)
        {
        }
        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        // Nút Tạo phòng
        private void btnCreateRoom_Click(object sender, EventArgs e)
        {
            string userName = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("Vui lòng nhập tên người dùng!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string roomCode = Guid.NewGuid()
                                  .ToString()
                                  .Substring(0, 6)
                                  .ToUpper();

            Form1 mainForm = new Form1(userName, roomCode);

            this.Hide();
            mainForm.Show();
        }

        // Nút Vào phòng
        private void btnJoinRoom_Click(object sender, EventArgs e)
        {
            string userName = textBox1.Text.Trim();
            string roomCode = txtRoomCode.Text.Trim();

            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("Vui lòng nhập tên người dùng!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(roomCode))
            {
                MessageBox.Show("Vui lòng nhập mã phòng!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Form1 mainForm = new Form1(userName, roomCode);

            this.Hide();
            mainForm.Show();
        }
    }
}
