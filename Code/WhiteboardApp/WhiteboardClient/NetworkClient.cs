using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WhiteboardClient 
{
    class NetworkClient
    {
        static TcpClient tcpClient;
        static NetworkStream networkStream;
        static StreamWriter writer;
        static StreamReader reader;
        static Random random = new Random();

        public static async Task StartClientAsync()
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
}