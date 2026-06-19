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
        private Button btnClearAll;
        private Button btnSaveImage;
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
            // 1. Cấu hình nút Xóa Toàn Bộ (Đã thêm chữ Button ở đầu để hết đỏ)
            Button btnClearAll = new Button();
            btnClearAll.Text = "Xóa Toàn Bộ";
            btnClearAll.Size = new Size(100, 30);
            btnClearAll.Click += BtnClearAll_Click;

            // 2. Cấu hình nút Lưu Ảnh (Đã thêm chữ Button ở đầu để hết đỏ)
            Button btnSaveImage = new Button();
            btnSaveImage.Text = "Lưu Ảnh";
            btnSaveImage.Size = new Size(100, 30);
            btnSaveImage.Click += BtnSaveImage_Click;

            // 3. Đưa 2 nút lên Form (Đã thay chữ pnlAutomaticToolbar bằng chữ "this")
            this.Controls.Add(btnClearAll);
            this.Controls.Add(btnSaveImage);

            // 1. Tự động dựng vùng bảng vẽ lấp đầy không gian màn hình
            InitializeCanvasPanel();

            // 2. Nạp danh sách thành viên ảo hiển thị lên thanh điều khiển
            LoadMockUsers();
            // 3. ĐĂNG KÝ SỰ KIỆN: Khi mạng nhận được nét vẽ từ máy khác, tự động gọi hàm HandleNetworkData để vẽ lên màn hình của mình
            NetworkClient.OnDataReceived += HandleNetworkData;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string myName = "Nguyễn Văn An";

                // SỬA: Đổi từ NetworkClient.client thành NetworkClient.tcpClient
                if (NetworkClient.tcpClient != null && NetworkClient.tcpClient.Connected)
                {
                    // SỬA: Đổi từ NetworkClient.client thành NetworkClient.tcpClient
                    StreamWriter writer = new StreamWriter(NetworkClient.tcpClient.GetStream()) { AutoFlush = true };
                    writer.WriteLine($"CONNECT;{myName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi kết nối] {ex.Message}");
            }
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

            this.currentBrushColor = Color.Black; 
            this.brushSize = 4f;                 
            this.isEraser = false;
            this.Controls.Add(canvasPanel);
            canvasPanel.BringToFront();
            if (flpOnlineUsers != null)
            {
                flpOnlineUsers.BringToFront();
            }

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

                NetworkClient.SendDrawData(lastPoint.X, lastPoint.Y, e.Location.X, e.Location.Y, currentBrushColor, brushSize, isEraser);

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


        /// <summary>
        /// HÀM NHẬN VÀ VẼ: Đọc chuỗi nét vẽ từ người khác gửi tới qua Server để vẽ đè lên màn hình cục bộ
        /// </summary>
        private void HandleNetworkData(string message)
        {
            // Tránh xung đột luồng giao diện khi chạy đa luồng (Thread-safe UI)
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(HandleNetworkData), message);
                return;
            }

            try
            {
                string[] parts = message.Split(';');
                string command = parts[0];

                if (command == "DRAW")
                {
                    // Bóc tách bộ tọa độ: X1, Y1, X2, Y2
                    string[] coords = parts[1].Split(',');
                    int x1 = int.Parse(coords[0]);
                    int y1 = int.Parse(coords[1]);
                    int x2 = int.Parse(coords[2]);
                    int y2 = int.Parse(coords[3]);

                    // Bóc tách thông tin màu sắc và độ dày bút vẽ
                    string colorInfo = parts[2];
                    float size = float.Parse(parts[3]);

                    Color drawColor;
                    if (colorInfo == "ERASE")
                    {
                        drawColor = Color.White; // Nếu máy khác đang chọn tẩy, ta vẽ nét màu trắng xóa đè lên
                    }
                    else
                    {
                        string[] rgb = colorInfo.Split(',');
                        drawColor = Color.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));
                    }

                    // Tiến hành tự động vẽ nét của người khác lên bảng vẽ của mình
                    using (Graphics g = canvasPanel.CreateGraphics())
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        using (Pen remotePen = new Pen(drawColor, size))
                        {
                            remotePen.StartCap = LineCap.Round;
                            remotePen.EndCap = LineCap.Round;
                            g.DrawLine(remotePen, new Point(x1, y1), new Point(x2, y2));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi xử lý vẽ mạng] {ex.Message}");
            }
        }

        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {
        }

    }
        }
    
    private void BtnClearAll_Click(object sender, EventArgs e)
        {
            // Làm trắng bảng vẽ cục bộ ngay lập tức
            canvasPanel.Invalidate();
        }

        private void BtnSaveImage_Click(object sender, EventArgs e)
        {
            // Tạo đối tượng ảnh Bitmap với kích thước bằng canvasPanel
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(canvasPanel.Width, canvasPanel.Height);

            // Chụp lại những gì đã vẽ trên panel đưa vào bitmap
            canvasPanel.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0, 0, canvasPanel.Width, canvasPanel.Height));

            // Mở hộp thoại để người dùng chọn nơi lưu file .png
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image (*.png)|*.png";
                saveFileDialog.Title = "Chọn nơi lưu bức tranh của bạn";
                saveFileDialog.FileName = "Whiteboard_Export.png";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Tiến hành lưu ảnh xuống máy
                    bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show("Lưu ảnh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            bitmap.Dispose();
        }
    }
}
    
