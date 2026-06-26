using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WhiteboardClient
{
    public class NetworkClient
    {
        public static TcpClient? tcpClient;
        static NetworkStream? networkStream;
        static StreamWriter? writer;
        static StreamReader? reader;
        public static event Action<string>? OnDataReceived;

        public static async Task StartClientAsync()
        {
            Console.WriteLine("=== Mạng Client - Kết nối tới 127.0.0.1:8888 ===");

            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync("127.0.0.1", 8888);
                networkStream = tcpClient.GetStream();
                writer = new StreamWriter(networkStream) { AutoFlush = true };
                reader = new StreamReader(networkStream);

                Console.WriteLine("[INFO] Đã kết nối thành công tới 127.0.0.1:8888\n");

                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (reader != null)
                        {
                            string? response = await reader.ReadLineAsync();
                            if (response == null) break;
                            OnDataReceived?.Invoke(response);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LỖI - Nhận] {ex.Message}");
                    }
                });

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

        public static void SendDrawData(int x1, int y1, int x2, int y2, Color color, float size, bool isEraser)
        {
            if (tcpClient == null || !tcpClient.Connected || writer == null) return;

            try
            {
                string colorStr = isEraser ? "ERASE" : $"{color.R},{color.G},{color.B}";
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