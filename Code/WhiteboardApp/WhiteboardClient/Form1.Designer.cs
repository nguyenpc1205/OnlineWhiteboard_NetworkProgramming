namespace WhiteboardClient
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblConnectionStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCoordinates = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTool = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblColor = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBrushSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlTopToolbar = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.pnlUser = new System.Windows.Forms.Panel();
            this.lblUserInfo = new System.Windows.Forms.Label();
            this.lblRoomName = new System.Windows.Forms.Label();
            this.panelDivider = new System.Windows.Forms.Panel();
            this.btnJoinRoom = new System.Windows.Forms.Button();
            this.btnNewRoom = new System.Windows.Forms.Button();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlCanvasContainer = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlCanvas = new System.Windows.Forms.Panel();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.flpOnlineUsers = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlLeftTools = new System.Windows.Forms.Panel();
            this.shape = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.grpProperties = new System.Windows.Forms.GroupBox();
            this.btnColor1 = new System.Windows.Forms.Button();
            this.btnColor2 = new System.Windows.Forms.Button();
            this.lblBrushSizeText = new System.Windows.Forms.Label();
            this.trackBrushSize = new System.Windows.Forms.TrackBar();
            this.lblColorLabel = new System.Windows.Forms.Label();
            this.grpDrawingTools = new System.Windows.Forms.GroupBox();
            this.btnEraser = new System.Windows.Forms.Button();
            this.btnPen = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.pnlTopToolbar.SuspendLayout();
            this.pnlUser.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlCanvasContainer.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlLeftTools.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBrushSize)).BeginInit();
            this.grpDrawingTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.fileToolStripMenuItem,
            this.roomToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1551, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(49, 24);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // roomToolStripMenuItem
            // 
            this.roomToolStripMenuItem.Name = "roomToolStripMenuItem";
            this.roomToolStripMenuItem.Size = new System.Drawing.Size(63, 24);
            this.roomToolStripMenuItem.Text = "Room";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblConnectionStatus,
            this.lblCoordinates,
            this.lblTool,
            this.lblColor,
            this.lblBrushSize});
            this.statusStrip1.Location = new System.Drawing.Point(0, 735);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1551, 26);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblConnectionStatus.Image = ((System.Drawing.Image)(resources.GetObject("lblConnectionStatus.Image")));
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(256, 20);
            this.lblConnectionStatus.Text = "📶 Connected: 192.168.1.100:8080";
            // 
            // lblCoordinates
            // 
            this.lblCoordinates.Margin = new System.Windows.Forms.Padding(20, 3, 0, 2);
            this.lblCoordinates.Name = "lblCoordinates";
            this.lblCoordinates.Size = new System.Drawing.Size(87, 21);
            this.lblCoordinates.Text = "X: 181, Y: 14";
            // 
            // lblTool
            // 
            this.lblTool.Margin = new System.Windows.Forms.Padding(20, 3, 0, 2);
            this.lblTool.Name = "lblTool";
            this.lblTool.Size = new System.Drawing.Size(68, 21);
            this.lblTool.Text = "Tool: Pen";
            // 
            // lblColor
            // 
            this.lblColor.Margin = new System.Windows.Forms.Padding(20, 3, 0, 2);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(107, 21);
            this.lblColor.Text = "Color: #FF0000";
            // 
            // lblBrushSize
            // 
            this.lblBrushSize.Margin = new System.Windows.Forms.Padding(20, 3, 0, 2);
            this.lblBrushSize.Name = "lblBrushSize";
            this.lblBrushSize.Size = new System.Drawing.Size(67, 21);
            this.lblBrushSize.Text = "Size: 3px";
            // 
            // pnlTopToolbar
            // 
            this.pnlTopToolbar.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlTopToolbar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTopToolbar.Controls.Add(this.btnLogout);
            this.pnlTopToolbar.Controls.Add(this.pnlUser);
            this.pnlTopToolbar.Controls.Add(this.lblRoomName);
            this.pnlTopToolbar.Controls.Add(this.panelDivider);
            this.pnlTopToolbar.Controls.Add(this.btnJoinRoom);
            this.pnlTopToolbar.Controls.Add(this.btnNewRoom);
            this.pnlTopToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopToolbar.Location = new System.Drawing.Point(0, 28);
            this.pnlTopToolbar.Name = "pnlTopToolbar";
            this.pnlTopToolbar.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.pnlTopToolbar.Size = new System.Drawing.Size(1551, 45);
            this.pnlTopToolbar.TabIndex = 2;
            // 
            // btnLogout
            // 
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnLogout.Location = new System.Drawing.Point(1324, 8);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 27);
            this.btnLogout.TabIndex = 6;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            // 
            // pnlUser
            // 
            this.pnlUser.Controls.Add(this.lblUserInfo);
            this.pnlUser.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlUser.Location = new System.Drawing.Point(1399, 8);
            this.pnlUser.Name = "pnlUser";
            this.pnlUser.Size = new System.Drawing.Size(140, 27);
            this.pnlUser.TabIndex = 5;
            // 
            // lblUserInfo
            // 
            this.lblUserInfo.AutoSize = true;
            this.lblUserInfo.Location = new System.Drawing.Point(34, 6);
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Size = new System.Drawing.Size(110, 20);
            this.lblUserInfo.TabIndex = 1;
            this.lblUserInfo.Text = "Nguyễn Văn An";
            this.lblUserInfo.Click += new System.EventHandler(this.lblUserInfo_Click);
            // 
            // lblRoomName
            // 
            this.lblRoomName.AutoSize = true;
            this.lblRoomName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblRoomName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRoomName.ForeColor = System.Drawing.Color.DimGray;
            this.lblRoomName.Location = new System.Drawing.Point(180, 8);
            this.lblRoomName.Name = "lblRoomName";
            this.lblRoomName.Padding = new System.Windows.Forms.Padding(10, 5, 0, 0);
            this.lblRoomName.Size = new System.Drawing.Size(180, 25);
            this.lblRoomName.TabIndex = 4;
            this.lblRoomName.Text = "Room: Phòng Vẽ Chính";
            this.lblRoomName.Click += new System.EventHandler(this.lblRoomName_Click);
            // 
            // panelDivider
            // 
            this.panelDivider.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelDivider.Location = new System.Drawing.Point(170, 8);
            this.panelDivider.Name = "panelDivider";
            this.panelDivider.Size = new System.Drawing.Size(10, 27);
            this.panelDivider.TabIndex = 3;
            // 
            // btnJoinRoom
            // 
            this.btnJoinRoom.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnJoinRoom.Location = new System.Drawing.Point(90, 8);
            this.btnJoinRoom.Name = "btnJoinRoom";
            this.btnJoinRoom.Size = new System.Drawing.Size(80, 27);
            this.btnJoinRoom.TabIndex = 1;
            this.btnJoinRoom.Text = "Join Room";
            this.btnJoinRoom.UseVisualStyleBackColor = true;
            // 
            // btnNewRoom
            // 
            this.btnNewRoom.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnNewRoom.Location = new System.Drawing.Point(10, 8);
            this.btnNewRoom.Name = "btnNewRoom";
            this.btnNewRoom.Size = new System.Drawing.Size(80, 27);
            this.btnNewRoom.TabIndex = 0;
            this.btnNewRoom.Text = "New Room";
            this.btnNewRoom.UseVisualStyleBackColor = true;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.pnlCanvasContainer);
            this.pnlMain.Controls.Add(this.pnlRight);
            this.pnlMain.Controls.Add(this.pnlLeftTools);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 73);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(10);
            this.pnlMain.Size = new System.Drawing.Size(1551, 662);
            this.pnlMain.TabIndex = 3;
            // 
            // pnlCanvasContainer
            // 
            this.pnlCanvasContainer.Controls.Add(this.flowLayoutPanel1);
            this.pnlCanvasContainer.Controls.Add(this.pnlCanvas);
            this.pnlCanvasContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCanvasContainer.Location = new System.Drawing.Point(150, 10);
            this.pnlCanvasContainer.Name = "pnlCanvasContainer";
            this.pnlCanvasContainer.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.pnlCanvasContainer.Size = new System.Drawing.Size(1181, 642);
            this.pnlCanvasContainer.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(1181, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(203, 634);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // pnlCanvas
            // 
            this.pnlCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlCanvas.BackColor = System.Drawing.Color.White;
            this.pnlCanvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCanvas.Location = new System.Drawing.Point(0, 0);
            this.pnlCanvas.Name = "pnlCanvas";
            this.pnlCanvas.Size = new System.Drawing.Size(1175, 640);
            this.pnlCanvas.TabIndex = 0;
            this.pnlCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlCanvas_Paint);
            // 
            // pnlRight
            // 
            this.pnlRight.Controls.Add(this.flpOnlineUsers);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(1331, 10);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(210, 642);
            this.pnlRight.TabIndex = 1;
            // 
            // flpOnlineUsers
            // 
            this.flpOnlineUsers.AutoScroll = true;
            this.flpOnlineUsers.BackColor = System.Drawing.Color.White;
            this.flpOnlineUsers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpOnlineUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpOnlineUsers.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpOnlineUsers.Location = new System.Drawing.Point(0, 0);
            this.flpOnlineUsers.Name = "flpOnlineUsers";
            this.flpOnlineUsers.Padding = new System.Windows.Forms.Padding(5);
            this.flpOnlineUsers.Size = new System.Drawing.Size(210, 642);
            this.flpOnlineUsers.TabIndex = 0;
            this.flpOnlineUsers.WrapContents = false;
            this.flpOnlineUsers.Paint += new System.Windows.Forms.PaintEventHandler(this.flpOnlineUsers_Paint);
            // 
            // pnlLeftTools
            // 
            this.pnlLeftTools.Controls.Add(this.shape);
            this.pnlLeftTools.Controls.Add(this.button3);
            this.pnlLeftTools.Controls.Add(this.button2);
            this.pnlLeftTools.Controls.Add(this.button1);
            this.pnlLeftTools.Controls.Add(this.grpActions);
            this.pnlLeftTools.Controls.Add(this.grpProperties);
            this.pnlLeftTools.Controls.Add(this.grpDrawingTools);
            this.pnlLeftTools.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeftTools.Location = new System.Drawing.Point(10, 10);
            this.pnlLeftTools.Name = "pnlLeftTools";
            this.pnlLeftTools.Size = new System.Drawing.Size(140, 642);
            this.pnlLeftTools.TabIndex = 0;
            // 
            // shape
            // 
            this.shape.AutoSize = true;
            this.shape.Location = new System.Drawing.Point(38, 381);
            this.shape.Name = "shape";
            this.shape.Size = new System.Drawing.Size(50, 20);
            this.shape.TabIndex = 0;
            this.shape.Text = "Shape\r\n";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(10, 510);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(120, 40);
            this.button3.TabIndex = 5;
            this.button3.Text = "⭕ Tròn";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(10, 464);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 40);
            this.button2.TabIndex = 4;
            this.button2.Text = "🔲 Vuông";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 413);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 45);
            this.button1.TabIndex = 3;
            this.button1.Text = "🖋️ Thẳng";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnSaveImage);
            this.grpActions.Controls.Add(this.btnClearAll);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpActions.ForeColor = System.Drawing.Color.DimGray;
            this.grpActions.Location = new System.Drawing.Point(0, 255);
            this.grpActions.Name = "grpActions";
            this.grpActions.Padding = new System.Windows.Forms.Padding(10);
            this.grpActions.Size = new System.Drawing.Size(140, 123);
            this.grpActions.TabIndex = 2;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // btnSaveImage
            // 
            this.btnSaveImage.ForeColor = System.Drawing.Color.Black;
            this.btnSaveImage.Location = new System.Drawing.Point(7, 33);
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.Size = new System.Drawing.Size(120, 30);
            this.btnSaveImage.TabIndex = 1;
            this.btnSaveImage.Text = "📥 Save Image";
            this.btnSaveImage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSaveImage.UseVisualStyleBackColor = true;
            // 
            // btnClearAll
            // 
            this.btnClearAll.ForeColor = System.Drawing.Color.Black;
            this.btnClearAll.Location = new System.Drawing.Point(10, 66);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(120, 30);
            this.btnClearAll.TabIndex = 0;
            this.btnClearAll.Text = "🗑 Clear All";
            this.btnClearAll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClearAll.UseVisualStyleBackColor = true;
            // 
            // grpProperties
            // 
            this.grpProperties.Controls.Add(this.btnColor1);
            this.grpProperties.Controls.Add(this.btnColor2);
            this.grpProperties.Controls.Add(this.lblBrushSizeText);
            this.grpProperties.Controls.Add(this.trackBrushSize);
            this.grpProperties.Controls.Add(this.lblColorLabel);
            this.grpProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpProperties.ForeColor = System.Drawing.Color.DimGray;
            this.grpProperties.Location = new System.Drawing.Point(0, 115);
            this.grpProperties.Name = "grpProperties";
            this.grpProperties.Padding = new System.Windows.Forms.Padding(10);
            this.grpProperties.Size = new System.Drawing.Size(140, 140);
            this.grpProperties.TabIndex = 1;
            this.grpProperties.TabStop = false;
            this.grpProperties.Text = "Properties";
            // 
            // btnColor1
            // 
            this.btnColor1.BackColor = System.Drawing.Color.Red;
            this.btnColor1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor1.Location = new System.Drawing.Point(99, 46);
            this.btnColor1.Name = "btnColor1";
            this.btnColor1.Size = new System.Drawing.Size(35, 25);
            this.btnColor1.TabIndex = 1;
            this.btnColor1.UseVisualStyleBackColor = false;
            this.btnColor1.Click += new System.EventHandler(this.btnColor1_Click);
            // 
            // btnColor2
            // 
            this.btnColor2.BackColor = System.Drawing.Color.Red;
            this.btnColor2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor2.Location = new System.Drawing.Point(10, 46);
            this.btnColor2.Name = "btnColor2";
            this.btnColor2.Size = new System.Drawing.Size(78, 25);
            this.btnColor2.TabIndex = 2;
            this.btnColor2.UseVisualStyleBackColor = false;
            this.btnColor2.Click += new System.EventHandler(this.btnColor2_Click);
            // 
            // lblBrushSizeText
            // 
            this.lblBrushSizeText.AutoSize = true;
            this.lblBrushSizeText.BackColor = System.Drawing.Color.CornflowerBlue;
            this.lblBrushSizeText.ForeColor = System.Drawing.Color.White;
            this.lblBrushSizeText.Location = new System.Drawing.Point(10, 80);
            this.lblBrushSizeText.Name = "lblBrushSizeText";
            this.lblBrushSizeText.Padding = new System.Windows.Forms.Padding(2);
            this.lblBrushSizeText.Size = new System.Drawing.Size(111, 24);
            this.lblBrushSizeText.TabIndex = 4;
            this.lblBrushSizeText.Text = "Brush Size: 3px";
            this.lblBrushSizeText.Click += new System.EventHandler(this.lblBrushSizeText_Click);
            // 
            // trackBrushSize
            // 
            this.trackBrushSize.AutoSize = false;
            this.trackBrushSize.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.trackBrushSize.Location = new System.Drawing.Point(10, 105);
            this.trackBrushSize.Maximum = 20;
            this.trackBrushSize.Minimum = 1;
            this.trackBrushSize.Name = "trackBrushSize";
            this.trackBrushSize.Size = new System.Drawing.Size(120, 25);
            this.trackBrushSize.TabIndex = 3;
            this.trackBrushSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBrushSize.Value = 3;
            this.trackBrushSize.Scroll += new System.EventHandler(this.trackBrushSize_Scroll);
            // 
            // lblColorLabel
            // 
            this.lblColorLabel.AutoSize = true;
            this.lblColorLabel.ForeColor = System.Drawing.Color.Black;
            this.lblColorLabel.Location = new System.Drawing.Point(10, 23);
            this.lblColorLabel.Name = "lblColorLabel";
            this.lblColorLabel.Size = new System.Drawing.Size(48, 20);
            this.lblColorLabel.TabIndex = 0;
            this.lblColorLabel.Text = "Color:";
            // 
            // grpDrawingTools
            // 
            this.grpDrawingTools.Controls.Add(this.btnEraser);
            this.grpDrawingTools.Controls.Add(this.btnPen);
            this.grpDrawingTools.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpDrawingTools.ForeColor = System.Drawing.Color.DimGray;
            this.grpDrawingTools.Location = new System.Drawing.Point(0, 0);
            this.grpDrawingTools.Name = "grpDrawingTools";
            this.grpDrawingTools.Padding = new System.Windows.Forms.Padding(10);
            this.grpDrawingTools.Size = new System.Drawing.Size(140, 115);
            this.grpDrawingTools.TabIndex = 0;
            this.grpDrawingTools.TabStop = false;
            this.grpDrawingTools.Text = "Drawing Tools";
            // 
            // btnEraser
            // 
            this.btnEraser.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEraser.ForeColor = System.Drawing.Color.Black;
            this.btnEraser.Location = new System.Drawing.Point(10, 70);
            this.btnEraser.Name = "btnEraser";
            this.btnEraser.Size = new System.Drawing.Size(120, 40);
            this.btnEraser.TabIndex = 1;
            this.btnEraser.Text = "⌫ Eraser";
            this.btnEraser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEraser.UseVisualStyleBackColor = true;
            this.btnEraser.Click += new System.EventHandler(this.btnEraser_Click);
            // 
            // btnPen
            // 
            this.btnPen.BackColor = System.Drawing.Color.White;
            this.btnPen.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPen.ForeColor = System.Drawing.Color.Black;
            this.btnPen.Location = new System.Drawing.Point(10, 30);
            this.btnPen.Name = "btnPen";
            this.btnPen.Size = new System.Drawing.Size(120, 40);
            this.btnPen.TabIndex = 0;
            this.btnPen.Text = "✎ Pen";
            this.btnPen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPen.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1551, 761);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlTopToolbar);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Whiteboard Client";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pnlTopToolbar.ResumeLayout(false);
            this.pnlTopToolbar.PerformLayout();
            this.pnlUser.ResumeLayout(false);
            this.pnlUser.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlCanvasContainer.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlLeftTools.ResumeLayout(false);
            this.pnlLeftTools.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpProperties.ResumeLayout(false);
            this.grpProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBrushSize)).EndInit();
            this.grpDrawingTools.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem roomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblConnectionStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblCoordinates;
        private System.Windows.Forms.ToolStripStatusLabel lblTool;
        private System.Windows.Forms.ToolStripStatusLabel lblColor;
        private System.Windows.Forms.ToolStripStatusLabel lblBrushSize;
        private System.Windows.Forms.Panel pnlTopToolbar;
        private System.Windows.Forms.Button btnNewRoom;
        private System.Windows.Forms.Button btnJoinRoom;
        private System.Windows.Forms.Panel panelDivider;
        private System.Windows.Forms.Label lblRoomName;
        private System.Windows.Forms.Panel pnlUser;
        private System.Windows.Forms.Label lblUserInfo;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlLeftTools;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.FlowLayoutPanel flpOnlineUsers;
        private System.Windows.Forms.Panel pnlCanvasContainer;
        private System.Windows.Forms.Panel pnlCanvas;
        private System.Windows.Forms.GroupBox grpDrawingTools;
        private System.Windows.Forms.Button btnPen;
        private System.Windows.Forms.Button btnEraser;
        private System.Windows.Forms.GroupBox grpProperties;
        private System.Windows.Forms.Label lblColorLabel;
        private System.Windows.Forms.Button btnColor1;
        private System.Windows.Forms.Button btnColor2;
        private System.Windows.Forms.TrackBar trackBrushSize;
        private System.Windows.Forms.Label lblBrushSizeText;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Button btnSaveImage;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label shape;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}