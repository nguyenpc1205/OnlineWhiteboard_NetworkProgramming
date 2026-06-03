using System;
using System.Drawing;
using System.Windows.Forms;

namespace WhiteboardClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadMockUsers();
        }

        private void LoadMockUsers()
        {
            AddUser("Nguyễn Văn An", Color.Crimson);
            AddUser("Trần Thị Bình", Color.LimeGreen);
            AddUser("Lê Minh Cường", Color.Blue);
            AddUser("Phạm Thu Dung", Color.Magenta);
        }

        private void AddUser(string name, Color avatarColor)
        {
            Panel pnlUser = new Panel();
            pnlUser.Width = flpOnlineUsers.Width - 15;
            pnlUser.Height = 50;
            pnlUser.Margin = new Padding(0, 0, 0, 5);
            pnlUser.BackColor = Color.White;

            Panel pnlBorder = new Panel();
            pnlBorder.Height = 1;
            pnlBorder.Dock = DockStyle.Bottom;
            pnlBorder.BackColor = Color.LightGray;
            pnlUser.Controls.Add(pnlBorder);

            Label lblAvatar = new Label();
            lblAvatar.Text = name[0].ToString().ToUpper();
            lblAvatar.Size = new Size(30, 30);
            lblAvatar.Location = new Point(5, 10);
            lblAvatar.BackColor = avatarColor;
            lblAvatar.ForeColor = Color.White;
            lblAvatar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAvatar.TextAlign = ContentAlignment.MiddleCenter;
            pnlUser.Controls.Add(lblAvatar);

            Label lblName = new Label();
            lblName.Text = name;
            lblName.Location = new Point(45, 8);
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 9F);
            pnlUser.Controls.Add(lblName);

            Label lblStatus = new Label();
            lblStatus.Text = "■ Online";
            lblStatus.Location = new Point(45, 25);
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.LimeGreen;
            lblStatus.Font = new Font("Segoe UI", 8F);
            pnlUser.Controls.Add(lblStatus);

            Label lblColorBadge = new Label();
            lblColorBadge.Size = new Size(15, 15);
            lblColorBadge.Location = new Point(170, 17);
            lblColorBadge.BackColor = avatarColor;
            pnlUser.Controls.Add(lblColorBadge);

            flpOnlineUsers.Controls.Add(pnlUser);
        }
    }
}