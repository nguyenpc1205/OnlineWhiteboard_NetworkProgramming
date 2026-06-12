using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace WhiteboardClient
{
    public partial class Form1 : Form
    {
        // Các biến phục vụ chức năng xử lý đồ họa nét vẽ
        private bool isDrawing = false;
        private Point lastPoint;
        private Color currentBrushColor = Color.Black;
        private float brushSize = 3f;
        private bool isEraser = false;
        private Panel canvasPanel;

        public Form1()
        {
            InitializeComponent();

            // 1. Tự động dựng vùng bảng vẽ lấp đầy không gian màn hình
            InitializeCanvasPanel();

            // 2. Nạp danh sách thành viên ảo hiển thị lên thanh điều khiển
            LoadMockUsers();
        }

        // --- PHÂN HỆ 1: QUẢN LÝ GIAO DIỆN THÀNH VIÊN ONLINE ---
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
            lblStatus.Text = " ■  Online";
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


        // --- PHÂN HỆ 2: KHỞI TẠO VÀ XỬ LÝ SỰ KIỆN BẢNG VẼ ---
        private void InitializeCanvasPanel()
        {
            canvasPanel = new Panel
            {
                Dock = DockStyle.Fill, // Tự động lấp đầy phần diện tích còn lại của Form
                BackColor = Color.White
            };

            // Gắn 3 sự kiện tương tác chuột thiết yếu vào bảng vẽ
            canvasPanel.MouseDown += CanvasPanel_MouseDown;
            canvasPanel.MouseMove += CanvasPanel_MouseMove;
            canvasPanel.MouseUp += CanvasPanel_MouseUp;

            this.Controls.Add(canvasPanel); // Đưa bảng vẽ lên trên giao diện

            // Đảm bảo bảng vẽ không đè lên thanh danh sách người dùng online
            canvasPanel.SendToBack();
        }

        private void CanvasPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                lastPoint = e.Location;
            }
        }

        private void CanvasPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                using (Graphics g = canvasPanel.CreateGraphics())
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias; // Làm mượt nét vẽ, chống răng cưa
                    Color activeColor = isEraser ? Color.White : currentBrushColor;

                    using (Pen drawingPen = new Pen(activeColor, brushSize))
                    {
                        drawingPen.StartCap = LineCap.Round; // Bo tròn điểm đầu nét vẽ
                        drawingPen.EndCap = LineCap.Round;   // Bo tròn điểm cuối nét vẽ
                        g.DrawLine(drawingPen, lastPoint, e.Location); // Vẽ đường nối điểm cũ và điểm mới
                    }
                }
                lastPoint = e.Location; // Cập nhật lại tọa độ hiện tại
            }
        }

        private void CanvasPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = false;
            }
        }
    }
}
