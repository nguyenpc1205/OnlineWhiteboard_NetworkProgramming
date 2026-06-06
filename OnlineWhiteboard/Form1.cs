using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnlineWhiteboard
{
    public partial class Form1 : Form
    {
        private bool isDrawing = false;
        private Point lastPoint;
        private Color currentBrushColor = Color.Black;
        private float brushSize = 3f;
        private bool isEraser = false;

        private Panel canvasPanel;

        public Form1()
        {
            InitializeComponent();
            InitializeCanvasPanel(); // Bắt buộc phải có dòng này để tạo vùng vẽ
        }

        private void InitializeCanvasPanel()
        {
            canvasPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Gắn 3 sự kiện chuột vào panel
            canvasPanel.MouseDown += CanvasPanel_MouseDown;
            canvasPanel.MouseMove += CanvasPanel_MouseMove;
            canvasPanel.MouseUp += CanvasPanel_MouseUp;

            this.Controls.Add(canvasPanel);
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
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    Color activeColor = isEraser ? Color.White : currentBrushColor;
                    using (Pen drawingPen = new Pen(activeColor, brushSize))
                    {
                        drawingPen.StartCap = LineCap.Round;
                        drawingPen.EndCap = LineCap.Round;
                        g.DrawLine(drawingPen, lastPoint, e.Location);
                    }
                }
                lastPoint = e.Location;
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