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
        private enum DrawTool { Pen, Line, Rectangle, Circle, Eraser }
        private DrawTool currentTool = DrawTool.Pen; // Mặc định ban đầu là bút vẽ tự do

        // Biến phục vụ tính năng kéo thả hình học
        private Point shapeStartPoint;
        private Point shapeEndPoint;
        private bool isDrawingShape = false;

        // Khai báo thêm các nút bấm chọn hình học hiển thị trên thanh công cụ
        private Button btnToolPen;
       
       

        // Bộ nhớ đệm đồ họa ẩn giúp hình vẽ mượt mà, không bị nhấp nháy màn hình
        private Bitmap canvasBitmap;
        private Graphics bitmapGraphics;

        private bool isDrawing = false;
        private Point lastPoint;
        private Color currentBrushColor = Color.Black;
        private float brushSize = 3f;
        private bool isEraser = false;
        private string currentRoomId = "";
        private string currentUserName = "";
        

        public Form1()
        {
            InitializeComponent();
            InitializepnlCanvas();
        }

        public Form1(string username, string roomID, string actionType)
        {
            IntPtr forceHandle = this.Handle; // Ép WinForms tạo Handle tránh lỗi đồng bộ luồng
            InitializeComponent();

            if (btnPen != null) btnPen.Click += btnPen_Click;
            if (btnEraser != null) btnEraser.Click += btnEraser_Click;
            if (button1 != null) button1.Click += button1_Click;
            if (button2 != null) button2.Click += button2_Click;
            if (button3 != null) button3.Click += button3_Click;
            // GÁN CHÍNH XÁC TÊN VÀ PHÒNG ĐƯỢC TRUYỀN TỪ LOGIN PHONG SANG
            this.currentUserName = username;
            this.currentRoomId = roomID;
            btnColor1.BackColor = Color.Black;
            lblRoomName.Text = $"PHÒNG VẼ: {roomID}"; // Gán mã phòng vào nhãn sẵn có
            lblUserInfo.Text = username;                // Gán tên người dùng vào nhãn sẵn có ở góc phải

            // Cấu hình nút Xóa Toàn Bộ
            btnClearAll.Click += BtnClearAll_Click;
            // Cấu hình nút Lưu Ảnh
            btnSaveImage.Click += BtnSaveImage_Click;

            btnClearAll.BringToFront();
            btnSaveImage.BringToFront();

            // Khởi tạo Panel vẽ hình
            InitializepnlCanvas();
            this.Load += Form1_Load;

            // Đăng ký nhận gói tin mạng
            NetworkClient.OnDataReceived += HandleNetworkData;

            // Gửi lệnh lên Server thông báo phân luồng cô lập phòng vẽ
            if (NetworkClient.tcpClient != null && NetworkClient.tcpClient.Connected)
            {
                try
                {
                    StreamWriter writer = new StreamWriter(NetworkClient.tcpClient.GetStream()) { AutoFlush = true };
                    if (actionType == "CREATE_ROOM")
                    {
                        writer.WriteLine($"CREATE_ROOM;{username};{roomID}");
                    }
                    else if (actionType == "JOIN_ROOM")
                    {
                        writer.WriteLine($"JOIN_ROOM;{username};{roomID}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Lỗi mạng] {ex.Message}");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (pnlCanvas.Width > 0 && pnlCanvas.Height > 0)
            {
                canvasBitmap = new Bitmap(pnlCanvas.Width, pnlCanvas.Height);
                bitmapGraphics = Graphics.FromImage(canvasBitmap);
                bitmapGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                bitmapGraphics.Clear(Color.White);

                trackBrushSize.Value = (int)brushSize; // Đưa thanh trượt về đúng mức size mặc định (4f)
                lblBrushSizeText.Text = brushSize.ToString();
                pnlCanvas.Invalidate();
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

        private void UpdateUserList(string[] users)
        {
            flpOnlineUsers.Controls.Clear();
            Color[] colors = { Color.Crimson, Color.Blue, Color.Green, Color.Orange, Color.Purple };
            int index = 0;

            foreach (string user in users)
            {
                if (!string.IsNullOrWhiteSpace(user))
                {
                    AddUser(user.Trim(), colors[index % colors.Length]);
                    index++;
                }
            }
        }

        // --- PHÂN HỆ 2: KHỞI TẠO VÀ XỬ LÝ SỰ KIỆN BẢNG VẼ ---
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
            this.isEraser = false;

            if (flpOnlineUsers != null) flpOnlineUsers.BringToFront();
            if (btnClearAll != null) btnClearAll.BringToFront();
            if (btnSaveImage != null) btnSaveImage.BringToFront();
            typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                 ?.SetValue(pnlCanvas, true, null);
        }

        private void SwitchTool(DrawTool tool, Button activeButton)
        {
            currentTool = tool;
            if (btnToolPen != null) btnToolPen.BackColor = Color.Empty;
            if (btnEraser != null) btnEraser.BackColor = Color.Empty;
            if (activeButton != null) activeButton.BackColor = Color.LightGray;
        }
        private void pnlCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastPoint = e.Location;

                // ÉP CHẶT TRẠNG THÁI: Nếu là Pen hoặc Eraser thì KHÔNG ĐƯỢC PHÉP vẽ hình học
                if (currentTool == DrawTool.Pen || currentTool == DrawTool.Eraser)
                {
                    isDrawing = true;
                    isDrawingShape = false;
                }
                // Nếu là công cụ hình học
                else if (currentTool == DrawTool.Line || currentTool == DrawTool.Rectangle || currentTool == DrawTool.Circle)
                {
                    isDrawing = false;
                    isDrawingShape = true;
                    shapeStartPoint = e.Location;
                    shapeEndPoint = e.Location; // Khởi tạo điểm kết thúc trùng điểm đầu
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
            // Nếu đang vẽ tự do (Pen/Eraser) thì xử lý xong rồi return luôn, không chạy xuống kiểm tra hình học
            if (isDrawing && bitmapGraphics != null)
            {
                if (currentTool == DrawTool.Pen)
                {
                    using (Pen pen = new Pen(currentBrushColor, brushSize))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        bitmapGraphics.DrawLine(pen, lastPoint, e.Location);
                    }
                }
                else if (currentTool == DrawTool.Eraser)
                {
                    using (Pen eraserPen = new Pen(Color.White, brushSize * 4))
                    {
                        eraserPen.StartCap = LineCap.Round;
                        eraserPen.EndCap = LineCap.Round;
                        bitmapGraphics.DrawLine(eraserPen, lastPoint, e.Location);
                    }
                }

                // Gửi dữ liệu mạng vẽ tự do
                string colorHex = (currentTool == DrawTool.Eraser) ? "#FFFFFF" : ColorTranslator.ToHtml(currentBrushColor);
                float currentSize = (currentTool == DrawTool.Eraser) ? (brushSize * 4) : brushSize;
                SendDrawData($"DRAW;{lastPoint.X};{lastPoint.Y};{e.X};{e.Y};{colorHex};{currentSize};{currentRoomId}");

                lastPoint = e.Location;
                pnlCanvas.Invalidate();
                return; // CHẶT ĐỨT LUỒNG TẠI ĐÂY, không cho chạy xuống kiểm tra shape nữa
            }

            // Chỉ vẽ hình học khi thực sự đang chọn công cụ hình học
            if (isDrawingShape && (currentTool == DrawTool.Line || currentTool == DrawTool.Rectangle || currentTool == DrawTool.Circle))
            {
                shapeEndPoint = e.Location;
                pnlCanvas.Invalidate();
            }
        }


        private void pnlCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = false;

                // Chỉ xử lý gửi hình học nếu biến tool thực sự thuộc nhóm hình học
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

                    // Đồng bộ hình học qua mạng internet
                    string colorHex = ColorTranslator.ToHtml(currentBrushColor);
                    string toolName = currentTool.ToString().ToUpper();
                    SendDrawData($"DRAW;{currentRoomId};SHAPE;{toolName};{shapeStartPoint.X};{shapeStartPoint.Y};{shapeEndPoint.X};{shapeEndPoint.Y};{colorHex};{brushSize}");

                    pnlCanvas.Invalidate();
                }

                // Đảm bảo dập tắt cờ dù có vẽ hay không
                isDrawingShape = false;
            }
        }

        // ⭐ ĐÃ SỬA: Hàm Paint hiển thị đệm Bitmap và vẽ bóng nét đứt xem trước hình học
        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (canvasBitmap != null)
            {
                e.Graphics.DrawImage(canvasBitmap, 0, 0);
            }

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
            if (tool == DrawTool.Line)
            {
                g.DrawLine(pen, start, end);
            }
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

        private void SendDrawData(string message)
        {
            if (NetworkClient.tcpClient != null && NetworkClient.tcpClient.Connected)
            {
                try
                {
                    StreamWriter writer = new StreamWriter(NetworkClient.tcpClient.GetStream(), System.Text.Encoding.UTF8) { AutoFlush = true };
                    writer.WriteLine(message);
                }
                catch (Exception ex) { Console.WriteLine($"[Lỗi mạng gửi dữ liệu vẽ] {ex.Message}"); }
            }
        }

        private void HandleNetworkData(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;

            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    // LOẠI BỎ CÁC KÝ TỰ BOM /ẨN/ RÁC Ở ĐẦU VÀ CUỐI CHUỖI NHẬN ĐƯỢC
                    string cleanData = data.Trim().Replace("\0", "");

                    // Nếu chuỗi có ký tự BOM ẩn của UTF-8, ta xóa nó đi
                    if (cleanData.StartsWith("\uFEFF"))
                    {
                        cleanData = cleanData.Substring(1);
                    }

                    string[] parts = cleanData.Split(';');
                    if (parts.Length == 0) return;

                    // Kiểm tra lệnh DRAW sau khi đã làm sạch chuỗi
                    if (parts[0] == "DRAW")
                    {
                        // TRƯỜNG HỢP 1: ĐỒNG BỘ HÌNH HỌC KÉO THẢ (Line, Rectangle, Circle)
                        // Dựa theo log Server: DRAW;ROOM71675;SHAPE;LINE;75;34;286;233;Black;4
                        if (parts.Length >= 4 && parts[2] == "SHAPE")
                        {
                            string shapeType = parts[3];
                            int x1 = int.Parse(parts[4]);
                            int y1 = int.Parse(parts[5]);
                            int x2 = int.Parse(parts[6]);
                            int y2 = int.Parse(parts[7]);
                            Color netColor = ColorTranslator.FromHtml(parts[8]);
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
                            }
                            pnlCanvas.Invalidate();
                        }
                        // TRƯỜNG HỢP 2: ĐỒNG BỘ NÉT VẼ TỰ DO / TẨY
                        // Dựa theo log Server: DRAW;428;194;426;197;Black;4;ROOM71675
                        else if (parts.Length >= 7)
                        {
                            int x1 = int.Parse(parts[1]);
                            int y1 = int.Parse(parts[2]);
                            int x2 = int.Parse(parts[3]);
                            int y2 = int.Parse(parts[4]);

                            // Chuyển đổi tên màu (như "Black", "Red") hoặc mã Hex sang cấu trúc Color
                            Color netColor;
                            if (parts[5].StartsWith("#"))
                                netColor = ColorTranslator.FromHtml(parts[5]);
                            else
                                netColor = Color.FromName(parts[5]);

                            float thickness = float.Parse(parts[6]);

                            if (bitmapGraphics != null)
                            {
                                using (Pen remotePen = new Pen(netColor, thickness))
                                {
                                    remotePen.StartCap = LineCap.Round;
                                    remotePen.EndCap = LineCap.Round;
                                    bitmapGraphics.DrawLine(remotePen, new Point(x1, y1), new Point(x2, y2));
                                }
                            }
                            pnlCanvas.Invalidate();
                        }
                    }
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
                    Console.WriteLine($"[Lỗi dựng hình mạng]: {ex.Message}");
                }
            });
        }


        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            if (bitmapGraphics != null)
            {
                bitmapGraphics.Clear(Color.White); // Làm trắng bitmap đệm ẩn
            }
            pnlCanvas.Invalidate(); // Ép vẽ lại giao diện trắng hoàn toàn

            if (NetworkClient.tcpClient != null && NetworkClient.tcpClient.Connected)
            {
                try
                {
                    StreamWriter writer = new StreamWriter(NetworkClient.tcpClient.GetStream(), System.Text.Encoding.UTF8) { AutoFlush = true };
                    writer.WriteLine($"CLEAR_CANVAS;{currentRoomId}");
                }
                catch (Exception ex) { Console.WriteLine($"[Lỗi xóa mạng]: {ex.Message}"); }
            }
        }

        private void BtnSaveImage_Click(object sender, EventArgs e)
        {
            if (canvasBitmap == null) return;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image (*.png)|*.png";
                saveFileDialog.Title = "Chọn nơi lưu bức tranh của bạn";
                saveFileDialog.FileName = "Whiteboard_Export.png";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    canvasBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show("Lưu ảnh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        // nút vẽ
        private void btnPen_Click(object sender, EventArgs e)
        {
            currentTool = DrawTool.Pen;
            isDrawingShape = false;

            ResetButtonColors();
            btnPen.BackColor = Color.LightBlue;
        }
        // mã phòng
        private void lblRoomName_Click(object sender, EventArgs e)
        {
            // Lấy ra mã phòng (Ví dụ tách lấy chữ "ROOM83109" từ chuỗi "PHÒNG VẼ: ROOM83109")
            if (!string.IsNullOrEmpty(currentRoomId))
            {
                Clipboard.SetText(currentRoomId); // Sao chép mã phòng vào bộ nhớ tạm
                MessageBox.Show($"Đã sao chép mã phòng: {currentRoomId} vào bộ nhớ đệm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lblUserInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Tài khoản: {lblUserInfo.Text}\nTrạng thái: Đang tham gia phòng vẽ", "Thông tin người dùng", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            currentTool = DrawTool.Eraser;
            isDrawingShape = false;

            ResetButtonColors();
            btnEraser.BackColor = Color.LightBlue; // Làm nổi bật nút Tẩy khi chọn
        }

        // 3. Nút chọn màu nâng cao (Bảng màu hệ thống)
        private void btnColor2_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = currentBrushColor;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    currentBrushColor = colorDialog.Color;
                    btnColor1.BackColor = currentBrushColor;

                    // ĐÃ SỬA: Thay vì gọi SwitchTool lỗi, ta kích hoạt trực tiếp sự kiện click của btnPen
                    if (currentTool == DrawTool.Eraser)
                    {
                        btnPen_Click(btnPen, EventArgs.Empty);
                    }
                }
            }
        }

        // 4. Nút hiển thị màu hiện tại
        private void btnColor1_Click(object sender, EventArgs e)
        {
            // ĐÃ SỬA: Khi bấm vào ô màu, tự động đưa công cụ về Pen chuẩn hóa thông qua hàm click của nó
            btnPen_Click(btnPen, EventArgs.Empty);
            btnColor1.BackColor = currentBrushColor;
        }

        // 5. Nút vẽ Đường thẳng
        private void button1_Click(object sender, EventArgs e)
        {
            currentTool = DrawTool.Line;
            isDrawingShape = false;
            ResetButtonColors();
            button1.BackColor = Color.LightBlue;
        }

        // 6. Nút vẽ Hình chữ nhật
        private void button2_Click(object sender, EventArgs e)
        {
            currentTool = DrawTool.Rectangle;
            isDrawingShape = false;
            ResetButtonColors();
            button2.BackColor = Color.LightBlue;
        }

        // 7. Nút vẽ Hình tròn
        private void button3_Click(object sender, EventArgs e)
        {
            currentTool = DrawTool.Circle;
            isDrawingShape = false;
            ResetButtonColors();
            button3.BackColor = Color.LightBlue;
        }

        // 8. Hàm Reset màu sắc (Đã giữ nguyên theo chuẩn Control cũ của bạn)
        private void ResetButtonColors()
        {
            // Sử dụng Color.Empty hoặc Color.FromArgb(240, 240, 240) để đưa nút về trạng thái xám ban đầu
            if (button1 != null) button1.BackColor = Color.Empty;
            if (button2 != null) button2.BackColor = Color.Empty;
            if (button3 != null) button3.BackColor = Color.Empty;

            if (btnPen != null) btnPen.BackColor = Color.Empty;
            if (btnEraser != null) btnEraser.BackColor = Color.Empty;
        }

        private void trackBrushSize_Scroll(object sender, EventArgs e)
        {
            brushSize = (float)trackBrushSize.Value;
        }

        private void lblBrushSizeText_Click(object sender, EventArgs e)
        {
            brushSize = (float)trackBrushSize.Value;

            lblBrushSizeText.Text = brushSize.ToString();
        }
    }
    }