using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WhiteboardClient
{
    public class NetworkClient
    {
        public static TcpClient tcpClient;
        static NetworkStream networkStream;
        static StreamWriter writer;
        static StreamReader reader;
        public static event Action<string> OnDataReceived;

        public static async Task StartClientAsync()
        {
            Console.WriteLine("=== Mạng Client - Đang kết nối ===");
            try
            {
                tcpClient = new TcpClient();
                // CHÚ Ý: Đảm bảo IP này đúng với IP Server của bạn
                await tcpClient.ConnectAsync("127.0.0.1", 8888);
                networkStream = tcpClient.GetStream();

                writer = new StreamWriter(networkStream, new UTF8Encoding(false)) { AutoFlush = true };
                reader = new StreamReader(networkStream, new UTF8Encoding(false));

                Console.WriteLine("[INFO] Đã kết nối thành công!\n");

                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (true)
                        {
                            // Đọc nguyên vẹn 100% từng dòng từ Server gửi xuống
                            string response = await reader.ReadLineAsync();
                            if (response == null) break;

                            // Đẩy thẳng sang Form1 xử lý mà không làm gì thêm
                            OnDataReceived?.Invoke(response);
                        }
                    }
                    catch (Exception ex) { Console.WriteLine($"[LỖI - Nhận] {ex.Message}"); }
                });

                while (tcpClient.Connected) { await Task.Delay(100); }
            }
            catch (Exception ex) { Console.WriteLine($"[LỖI] {ex.Message}"); }
        }

        public static void SendMessage(string message)
        {
            if (tcpClient == null || !tcpClient.Connected || writer == null) return;
            try { writer.WriteLine(message); }
            catch (Exception ex) { Console.WriteLine($"[Lỗi gửi mạng] {ex.Message}"); }
        }
    }
}