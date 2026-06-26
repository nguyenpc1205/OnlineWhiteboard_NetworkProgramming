namespace WhiteboardClient
{
    partial class FormRoomSelection
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
            lblTitle = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            lblUser = new Label();
            lblRoom = new Label();
            txtRoomCode = new TextBox();
            btnCreateRoom = new Button();
            btnJoinRoom = new Button();

            SuspendLayout();

            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.Location = new Point(220, 50);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(340, 41);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "ONLINE WHITEBOARD";

            // label2
            label2.AutoSize = true;
            label2.Location = new Point(0, 0);
            label2.Name = "label2";
            label2.Size = new Size(0, 20);
            label2.TabIndex = 1;

            // textBox1 (Tên người dùng)
            textBox1.Location = new Point(300, 150);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(220, 27);
            textBox1.TabIndex = 2;

            // lblUser
            lblUser.AutoSize = true;
            lblUser.Location = new Point(150, 153);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(117, 20);
            lblUser.TabIndex = 3;
            lblUser.Text = "Tên người dùng";

            // lblRoom
            lblRoom.AutoSize = true;
            lblRoom.Location = new Point(150, 210);
            lblRoom.Name = "lblRoom";
            lblRoom.Size = new Size(77, 20);
            lblRoom.TabIndex = 4;
            lblRoom.Text = "Mã phòng";

            // txtRoomCode
            txtRoomCode.Location = new Point(300, 207);
            txtRoomCode.Name = "txtRoomCode";
            txtRoomCode.Size = new Size(220, 27);
            txtRoomCode.TabIndex = 5;

            // btnCreateRoom
            btnCreateRoom.Location = new Point(250, 280);
            btnCreateRoom.Name = "btnCreateRoom";
            btnCreateRoom.Size = new Size(130, 40);
            btnCreateRoom.TabIndex = 6;
            btnCreateRoom.Text = "Tạo phòng";
            btnCreateRoom.UseVisualStyleBackColor = true;
            btnCreateRoom.Click += btnCreateRoom_Click;

            // btnJoinRoom
            btnJoinRoom.Location = new Point(400, 280);
            btnJoinRoom.Name = "btnJoinRoom";
            btnJoinRoom.Size = new Size(130, 40);
            btnJoinRoom.TabIndex = 7;
            btnJoinRoom.Text = "Vào phòng";
            btnJoinRoom.UseVisualStyleBackColor = true;
            btnJoinRoom.Click += btnJoinRoom_Click;

            // FormRoomSelection
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);

            Controls.Add(btnJoinRoom);
            Controls.Add(btnCreateRoom);
            Controls.Add(txtRoomCode);
            Controls.Add(lblRoom);
            Controls.Add(lblUser);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(lblTitle);

            Name = "FormRoomSelection";
            Text = "Room Selection";
            Load += FormRoomSelection_Load;

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label label2;
        private TextBox textBox1;
        private Label lblUser;
        private Label lblRoom;
        private TextBox txtRoomCode;
        private Button btnCreateRoom;
        private Button btnJoinRoom;
    }
}
