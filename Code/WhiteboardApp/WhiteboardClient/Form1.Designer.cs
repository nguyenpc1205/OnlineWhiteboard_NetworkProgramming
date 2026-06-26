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
            flpOnlineUsers = new FlowLayoutPanel();
            
            SuspendLayout();

            // flpOnlineUsers
            flpOnlineUsers.AutoScroll = true;
            flpOnlineUsers.BackColor = Color.FromArgb(240, 240, 240);
            flpOnlineUsers.Dock = DockStyle.Right;
            flpOnlineUsers.Location = new Point(650, 0);
            flpOnlineUsers.Name = "flpOnlineUsers";
            flpOnlineUsers.Size = new Size(200, 600);
            flpOnlineUsers.TabIndex = 0;
            flpOnlineUsers.WrapContents = false;

            // Form1
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(850, 600);
            Controls.Add(flpOnlineUsers);
            Name = "Form1";
            Text = "Online Whiteboard";
            Load += Form1_Load;

            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flpOnlineUsers;
    }
}
