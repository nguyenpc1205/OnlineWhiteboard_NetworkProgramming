using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WhiteboardClient
{
    public partial class Form1 : Form
    {
        private bool isDrawing = false;
        private Point lastPoint;
        private Color currentBrushColor = Color.Black;
        private float brushSize = 3f;
        private bool isEraser = false;
        private Panel? canvasPanel;
        private Panel? leftPanel;
        private Panel? rightPanel;
        private Panel? topPanel;
        private Panel? statusPanel;
        private Label? statusLabel;
        private string currentRoomId;
        private string currentUserName;
        private TrackBar? brushSizeTrackBar;
        private Label? brushSizeLabel;

        public Form1(string userName = "Ẩn danh", string roomId = "ROOM001")
        {
            currentUserName = userName;
            currentRoomId = roomId;
            InitializeComponent();

            this.Text = $"Whiteboard Client - {userName} (Phòng: {roomId})";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Tạo menu bar
            CreateMenuBar();
            
            // Tạo top panel (thông tin phòng)
            CreateTopPanel();

            // Tạo left panel (Drawing Tools, Properties, Actions)
            CreateLeftPanel();

            // Tạo right panel (Online Users)
            CreateRightPanel();

            // Tạo canvas (vùng vẽ chính)
            CreateCanvasPanel();

            // Tạo status bar
            CreateStatusBar();

            // Đăng ký sự kiện mạng
            NetworkClient.OnDataReceived += HandleNetworkData;
        }

        private void CreateMenuBar()
        {
            MenuStrip menuStrip = new MenuStrip();

            // File Menu
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("Exit", null, (s, e) => this.Close());
            menuStrip.Items.Add(fileMenu);

            // Edit Menu
            ToolStripMenuItem editMenu = new ToolStripMenuItem("Edit");
            editMenu.DropDownItems.Add("Clear Canvas", null, (s, e) => ClearCanvasAndBroadcast());
            menuStrip.Items.Add(editMenu);

            // Room Menu
            ToolStripMenuItem roomMenu = new ToolStripMenuItem("Room");
            roomMenu.DropDownItems.Add($"Current: {currentRoomId}", null, null);
            menuStrip.Items.Add(roomMenu);

            // Help Menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");
            helpMenu.DropDownItems.Add("About", null, (s, e) => 
                MessageBox.Show("Online Whiteboard v1.0\nCollaborative Drawing Application", "About"));
            menuStrip.Items.Add(helpMenu);

            this.Controls.Add(menuStrip);
            menuStrip.Dock = DockStyle.Top;
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240),
                Dock = DockStyle.Top,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblRoom = new Label
            {
                Text = $"📍 Phòng: {currentRoomId}",
                AutoSize = true,
                Location = new Point(15, 15),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 100, 200)
            };

            Label lblUser = new Label
            {
                Text = $"👤 Người dùng: {currentUserName}",
                AutoSize = true,
                Location = new Point(250, 15),
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            topPanel.Controls.Add(lblRoom);
            topPanel.Controls.Add(lblUser);
            this.Controls.Add(topPanel);
        }

        private void CreateLeftPanel()
        {
            leftPanel = new Panel
            {
                Width = 200,
                BackColor = Color.FromArgb(250, 250, 250),
                Dock = DockStyle.Left,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            int yPos = 10;

            // === DRAWING TOOLS ===
            Label lblTools = new Label
            {
                Text = "🖌️ Drawing Tools",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true,
                ForeColor = Color.FromArgb(50, 100, 200)
            };
            leftPanel.Controls.Add(lblTools);
            yPos += 35;

            // Pen Button
            Button btnPen = new Button
            {
                Text = "✏️ Pen",
                Size = new Size(170, 35),
                Location = new Point(10, yPos),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 150, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnPen.Click += (s, e) =>
            {
                isEraser = false;
                btnPen.BackColor = Color.FromArgb(50, 100, 200);
                btnPen.ForeColor = Color.White;
            };
            leftPanel.Controls.Add(btnPen);
            yPos += 40;

            // Eraser Button
            Button btnEraser = new Button
            {
                Text = "🧹 Eraser",
                Size = new Size(170, 35),
                Location = new Point(10, yPos),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnEraser.Click += (s, e) =>
            {
                isEraser = true;
                btnEraser.BackColor = Color.FromArgb(255, 100, 100);
                btnEraser.ForeColor = Color.White;
            };
            leftPanel.Controls.Add(btnEraser);
            yPos += 50;

            // === PROPERTIES ===
            Label lblProperties = new Label
            {
                Text = "⚙️ Properties",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true,
                ForeColor = Color.FromArgb(50, 100, 200)
            };
            leftPanel.Controls.Add(lblProperties);
            yPos += 35;

            // Color Label
            Label lblColor = new Label
            {
                Text = "Màu sắc:",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            leftPanel.Controls.Add(lblColor);
            yPos += 25;

            // Color Picker Button
            Panel pnlColorPicker = new Panel
            {
                Size = new Size(170, 40),
                Location = new Point(10, yPos),
                BackColor = currentBrushColor,
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand
            };
            pnlColorPicker.Click += (s, e) =>
            {
                ColorDialog colorDialog = new ColorDialog();
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    currentBrushColor = colorDialog.Color;
                    pnlColorPicker.BackColor = currentBrushColor;
                }
            };
            leftPanel.Controls.Add(pnlColorPicker);
            yPos += 50;

            // Brush Size Label
            brushSizeLabel = new Label
            {
                Text = $"Kích thước: {brushSize:F1}px",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            leftPanel.Controls.Add(brushSizeLabel);
            yPos += 25;

            // Brush Size Slider
            brushSizeTrackBar = new TrackBar
            {
                Minimum = 1,
                Maximum = 20,
                Value = (int)brushSize,
                Size = new Size(170, 45),
                Location = new Point(10, yPos),
                TickStyle = TickStyle.BottomRight
            };
            brushSizeTrackBar.ValueChanged += (s, e) =>
            {
                brushSize = brushSizeTrackBar.Value;
                brushSizeLabel!.Text = $"Kích thước: {brushSize:F1}px";
            };
            leftPanel.Controls.Add(brushSizeTrackBar);
            yPos += 50;

            // === ACTIONS ===
            Label lblActions = new Label
            {
                Text = "📋 Actions",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true,
                ForeColor = Color.FromArgb(50, 100, 200)
            };
            leftPanel.Controls.Add(lblActions);
            yPos += 35;

            // Clear All Button
            Button btnClearAll = new Button
            {
                Text = "🗑️ Clear All",
                Size = new Size(170, 35),
                Location = new Point(10, yPos),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 150, 150),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnClearAll.Click += (s, e) => ClearCanvasAndBroadcast();
            leftPanel.Controls.Add(btnClearAll);
            yPos += 40;

            // Save Image Button
            Button btnSaveImage = new Button
            {
                Text = "💾 Save Image",
                Size = new Size(170, 35),
                Location = new Point(10, yPos),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 200, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnSaveImage.Click += (s, e) => SaveImage();
            leftPanel.Controls.Add(btnSaveImage);

            this.Controls.Add(leftPanel);
        }

        private void CreateRightPanel()
        {
            rightPanel = new Panel
            {
                Width = 200,
                BackColor = Color.FromArgb(240, 240, 240),
                Dock = DockStyle.Right,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblOnlineUsers = new Label
            {
                Text = "👥 Online Users",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true,
                ForeColor = Color.FromArgb(50, 100, 200)
            };
            rightPanel.Controls.Add(lblOnlineUsers);

            FlowLayoutPanel flpUsers = new FlowLayoutPanel
            {
                AutoScroll = true,
                Location = new Point(0, 40),
                Size = new Size(200, rightPanel.Height - 40),
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            // Mock users
            AddUserToPanel(flpUsers, "Nguyễn Văn An", Color.Crimson);
            AddUserToPanel(flpUsers, "Trần Thị Bình", Color.LimeGreen);
            AddUserToPanel(flpUsers, "Lê Minh Cường", Color.Blue);
            AddUserToPanel(flpUsers, "Phạm Thu Dung", Color.Magenta);

            rightPanel.Controls.Add(flpUsers);
            this.Controls.Add(rightPanel);
        }

        private void AddUserToPanel(FlowLayoutPanel flpUsers, string name, Color avatarColor)
        {
            Panel pnlUser = new Panel
            {
                Width = flpUsers.Width - 20,
                Height = 50,
                Margin = new Padding(0, 0, 0, 5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblAvatar = new Label
            {
                Text = name[0].ToString().ToUpper(),
                Size = new Size(30, 30),
                Location = new Point(5, 10),
                BackColor = avatarColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlUser.Controls.Add(lblAvatar);

            Label lblName = new Label
            {
                Text = name,
                Location = new Point(45, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };
            pnlUser.Controls.Add(lblName);

            Label lblStatus = new Label
            {
                Text = "● Online",
                Location = new Point(45, 25),
                AutoSize = true,
                ForeColor = Color.LimeGreen,
                Font = new Font("Segoe UI", 8F)
            };
            pnlUser.Controls.Add(lblStatus);

            flpUsers.Controls.Add(pnlUser);
        }

        private void CreateCanvasPanel()
        {
            canvasPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            canvasPanel.MouseDown += CanvasPanel_MouseDown;
            canvasPanel.MouseMove += CanvasPanel_MouseMove;
            canvasPanel.MouseUp += CanvasPanel_MouseUp;

            this.Controls.Add(canvasPanel);
        }

        private void CreateStatusBar()
        {
            statusPanel = new Panel
            {
                Height = 25,
                BackColor = Color.FromArgb(50, 50, 50),
                Dock = DockStyle.Bottom,
                BorderStyle = BorderStyle.FixedSingle
            };

            statusLabel = new Label
            {
                Text = $"🟢 Connected: 192.168.1.100:8080 | X: 0, Y: 0 | Tool: Pen | Color: #000000 | Size: 3px",
                ForeColor = Color.White,
                Location = new Point(5, 3),
                Font = new Font("Segoe UI", 8F),
                AutoSize = false,
                Size = new Size(statusPanel.Width - 10, statusPanel.Height - 6)
            };

            statusPanel.Controls.Add(statusLabel);
            this.Controls.Add(statusPanel);
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            try
            {
                if (NetworkClient.tcpClient != null && NetworkClient.tcpClient.Connected)
                {
                    StreamWriter writer = new StreamWriter(NetworkClient.tcpClient.GetStream()) { AutoFlush = true };
                    writer.WriteLine($"CONNECT;{currentUserName}");
                    UpdateStatus("Connected!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Connection Error: {ex.Message}");
            }
        }

        private void CanvasPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                lastPoint = e.Location;
            }
        }

        private void CanvasPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            if (isDrawing && canvasPanel != null)
            {
                using (Graphics g = canvasPanel.CreateGraphics())
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    Color activeColor = isEraser ? Color.White : currentBrushColor;

                    using (Pen drawingPen = new Pen(activeColor, brushSize))
                    {
                        drawingPen.StartCap = LineCap.Round;
                        drawingPen.EndCap = LineCap.Round;
                        g.DrawLine(drawingPen, lastPoint, e.Location);
                    }
                }

                NetworkClient.SendDrawData(lastPoint.X, lastPoint.Y, e.Location.X, e.Location.Y, currentBrushColor, brushSize, isEraser);
                lastPoint = e.Location;
                
                UpdateStatus($"X: {e.X}, Y: {e.Y}");
            }
        }

        private void CanvasPanel_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = false;
            }
        }

        private void HandleNetworkData(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(HandleNetworkData), message);
                return;
            }

            if (canvasPanel == null) return;

            try
            {
                string[] parts = message.Split(';');
                string command = parts[0];

                if (command == "DRAW" && parts.Length >= 4)
                {
                    string[] coords = parts[1].Split(',');
                    int x1 = int.Parse(coords[0]);
                    int y1 = int.Parse(coords[1]);
                    int x2 = int.Parse(coords[2]);
                    int y2 = int.Parse(coords[3]);

                    string colorInfo = parts[2];
                    float size = float.Parse(parts[3]);

                    Color drawColor;
                    if (colorInfo == "ERASE")
                    {
                        drawColor = Color.White;
                    }
                    else
                    {
                        string[] rgb = colorInfo.Split(',');
                        drawColor = Color.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));
                    }

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
                else if (command == "CLEAR_CANVAS" && parts.Length >= 2)
                {
                    string roomId = parts[1];
                    if (roomId == currentRoomId)
                    {
                        ClearCanvas();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi xử lý vẽ] {ex.Message}");
            }
        }

        private void ClearCanvasAndBroadcast()
        {
            ClearCanvas();
            NetworkClient.SendMessage($"CLEAR_CANVAS;{currentRoomId}");
            UpdateStatus("Canvas cleared");
        }

        private void ClearCanvas()
        {
            if (canvasPanel != null)
                canvasPanel.Invalidate();
        }

        private void SaveImage()
        {
            if (canvasPanel == null) return;

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(canvasPanel.Width, canvasPanel.Height);
            canvasPanel.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0, 0, canvasPanel.Width, canvasPanel.Height));

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image (*.png)|*.png";
                saveFileDialog.Title = "Chọn nơi lưu bức tranh";
                saveFileDialog.FileName = "Whiteboard_Export.png";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show("Lưu ảnh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStatus($"Image saved: {saveFileDialog.FileName}");
                }
            }
            bitmap.Dispose();
        }

        private void UpdateStatus(string message)
        {
            if (statusLabel != null)
            {
                string toolName = isEraser ? "Eraser" : "Pen";
                string colorHex = $"#{currentBrushColor.R:X2}{currentBrushColor.G:X2}{currentBrushColor.B:X2}";
                statusLabel.Text = $"🟢 {message} | Tool: {toolName} | Color: {colorHex} | Size: {brushSize:F0}px";
            }
        }
    }
}
