using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

class NetworkClient
{
    static TcpClient tcpClient;
    static NetworkStream networkStream;
    static StreamWriter writer;
    static StreamReader reader;
    static Random random = new Random();

    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Mạng Client - Kết nối tới 127.0.0.1:8888 ===");

        try
        {
            // 1. Khởi tạo TcpClient và kết nối tới Loopback
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("127.0.0.1", 8888);
            networkStream = tcpClient.GetStream();
            writer = new StreamWriter(networkStream) { AutoFlush = true };
            reader = new StreamReader(networkStream);

            Console.WriteLine("[INFO] Đã kết nối thành công tới 127.0.0.1:8888\n");

            // 2. Luồng ngầm lắng nghe dữ liệu từ Server
            _ = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        string response = await reader.ReadLineAsync();
                        if (response == null) break; // Server đóng kết nối
                        Console.WriteLine($"[SERVER] {response}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LỖI - Nhận] {ex.Message}");
                }
            });

            // 3. Vòng lặp vô hạn gửi tọa độ ngẫu nhiên mỗi 1 giây
            while (true)
            {
                string drawCommand = GenerateDrawCommand();
                await writer.WriteLineAsync(drawCommand);
                Console.WriteLine($"[GỬI]    {drawCommand}");
                await Task.Delay(1000); // Đợi 1 giây
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LỖI] {ex.Message}");
        }
        finally
        {
            writer?.Close();
            reader?.Close();
            networkStream?.Close();
            tcpClient?.Close();
            Console.WriteLine("[INFO] Đã đóng kết nối.");
        }
    }

    /// <summary>
    /// Tạo chuỗi lệnh DRAW với tọa độ ngẫu nhiên theo cú pháp:
    /// DRAW;x1,y1,x2,y2;R,G,B;thickness
    /// </summary>
    static string GenerateDrawCommand()
    {
        int x1 = random.Next(0, 1920);
        int y1 = random.Next(0, 1080);
        int x2 = random.Next(0, 1920);
        int y2 = random.Next(0, 1080);

        int r = random.Next(0, 256);
        int g = random.Next(0, 256);
        int b = random.Next(0, 256);

        int thickness = random.Next(1, 11); // Độ dày nét từ 1 đến 10

        return $"DRAW;{x1},{y1},{x2},{y2};{r},{g},{b};{thickness}";
    }
}

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
