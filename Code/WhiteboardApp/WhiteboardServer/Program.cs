using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Data.Sqlite;
using System.Text;

namespace WhiteboardServer
{
    class Program
    {
        // Cấu hình kết nối tới file database cục bộ
        private static string _dbPath = "Data Source=whiteboard.db";
        private static string _serverIP = "127.0.0.1";
        private static int _serverPort = 8888;

        // Danh sách lưu trữ các kết nối client kết nối tới hệ thống (Dành cho TV4 và TV5)
        private static List<TcpClient> clientList = new List<TcpClient>();
        // Quản lý xem thiết bị Client nào đang đứng ở RoomID nào để cô lập mạng
        private static Dictionary<TcpClient, string> clientRooms = new Dictionary<TcpClient, string>();
        private static Dictionary<TcpClient, string> clientNames = new Dictionary<TcpClient, string>();
        // logic Server cần quản lý:
        public static Dictionary<string, List<TcpClient>> Rooms = new Dictionary<string, List<TcpClient>>();

        static void Main(string[] args)
        {
            // Thiết lập mã hóa UTF-8 cho console để hiển thị tiếng Việt đúng cách
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("--- KHỞI ĐỘNG MÁY CHỦ WHITEBOARD ---");
            Console.WriteLine(">>> Bat dau khoi tao he thong Server...");
            //Nạp thông số mạng động từ file config.ini trước khi mở socket
            DocFileConfigIni("config.ini");

            // Tu dong kiem tra va tao bang du lieu ban dau
            TaoDatabaseForm();

            // Khởi tạo Database nếu bạn có class DatabaseManager riêng
            // DatabaseManager.InitializeDatabase(); 

            // --- KHU VỰC CHẠY THỬ NGHIỆM ĐỘC LẬP TẠI NHÀ ---
            Console.WriteLine("\n--- TIEN HANH KIEM TRA DATA ---");
            DocLichSuPhong("ROOM_101");
            Console.WriteLine("--------------------------------\n");

            //  KHU VỰC LẬP TRÌNH SOCKET 
            Console.WriteLine("[SERVER] Khoi dong Socket cho Client ket noi...");

            try
            {
                IPAddress ipAddr = IPAddress.Parse(_serverIP);
                TcpListener server = new TcpListener(ipAddr, _serverPort);
                server.Start();

                Console.WriteLine($"[SERVER] Dang lang nghe tai cong {_serverPort}...");

                while (true)
                {
                    // Phương thức chặn đứng chờ thiết bị kết nối
                    TcpClient client = server.AcceptTcpClient();

                    Console.WriteLine($"[SERVER] Client moi ket noi: {client.Client.RemoteEndPoint}");

                    // Đồng bộ bảo vệ tài nguyên danh sách khi thêm Client mới
                    lock (clientList)
                    {
                        clientList.Add(client);
                    }
                    // Kích hoạt đa luồng xử lý riêng biệt cho Client (Giữ nguyên kiến trúc đa luồng của bạn)
                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERVER ERROR] {ex.Message}");
            }
        }

        // Cập nhật danh sách User
        private static void BroadcastUserList(string roomID)
        {
            List<string> usersInRoom = new List<string>();

            // Lọc tìm tất cả những người đang ở trong roomID này
            lock (clientRooms)
            {
                foreach (var kvp in clientRooms)
                {
                    if (kvp.Value == roomID)
                    {
                        lock (clientNames)
                        {
                            if (clientNames.ContainsKey(kvp.Key))
                                usersInRoom.Add(clientNames[kvp.Key]);
                        }
                    }
                }
            }

            // Ghép tên thành chuỗi: User1,User2,User3
            string userListString = string.Join(",", usersInRoom);

            // ⚡ ĐÃ SỬA LỖI: Cấu trúc gọi về Client chỉ còn USER_LIST;DanhSachTen
            string packetData = $"USER_LIST;{userListString}";

            // Bắn danh sách này cho toàn bộ phòng (Dùng lại hàm BroadcastData nhưng truyền sender = null để gửi cho TẤT CẢ)
            BroadcastData(roomID, packetData, null);
        }

        // Hàm HandleClient xử lý dòng dữ liệu mạng Real-time liên tục
        private static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);

            string currentRoomID = "ROOM001";
            lock (clientRooms)
            {
                clientRooms[client] = currentRoomID;
            }

            try
            {
                while (true)
                {
                    // Đọc gói tin nét vẽ truyền từ Client lên qua mạng
                    string message = reader.ReadLine();
                    if (message == null) break; // Client rời phòng hoặc đóng ứng dụng

                    Console.WriteLine($"[NHẬN ĐƯỢC TỪ CLIENT] {message}");

                    string username = "Ẩn danh";
                    string packetData = message;

                    // Phân tách nếu Client có gửi kèm tên dạng "Username|Gói_Tin"
                    if (message.Contains("|"))
                    {
                        var parts = message.Split('|');
                        username = parts[0];
                        packetData = parts[1];
                    }

                    // Phân tách giao thức bằng dấu chấm phẩy ;
                    string[] protocolParts = packetData.Split(';');

                    if (protocolParts.Length >= 2)
                    {
                        string command = protocolParts[0];

                        // CẬP NHẬT ROOM ID CHO CÁC LỆNH VÀO PHÒNG
                        if (command == "CONNECT" || command == "JOIN_ROOM" || command == "CREATE_ROOM")
                        {
                            if (protocolParts.Length >= 3)
                            {
                                username = protocolParts[1];
                                currentRoomID = protocolParts[2]; // Nhận mã phòng thực tế từ Client
                            }
                            else if (command == "CONNECT" && protocolParts.Length >= 2)
                            {
                                username = protocolParts[1];
                                currentRoomID = "ROOM001";
                            }

                            // Lưu/Cập nhật thông tin phòng của Client vào Dictionary
                            lock (clientRooms) { clientRooms[client] = currentRoomID; }
                            lock (clientNames) { clientNames[client] = username; }

                            Console.WriteLine($"[HỆ THỐNG] Thành viên '{username}' đã kết nối vào phòng: {currentRoomID}");

                            // Đồng bộ lịch sử phòng cũ cho người mới vào
                            DongBoLichSuChoClientMoi(client, currentRoomID);

                            // Cập nhật danh sách User hiển thị trên UI cho cả phòng nhìn thấy nhau
                            BroadcastUserList(currentRoomID);
                            continue;
                        }
                        else if (command == "CLEAR_CANVAS" || command == "USER_LIST")
                        {
                            currentRoomID = protocolParts[1];
                        }
                        // TRÍCH XUẤT ROOM ID TỪ GÓI VẼ "DRAW"
                        else if (command == "DRAW")
                        {
                            // Vị trí cuối cùng của mảng chính là Room ID được Client đính kèm theo nét vẽ
                            currentRoomID = protocolParts[protocolParts.Length - 1];

                            // Cập nhật lại bộ nhớ tạm thời của Server để chắc chắn không bị lệch phòng khi truyền phát
                            lock (clientRooms) { clientRooms[client] = currentRoomID; }
                        }
                    }

                    // Hành động 1: Lưu nét vẽ vào đúng phòng hiện tại (SQLite)
                    if (packetData.StartsWith("DRAW"))
                    {
                        LuuNetVe(currentRoomID, username, packetData);
                    }

                    // Hành động 2: Phát sóng chuyển tiếp nét vẽ nguyên bản cho các máy khác CÙNG PHÒNG
                    BroadcastData(currentRoomID, packetData, client);
                }
            }
            catch (Exception)
            {
                // Bắt lỗi ngắt kết nối đột ngột của thiết bị mạng
            }
            finally
            {
                lock (clientList) { clientList.Remove(client); }
                lock (clientRooms) { clientRooms.Remove(client); }

                // ⚡ ĐÃ SỬA LỖI: Xóa tên người đó khỏi danh sách để UI không bị lưu lại
                lock (clientNames) { clientNames.Remove(client); }

                client.Close();
                Console.WriteLine($"[NGẮT KẾT NỐI] Một Client đã thoát. Trong phòng còn lại: {clientList.Count} người.");

                // ⚡ ĐÃ SỬA LỖI: Gửi tín hiệu điểm danh lại cho phòng
                if (!string.IsNullOrEmpty(currentRoomID))
                {
                    BroadcastUserList(currentRoomID);
                }
            }
        }

        // Bộ nạp thông số file config.ini thủ công
        private static void DocFileConfigIni(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[WARNING] Khong tim thay file {filePath}. He thong su dung cau hinh mac dinh.");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string cleanLine = line.Trim();
                if (string.IsNullOrEmpty(cleanLine) || cleanLine.StartsWith(";") || cleanLine.StartsWith("["))
                    continue;

                if (cleanLine.Contains("="))
                {
                    int idx = cleanLine.IndexOf('=');
                    string key = cleanLine.Substring(0, idx).Trim().ToLower();
                    string val = cleanLine.Substring(idx + 1).Trim();

                    if (key == "path") _dbPath = val;
                    if (key == "ip") _serverIP = val;
                    if (key == "port") _serverPort = int.Parse(val);
                }
            }
            Console.WriteLine($"[CONFIG.INI] Da nap cau hinh: DB={_dbPath} | IP={_serverIP} | Port={_serverPort}");
        }

        // Hàm 1: Khởi tạo tệp tin và tạo cấu trúc bảng chứa nét vẽ
        private static void TaoDatabaseForm()
        {
            using var conn = new SqliteConnection(_dbPath);
            conn.Open();

            var sqlCheckTable = @"
                CREATE TABLE IF NOT EXISTS DrawHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RoomID TEXT NOT NULL DEFAULT 'DefaultRoom',
                    Username TEXT NOT NULL,
                    PacketData TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );";

            using var cmd = new SqliteCommand(sqlCheckTable, conn);
            cmd.ExecuteNonQuery();

            Console.WriteLine("[SQLite] Check va khoi tao bang DrawHistory xong.");
        }

        // Hàm 2:bản ghi nét vẽ khi nhận được tín hiệu qua mạng
        public static void LuuNetVe(string roomID, string nguoiVe, string chuoiToaDo)
        {
            using var conn = new SqliteConnection(_dbPath);
            conn.Open();

            var sqlInsert = "INSERT INTO DrawHistory (RoomID, Username, PacketData) VALUES (@room, @user, @packet)";

            using var cmd = new SqliteCommand(sqlInsert, conn);
            // Gán tham số trực tiếp để tối ưu hóa truy vấn
            cmd.Parameters.AddWithValue("@room", roomID);
            cmd.Parameters.AddWithValue("@user", nguoiVe);
            cmd.Parameters.AddWithValue("@packet", chuoiToaDo);

            cmd.ExecuteNonQuery();
            Console.WriteLine($"[SQLite] Da ghi net ve cua: {nguoiVe} vao Phong: {roomID}");
        }

        // Hàm 3: Truy vấn ngược toàn bộ lịch sử để đồng bộ phòng vẽ
        public static void DocLichSuPhong(string roomID)
        {
            Console.WriteLine($"[SQLite] Lay danh sach lich su cua phong [{roomID}] tu file db...");

            using var conn = new SqliteConnection(_dbPath);
            conn.Open();

            var sqlSelect = "SELECT Id, Username, PacketData FROM DrawHistory WHERE RoomID = @room ORDER BY Id ASC";

            using var cmd = new SqliteCommand(sqlSelect, conn);
            cmd.Parameters.AddWithValue("@room", roomID);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int maSo = reader.GetInt32(0);
                string tenUser = reader.GetString(1);
                string thongTinMa = reader.GetString(2);

                Console.WriteLine($"   => Phong: {roomID} | Record #{maSo} | User: {tenUser} | Data: {thongTinMa}");
            }
        }

        // Bắn ngược toàn bộ lịch sử lưu trong SQLite cho Client mới kết nối muộn
        private static void DongBoLichSuChoClientMoi(TcpClient targetClient, string roomID)
        {
            try
            {
                using var conn = new SqliteConnection(_dbPath);
                conn.Open();

                // Chỉ lấy chuỗi PacketData gốc để gửi trực tiếp cho Client vẽ lại
                var sqlSelect = "SELECT PacketData FROM DrawHistory WHERE RoomID = @room ORDER BY Id ASC";
                using var cmd = new SqliteCommand(sqlSelect, conn);
                cmd.Parameters.AddWithValue("@room", roomID);
                using var reader = cmd.ExecuteReader();

                NetworkStream stream = targetClient.GetStream();
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                int count = 0;
                while (reader.Read())
                {
                    string data = reader.GetString(0);
                    writer.WriteLine(data); // Gửi chuỗi DRAW;X1,Y1... sạch sẽ không dính tên người vẽ ở đầu
                    count++;
                }
                Console.WriteLine($"[SQLite] Đã đồng bộ xong {count} nét vẽ cũ của phòng [{roomID}] cho thành viên mới.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLite Error] Đồng bộ lịch sử phòng {roomID} thất bại: {ex.Message}");
            }
        }

        //Gui thong diep toi tat ca client ngoai tru sender
        private static void BroadcastData(string roomID, string message, TcpClient sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");

            lock (clientList)
            {
                foreach (TcpClient client in clientList)
                {
                    // Không gửi lại cho chính client gửi
                    if (client == sender)
                        continue;
                    //CÔ LẬP PHÒNG: Kiểm tra nếu client đích không ở cùng RoomID thì bỏ qua
                    lock (clientRooms)
                    {
                        if (!clientRooms.ContainsKey(client) || clientRooms[client] != roomID)
                            continue;
                    }

                    try
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(data, 0, data.Length);
                        Console.WriteLine($"[Broadcast Phong {roomID}] Da chuyen tiep tin nhan.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Broadcast Error] {ex.Message}");
                    }
                }
            }
        }
    }
}