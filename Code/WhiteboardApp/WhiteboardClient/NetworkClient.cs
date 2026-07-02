using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WhiteboardClient
{
    public class NetworkClient
    {
        public static TcpClient tcpClient;
        static NetworkStream networkStream;
        static StreamWriter writer;
        static StreamReader reader;
        internal static object client;
        public static event Action<string> OnDataReceived;

        public static async Task StartClientAsync()
        {
            Console.WriteLine("=== Mạng Client - Kết nối tới 10.144.154.176:8888 ===");

            try
            {
                // 1. Khởi tạo TcpClient và kết nối tới Loopback
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync("127.0.0.1", 8888);
                networkStream = tcpClient.GetStream();
                writer = new StreamWriter(networkStream) { AutoFlush = true };
                reader = new StreamReader(networkStream);

                Console.WriteLine("[INFO] Đã kết nối thành công tới 10.144.154.176:8888\n");

                // 2. Luồng ngầm lắng nghe dữ liệu từ Server
                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (true)
                        {
                            string response = await reader.ReadLineAsync();
                            if (response == null) break; // Server đóng kết nối
                            OnDataReceived?.Invoke(response);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LỖI - Nhận] {ex.Message}");
                    }

                });

                // 3. Giữ cho kết nối luôn mở 
                while (tcpClient.Connected)
                {
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI] {ex.Message}");
            }
        }
        /// <summary>
        /// HÀM MỚI: Gửi dữ liệu nét vẽ thật từ chuột và trạng thái cục tẩy lên mạng
        /// </summary>
       
        public static void SendDrawData(int x1, int y1, int x2, int y2, Color color, float size, bool isEraser)
        {
            if (tcpClient == null || !tcpClient.Connected || writer == null) return;

            try
            {
                // Kiểm tra xem có đang bật cục tẩy hay không
                string colorStr = isEraser ? "ERASE" : $"{color.R},{color.G},{color.B}";

                // Đóng gói chuỗi gửi mạng
                string packet = $"DRAW;{x1},{y1},{x2},{y2};{colorStr};{size}";

                writer.WriteLine(packet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi gửi mạng] {ex.Message}");
            }
        }
        public static void SendMessage(string message)
        {
            if (tcpClient == null || !tcpClient.Connected || writer == null)
                return;

            try
            {
                writer.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi gửi mạng] {ex.Message}");
            }
        }
    }
}