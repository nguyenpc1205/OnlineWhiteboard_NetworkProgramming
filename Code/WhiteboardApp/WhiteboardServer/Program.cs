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

            DatabaseManager.InitializeDatabase();

            // --- KHU VỰC CHẠY THỬ NGHIỆM ĐỘC LẬP TẠI NHÀ ---
            Console.WriteLine("\n--- TIEN HANH KIEM TRA DATA ---");

            string duLieuTest = "DRAW;ROOM_101;150,200;155,202;#FF0000;3";
            LuuNetVe("ROOM_101", "NguyenVanAn", duLieuTest);

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

        // Hàm HandleClient xử lý dòng dữ liệu mạng Real-time liên tục
        private static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);

            try
            {
                while (true)
                {
                    // Đọc gói tin nét vẽ truyền từ Client lên qua mạng
                    string message = reader.ReadLine();
                    if (message == null) break; // Client rời phòng hoặc đóng ứng dụng

                    Console.WriteLine($"[NHẬN ĐƯỢC TỪ CLIENT] {message}");

                    // Phân tích chuỗi dữ liệu (Định dạng mẫu: "TênNgườiVẽ|DữLiệuGóiTin")
                    string username = "Ẩn danh";
                    string packetData = message;
                    if (message.Contains("|"))
                    {
                        var parts = message.Split('|');
                        username = parts[0];
                        packetData = parts[1];
                    }
                    // LOGIC TRÍCH XUẤT ROOM ID TỰ ĐỘNG ĐỂ PHÂN LUỒNG
                    string currentRoomID = "DefaultRoom"; // Phòng mặc định nếu gói tin lỗi
                    string[] protocolParts = packetData.Split(';');

                    if (protocolParts.Length >= 2)
                    {
                        string command = protocolParts[0];
                        // Nếu là lệnh DRAW/CLEAR_CANVAS từ lớp Shared: DRAW;RoomID;ToaDo...
                        if (command == "DRAW" || command == "CLEAR_CANVAS" || command == "USER_LIST")
                        {
                            currentRoomID = protocolParts[1];
                        }
                        // Nếu là lệnh JOIN_ROOM: JOIN_ROOM;Username;RoomID
                        else if (command == "JOIN_ROOM" && protocolParts.Length >= 3)
                        {
                            currentRoomID = protocolParts[2];
                        }
                    }

                    // Kiểm tra xem Client này đã đăng ký phòng này chưa, nếu chưa thì thực hiện map phòng và tải lịch sử
                    bool isFirstTimeInRoom = false;
                    lock (clientRooms)
                    {
                        if (!clientRooms.ContainsKey(client) || clientRooms[client] != currentRoomID)
                        {
                            clientRooms[client] = currentRoomID;
                            isFirstTimeInRoom = true;
                        }
                    }

                    if (isFirstTimeInRoom)
                    {
                        Console.WriteLine($"[HỆ THỐNG] Người dùng '{username}' đã vào Phòng: {currentRoomID}");
                        // Tải và bắn ngược lịch sử ĐÚNG PHÒNG cho client vẽ đắp lại bảng
                        DongBoLichSuChoClientMoi(client, currentRoomID);
                    }

                    if (packetData.StartsWith("DRAW"))
                    {
                        LuuNetVe(currentRoomID, username, packetData);
                    }

                    // Hành động 2: Phát sóng chuyển tiếp nét vẽ cho các thành viên trực thuộc CÙNG PHÒNG
                    BroadcastData(currentRoomID, message, client);
                }
            }
            catch (Exception)
            {
                // Bắt lỗi ngắt kết nối đột ngột của thiết bị mạng
            }
            finally
            {
                lock (clientList)
                {
                    clientList.Remove(client);
                }
                lock (clientRooms)
                {
                    clientRooms.Remove(client);
                }
                    client.Close();
                    Console.WriteLine($"[NGẮT KẾT NỐI] Mot Client da thoat. Trong phong con lai: {clientList.Count} nguoi.");
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
        public static void LuuNetVe(string roomID ,string nguoiVe, string chuoiToaDo)
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

                // Chỉ lấy lịch sử của phòng mà Client này khai báo tham gia
                var sqlSelect = "SELECT Username, PacketData FROM DrawHistory WHERE RoomID = @room ORDER BY Id ASC";
                using var cmd = new SqliteCommand(sqlSelect, conn);
                cmd.Parameters.AddWithValue("@room", roomID);
                using var reader = cmd.ExecuteReader();

                NetworkStream stream = targetClient.GetStream();
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                int count = 0;
                while (reader.Read())
                {
                    string user = reader.GetString(0);
                    string data = reader.GetString(1);
                    writer.WriteLine($"{user}|{data}");
                    count++;
                }
                Console.WriteLine($"[SQLite] Da dong bo xong {count} net ve cu cua phong [{roomID}] cho thanh vien moi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLite Error] Dong bo lich su phong {roomID} that bai: {ex.Message}");
            }
        }
        // roomChannels: roomID -> danh sách client đang ở trong phòng đó
        private static Dictionary<string, List<TcpClient>> roomChannels = new Dictionary<string, List<TcpClient>>();
        
        private static void BroadcastData(string roomID, string message, TcpClient sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            
            List<TcpClient> clientsInRoom;
            
            lock (roomChannels)
            {
                if (!roomChannels.TryGetValue(roomID, out clientsInRoom) || clientsInRoom.Count == 0)
                {
                    Console.WriteLine($"[Broadcast] Phòng {roomID} không có client nào.");
                    return;
                }
                // Copy ra list mới để gửi data bên ngoài lock (tránh block các client khác join/leave room)
                clientsInRoom = new List<TcpClient>(clientsInRoom);
            }
            List<TcpClient> deadClients = null;
            foreach (TcpClient client in clientsInRoom)
            {
                // Không gửi lại cho chính client vừa gửi
                if (client == sender)
                    continue;
                try
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"[Broadcast Phòng {roomID}] Đã chuyển tiếp nét vẽ.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Broadcast Error - Phòng {roomID}] {ex.Message}");
                    (deadClients ??= new List<TcpClient>()).Add(client);
                }
            }
            // Dọn các client đã rớt kết nối khỏi room
            if (deadClients != null)
            {
                lock (roomChannels)
                {
                    foreach (var dead in deadClients)
                        clientsInRoom.Remove(dead); // hoặc roomChannels[roomID].Remove(dead);
                }
            }
        }
    }
}
