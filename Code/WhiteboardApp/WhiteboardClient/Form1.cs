using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace WhiteboardClient
{
    public partial class Form1 : Form
    {
        // KHAI BÁO CÁC BIẾN TRẠNG THÁI VÀ CẤU HÌNH
        private enum DrawTool { Pen, Line, Rectangle, Circle, Eraser }
        private DrawTool currentTool = DrawTool.Pen;

        private Point shapeStartPoint;
        private Point shapeEndPoint;
        private bool isDrawingShape = false;

        private Bitmap canvasBitmap;
        private Graphics bitmapGraphics;

        private bool isDrawing = false;
        private Point lastPoint;
        private Point lastSentPoint; // Lưu điểm cuối cùng thực sự gửi qua mạng để tính khoảng cách lọc (Throttle)
        private Color currentBrushColor = Color.Black;
        private float brushSize = 3f;
        private string currentRoomId = "";
        private string currentUserName = "";

        private System.Windows.Forms.Timer syncTimer;

        /// <summary>
        /// Hàm khởi tạo Form mặc định (không tham số).
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            InitializepnlCanvas();
        }

        /// <summary>
        /// Hàm khởi tạo Form có tham số khi User đăng nhập.
        /// </summary>
        public Form1(string username, string roomID, string actionType)
        {
            InitializeComponent();

            // Đăng ký sự kiện Click cho các nút chọn công cụ vẽ
            if (btnPen != null) btnPen.Click += btnPen_Click;
            if (btnEraser != null) btnEraser.Click += btnEraser_Click;
            if (button1 != null) button1.Click += button1_Click;
            if (button2 != null) button2.Click += button2_Click;
            if (button3 != null) button3.Click += button3_Click;

            // Lưu thông tin người dùng và phòng
            this.currentUserName = username;
            this.currentRoomId = roomID;
            btnColor1.BackColor = Color.Black;
            lblRoomName.Text = $"PHÒNG VẼ: {roomID}";
            lblUserInfo.Text = username;

            // Đăng ký sự kiện cho nút Clear All và Save Image
            btnClearAll.Click += BtnClearAll_Click;
            btnSaveImage.Click += BtnSaveImage_Click;

            // Đưa các nút chức năng lên lớp trên cùng để tránh bị panel che khuất
            btnClearAll.BringToFront();
            btnSaveImage.BringToFront();

            InitializepnlCanvas();

            // Làm sạch event cũ và đăng ký nhận dữ liệu từ luồng mạng ngầm
            NetworkClient.OnDataReceived -= HandleNetworkData;
            NetworkClient.OnDataReceived += HandleNetworkData;

            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (pnlCanvas.Width > 0 && pnlCanvas.Height > 0)
            {
                canvasBitmap = new Bitmap(pnlCanvas.Width, pnlCanvas.Height);
                bitmapGraphics = Graphics.FromImage(canvasBitmap);
                bitmapGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                bitmapGraphics.Clear(Color.White);

                trackBrushSize.Value = (int)brushSize;
                lblBrushSizeText.Text = "Brush Size: " + brushSize.ToString() + "px";
                pnlCanvas.Invalidate();
            }

            if (flpOnlineUsers != null)
            {
                flpOnlineUsers.AutoScroll = true;
            }

            // Tự động báo danh sau 1 giây
            syncTimer = new System.Windows.Forms.Timer();
            syncTimer.Interval = 1000;
            syncTimer.Tick += SyncTimer_Tick;
            syncTimer.Start();
        }

        private void SyncTimer_Tick(object sender, EventArgs e)
        {
            syncTimer.Stop();
            if (NetworkClient.tcpClient != null && NetworkClient.tcpClient.Connected)
            {
                // Thêm \n ở cuối thông điệp để kết thúc dòng rạch ròi theo hợp đồng dữ liệu
                NetworkClient.SendMessage($"JOIN_ROOM;{currentUserName};{currentRoomId}\n");
            }
        }

        // CƠ CHẾ NHẬN MẠNG
        private void HandleNetworkData(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;

            // Sử dụng BeginInvoke bất đồng bộ không chặn luồng nhận mạng ngầm
            this.BeginInvoke((MethodInvoker)delegate
            {
                try
                {
                    string cleanData = data.Trim().Replace("\0", "").Replace("\uFEFF", "");
                    string[] parts = cleanData.Split(';');
                    if (parts.Length == 0) return;

                    // 1. NHẬN DANH SÁCH NGƯỜI DÙNG
                    if (parts[0] == "USER_LIST")
                    {
                        if (parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[1]))
                        {
                            CapNhatDanhSachThanhVien(parts[1].Split(','));
                        }
                    }
                    // 2. NHẬN LỆNH VẼ
                    else if (parts[0] == "DRAW")
                    {
                        // KIỂM TRA CHUẨN MỚI: Hình Học
                        if (parts.Length >= 2 && parts[1] == "SHAPE")
                        {
                            if (parts.Length >= 10)
                            {
                                string shapeType = parts[2];
                                int x1 = int.Parse(parts[3]);
                                int y1 = int.Parse(parts[4]);
                                int x2 = int.Parse(parts[5]);
                                int y2 = int.Parse(parts[6]);
                                Color netColor = parts[7].StartsWith("#") ? ColorTranslator.FromHtml(parts[7]) : Color.FromName(parts[7]);
                                float thickness = float.Parse(parts[8]);

                                DrawTool remoteTool = DrawTool.Line;
                                if (shapeType == "RECTANGLE") remoteTool = DrawTool.Rectangle;
                                else if (shapeType == "CIRCLE") remoteTool = DrawTool.Circle;

                                if (bitmapGraphics != null)
                                {
                                    using (Pen remotePen = new Pen(netColor, thickness))
                                    {
                                        DrawShapeGeneric(bitmapGraphics, remoteTool, new Point(x1, y1), new Point(x2, y2), remotePen);
                                    }

                                    // Tối ưu UI: Chỉ làm mới vùng biên của hình vẽ thay vì Invalidate toàn bộ bảng
                                    int x = Math.Min(x1, x2) - (int)thickness - 2;
                                    int y = Math.Min(y1, y2) - (int)thickness - 2;
                                    int w = Math.Abs(x1 - x2) + (int)thickness * 2 + 4;
                                    int h = Math.Abs(y1 - y2) + (int)thickness * 2 + 4;
                                    pnlCanvas.Invalidate(new Rectangle(x, y, w, h));
                                }
                            }
                        }
                        // KIỂM TRA CHUẨN MỚI: Tự Do
                        else if (parts.Length >= 2 && parts[1] == "FREEHAND")
                        {
                            if (parts.Length >= 9)
                            {
                                int x1 = int.Parse(parts[2]);
                                int y1 = int.Parse(parts[3]);
                                int x2 = int.Parse(parts[4]);
                                int y2 = int.Parse(parts[5]);
                                Color netColor = parts[6].StartsWith("#") ? ColorTranslator.FromHtml(parts[6]) : Color.FromName(parts[6]);
                                float thickness = float.Parse(parts[7]);

                                if (bitmapGraphics != null)
                                {
                                    using (Pen remotePen = new Pen(netColor, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                                    {
                                        bitmapGraphics.DrawLine(remotePen, x1, y1, x2, y2);
                                    }

                                    // Tối ưu UI: Chỉ làm mới vùng chữ nhật nhỏ cục bộ chứa nét vẽ tự do vừa nhận
                                    int x = Math.Min(x1, x2) - (int)thickness - 2;
                                    int y = Math.Min(y1, y2) - (int)thickness - 2;
                                    int w = Math.Abs(x1 - x2) + (int)thickness * 2 + 4;
                                    int h = Math.Abs(y1 - y2) + (int)thickness * 2 + 4;
                                    pnlCanvas.Invalidate(new Rectangle(x, y, w, h));
                                }
                            }
                        }
                        // ĐỌC CHUẨN CŨ (Lịch sử SQLite tàn dư)
                        else if (parts.Length >= 4 && parts[2] == "SHAPE")
                        {
                            string shapeType = parts[3];
                            int x1 = int.Parse(parts[4]);
                            int y1 = int.Parse(parts[5]);
                            int x2 = int.Parse(parts[6]);
                            int y2 = int.Parse(parts[7]);
                            Color netColor = parts[8].StartsWith("#") ? ColorTranslator.FromHtml(parts[8]) : Color.FromName(parts[8]);
                            float thickness = float.Parse(parts[9]);

                            DrawTool remoteTool = DrawTool.Line;
                            if (shapeType == "RECTANGLE") remoteTool = DrawTool.Rectangle;
                            else if (shapeType == "CIRCLE") remoteTool = DrawTool.Circle;

                            if (bitmapGraphics != null)
                            {
                                using (Pen remotePen = new Pen(netColor, thickness))
                                {
                                    DrawShapeGeneric(bitmapGraphics, remoteTool, new Point(x1, y1), new Point(x2, y2), remotePen);
                                }
                                pnlCanvas.Invalidate();
                            }
                        }
                        // ĐỌC CHUẨN CŨ (Lịch sử nét tự do SQLite)
                        else if (parts.Length >= 7)
                        {
                            int x1 = int.Parse(parts[1]);
                            int y1 = int.Parse(parts[2]);
                            int x2 = int.Parse(parts[3]);
                            int y2 = int.Parse(parts[4]);
                            Color netColor = parts[5].StartsWith("#") ? ColorTranslator.FromHtml(parts[5]) : Color.FromName(parts[5]);
                            float thickness = float.Parse(parts[6]);

                            if (bitmapGraphics != null)
                            {
                                using (Pen remotePen = new Pen(netColor, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                                {
                                    bitmapGraphics.DrawLine(remotePen, x1, y1, x2, y2);
                                }
                                pnlCanvas.Invalidate();
                            }
                        }
                    }
                    // 3. NHẬN LỆNH XÓA BẢNG
                    else if (parts[0] == "CLEAR_CANVAS")
                    {
                        if (bitmapGraphics != null)
                        {
                            bitmapGraphics.Clear(Color.White);
                            pnlCanvas.Invalidate();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi hiển thị dữ liệu: {ex.Message}");
                }
            });
        }

        private void CapNhatDanhSachThanhVien(string[] danhSachUsers)
        {
            flpOnlineUsers.SuspendLayout();
            flpOnlineUsers.Controls.Clear();

            Color[] mauAvatar = { Color.Crimson, Color.DodgerBlue, Color.ForestGreen, Color.Orange, Color.Purple };
            int count = 0;

            foreach (string user in danhSachUsers)
            {
                string tenHopLe = user.Trim();
                if (string.IsNullOrWhiteSpace(tenHopLe)) continue;

                Panel pnlDong = new Panel();
                pnlDong.Width = flpOnlineUsers.Width > 40 ? flpOnlineUsers.Width - 10 : 180;
                pnlDong.Height = 55;
                pnlDong.Margin = new Padding(3);
                pnlDong.BackColor = Color.FromArgb(235, 240, 255);

                Label lblAvatar = new Label();
                lblAvatar.Text = tenHopLe[0].ToString().ToUpper();
                lblAvatar.Size = new Size(32, 32);
                lblAvatar.Location = new Point(8, 11);
                lblAvatar.BackColor = mauAvatar[count % mauAvatar.Length];
                lblAvatar.ForeColor = Color.White;
                lblAvatar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                lblAvatar.TextAlign = ContentAlignment.MiddleCenter;
                pnlDong.Controls.Add(lblAvatar);

                Label lblName = new Label();
                lblName.Text = tenHopLe;
                lblName.Location = new Point(48, 10);
                lblName.AutoSize = true;
                lblName.ForeColor = Color.Black;
                lblName.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                pnlDong.Controls.Add(lblName);

                Label lblStatus = new Label();
                lblStatus.Text = "● Online";
                lblStatus.Location = new Point(48, 28);
                lblStatus.AutoSize = true;
                lblStatus.ForeColor = Color.LimeGreen;
                lblStatus.Font = new Font("Segoe UI", 8F);
                pnlDong.Controls.Add(lblStatus);

                flpOnlineUsers.Controls.Add(pnlDong);
                count++;
            }
            flpOnlineUsers.ResumeLayout();
        }

        // CÁC HÀM XỬ LÝ SỰ KIỆN CHUỘT VÀ ĐỒNG BỘ
        private void InitializepnlCanvas()
        {
            pnlCanvas.Dock = DockStyle.Fill;
            pnlCanvas.BackColor = Color.White;
            pnlCanvas.MouseDown += pnlCanvas_MouseDown;
            pnlCanvas.MouseMove += pnlCanvas_MouseMove;
            pnlCanvas.MouseUp += pnlCanvas_MouseUp;
            pnlCanvas.Paint += pnlCanvas_Paint;
            pnlCanvas.Resize += pnlCanvas_Resize;
            pnlCanvas.SendToBack();

            this.currentBrushColor = Color.Black;
            this.brushSize = 4f;

            if (flpOnlineUsers != null) flpOnlineUsers.BringToFront();
            if (btnClearAll != null) btnClearAll.BringToFront();
            if (btnSaveImage != null) btnSaveImage.BringToFront();

            typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                 ?.SetValue(pnlCanvas, true, null);
        }

        private void pnlCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastPoint = e.Location;
                lastSentPoint = e.Location; // Gán mốc điểm bắt đầu vẽ cục bộ phục vụ việc Throttle mạng

                if (currentTool == DrawTool.Pen || currentTool == DrawTool.Eraser)
                {
                    isDrawing = true; isDrawingShape = false;
                }
                else if (currentTool == DrawTool.Line || currentTool == DrawTool.Rectangle || currentTool == DrawTool.Circle)
                {
                    isDrawing = false; isDrawingShape = true;
                    shapeStartPoint = e.Location; shapeEndPoint = e.Location;
                }
            }
        }

        private void pnlCanvas_Resize(object sender, EventArgs e)
        {
            if (pnlCanvas.Width > 0 && pnlCanvas.Height > 0)
            {
                Bitmap oldBitmap = canvasBitmap;
                canvasBitmap = new Bitmap(pnlCanvas.Width, pnlCanvas.Height);
                bitmapGraphics = Graphics.FromImage(canvasBitmap);
                bitmapGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                bitmapGraphics.Clear(Color.White);
                if (oldBitmap != null)
                {
                    bitmapGraphics.DrawImage(oldBitmap, 0, 0);
                    oldBitmap.Dispose();
                }
                pnlCanvas.Invalidate();
            }
        }

        private void pnlCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && bitmapGraphics != null)
            {
                float currentSize = (currentTool == DrawTool.Eraser) ? (brushSize * 4) : brushSize;

                if (currentTool == DrawTool.Pen)
                {
                    using (Pen pen = new Pen(currentBrushColor, brushSize))
                    {
                        pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                        bitmapGraphics.DrawLine(pen, lastPoint, e.Location);
                    }
                }
                else if (currentTool == DrawTool.Eraser)
                {
                    using (Pen eraserPen = new Pen(Color.White, currentSize))
                    {
                        eraserPen.StartCap = LineCap.Round; eraserPen.EndCap = LineCap.Round;
                        bitmapGraphics.DrawLine(eraserPen, lastPoint, e.Location);
                    }
                }

                // --- GIẢI PHÁP THROTTLING MẠNG (Chống lag và dính gói tin) ---
                // Tính khoảng cách Euclid bình phương giữa điểm vừa vẽ và điểm đã gửi gần nhất
                int dx = e.X - lastSentPoint.X;
                int dy = e.Y - lastSentPoint.Y;
                if ((dx * dx + dy * dy) >= 16) // Khoảng cách thực tế lớn hơn hoặc bằng 4 pixel mới truyền gói tin đi
                {
                    string colorHex = (currentTool == DrawTool.Eraser) ? "#FFFFFF" : ColorTranslator.ToHtml(currentBrushColor);
                    // Bổ sung ký tự ngắt dòng \n vào cuối gói tin theo đúng mô tả kỹ thuật
                    NetworkClient.SendMessage($"DRAW;FREEHAND;{lastSentPoint.X};{lastSentPoint.Y};{e.X};{e.Y};{colorHex};{currentSize};{currentRoomId}\n");
                    lastSentPoint = e.Location; // Cập nhật lại mốc điểm gửi mạng gần nhất
                }

                // Tối ưu hóa UI: Chỉ làm mới khu vực nét vẽ vừa di chuyển qua
                int minX = Math.Min(lastPoint.X, e.X) - (int)currentSize - 2;
                int minY = Math.Min(lastPoint.Y, e.Y) - (int)currentSize - 2;
                int width = Math.Abs(lastPoint.X - e.X) + (int)currentSize * 2 + 4;
                int height = Math.Abs(lastPoint.Y - e.Y) + (int)currentSize * 2 + 4;

                lastPoint = e.Location;
                pnlCanvas.Invalidate(new Rectangle(minX, minY, width, height));
                return;
            }

            if (isDrawingShape && (currentTool == DrawTool.Line || currentTool == DrawTool.Rectangle || currentTool == DrawTool.Circle))
            {
                // Đối với nét đứt xem trước (Preview Shape): Bắt buộc xóa vùng cũ vẽ vùng mới để tránh rác màn hình
                pnlCanvas.Invalidate();
                shapeEndPoint = e.Location;
            }
        }

        private void pnlCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isDrawing && (currentTool == DrawTool.Pen || currentTool == DrawTool.Eraser))
                {
                    // Đảm bảo điểm cuối cùng của nét vẽ tự do luôn được đồng bộ khi nhấc chuột lên
                    string colorHex = (currentTool == DrawTool.Eraser) ? "#FFFFFF" : ColorTranslator.ToHtml(currentBrushColor);
                    float currentSize = (currentTool == DrawTool.Eraser) ? (brushSize * 4) : brushSize;
                    NetworkClient.SendMessage($"DRAW;FREEHAND;{lastSentPoint.X};{lastSentPoint.Y};{e.X};{e.Y};{colorHex};{currentSize};{currentRoomId}\n");
                }

                isDrawing = false;

                if (isDrawingShape && (currentTool == DrawTool.Line || currentTool == DrawTool.Rectangle || currentTool == DrawTool.Circle))
                {
                    isDrawingShape = false;
                    shapeEndPoint = e.Location;
                    if (bitmapGraphics != null)
                    {
                        using (Pen permanentPen = new Pen(currentBrushColor, brushSize))
                        {
                            DrawShapeGeneric(bitmapGraphics, currentTool, shapeStartPoint, shapeEndPoint, permanentPen);
                        }
                    }

                    string colorHex = ColorTranslator.ToHtml(currentBrushColor);
                    string toolName = currentTool.ToString().ToUpper();
                    // Thêm \n vào cuối gói tin gửi đi
                    NetworkClient.SendMessage($"DRAW;SHAPE;{toolName};{shapeStartPoint.X};{shapeStartPoint.Y};{shapeEndPoint.X};{shapeEndPoint.Y};{colorHex};{brushSize};{currentRoomId}\n");

                    pnlCanvas.Invalidate();
                }
                isDrawingShape = false;
            }
        }

        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (canvasBitmap != null) e.Graphics.DrawImage(canvasBitmap, 0, 0);
            if (isDrawingShape)
            {
                using (Pen previewPen = new Pen(currentBrushColor, brushSize))
                {
                    previewPen.DashStyle = DashStyle.Dash;
                    DrawShapeGeneric(e.Graphics, currentTool, shapeStartPoint, shapeEndPoint, previewPen);
                }
            }
        }

        private void DrawShapeGeneric(Graphics g, DrawTool tool, Point start, Point end, Pen pen)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (tool == DrawTool.Line) g.DrawLine(pen, start, end);
            else
            {
                int x = Math.Min(start.X, end.X);
                int y = Math.Min(start.Y, end.Y);
                int width = Math.Abs(start.X - end.X);
                int height = Math.Abs(start.Y - end.Y);
                if (width == 0 || height == 0) return;
                if (tool == DrawTool.Rectangle) g.DrawRectangle(pen, x, y, width, height);
                else if (tool == DrawTool.Circle) g.DrawEllipse(pen, x, y, width, height);
            }
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            if (bitmapGraphics != null) { bitmapGraphics.Clear(Color.White); }
            pnlCanvas.Invalidate();
            NetworkClient.SendMessage($"CLEAR_CANVAS;{currentRoomId}\n");
        }

        private void BtnSaveImage_Click(object sender, EventArgs e)
        {
            if (canvasBitmap == null) return;
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image (*.png)|*.png";
                saveFileDialog.FileName = "Whiteboard_Export.png";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    canvasBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show("Lưu ảnh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnPen_Click(object sender, EventArgs e) { currentTool = DrawTool.Pen; isDrawingShape = false; ResetButtonColors(); btnPen.BackColor = Color.LightBlue; }
        private void lblRoomName_Click(object sender, EventArgs e) { if (!string.IsNullOrEmpty(currentRoomId)) { Clipboard.SetText(currentRoomId); MessageBox.Show($"Đã copy phòng: {currentRoomId}"); } }
        private void lblUserInfo_Click(object sender, EventArgs e) { MessageBox.Show($"Tài khoản: {lblUserInfo.Text}", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void btnEraser_Click(object sender, EventArgs e) { currentTool = DrawTool.Eraser; isDrawingShape = false; ResetButtonColors(); btnEraser.BackColor = Color.LightBlue; }

        private void btnColor2_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = currentBrushColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    currentBrushColor = cd.Color; btnColor1.BackColor = currentBrushColor;
                    if (currentTool == DrawTool.Eraser) btnPen_Click(btnPen, EventArgs.Empty);
                }
            }
        }

        private void btnColor1_Click(object sender, EventArgs e) { btnPen_Click(btnPen, EventArgs.Empty); btnColor1.BackColor = currentBrushColor; }
        private void button1_Click(object sender, EventArgs e) { currentTool = DrawTool.Line; isDrawingShape = false; ResetButtonColors(); button1.BackColor = Color.LightBlue; }
        private void button2_Click(object sender, EventArgs e) { currentTool = DrawTool.Rectangle; isDrawingShape = false; ResetButtonColors(); button2.BackColor = Color.LightBlue; }
        private void button3_Click(object sender, EventArgs e) { currentTool = DrawTool.Circle; isDrawingShape = false; ResetButtonColors(); button3.BackColor = Color.LightBlue; }

        private void ResetButtonColors()
        {
            if (button1 != null) button1.BackColor = Color.Empty;
            if (button2 != null) button2.BackColor = Color.Empty;
            if (button3 != null) button3.BackColor = Color.Empty;
            if (btnPen != null) btnPen.BackColor = Color.Empty;
            if (btnEraser != null) btnEraser.BackColor = Color.Empty;
        }

        private void trackBrushSize_Scroll(object sender, EventArgs e) { brushSize = (float)trackBrushSize.Value; lblBrushSizeText.Text = "Brush Size: " + brushSize + "px"; }
        private void lblBrushSizeText_Click(object sender, EventArgs e) { brushSize = (float)trackBrushSize.Value; lblBrushSizeText.Text = "Brush Size: " + brushSize + "px"; }
        private void flpOnlineUsers_Paint(object sender, PaintEventArgs e) { }
    }
}