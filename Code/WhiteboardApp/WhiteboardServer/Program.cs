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

        // Quản lý các kênh phòng: RoomID => danh sách client trong phòng
        private static Dictionary<string, List<TcpClient>> roomChannels = new Dictionary<string, List<TcpClient>>();
        // Quản lý phòng hiện tại của mỗi client
        private static Dictionary<TcpClient, string> clientRooms = new Dictionary<TcpClient, string>();
        private static readonly object roomLock = new object();
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            // Thiết lập mã hóa UTF-8 cho console để hiển thị tiếng Việt đúng cách
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("--- KHỞI ĐỘNG MÁY CHỦ WHITEBOARD ---");
            Console.WriteLine(">>> Bat dau khoi tao he thong Server...");
            
            // Nạp thông số mạng động từ file config.ini trước khi mở socket
            DocFileConfigIni("config.ini");

            // Tu dong kiem tra va tao bang du lieu ban dau
            TaoDatabaseForm();

            // Nếu bạn có class DatabaseManager riêng biệt, bỏ comment dòng dưới. Nếu không có thì xóa đi nhé.
            // DatabaseManager.InitializeDatabase();

            // --- KHU VỰC CHẠY THỬ NGHIỆM ĐỘC LẬP TẠI NHÀ ---
            Console.WriteLine("\n--- TIEN HANH KIEM TRA DATA ---");

            string duLieuTest = "DRAW;ROOM_101;150,200;155,202;#FF0000;3";
            LuuNetVe("ROOM_101", "NguyenVanAn", duLieuTest);

            DocLichSuPhong("ROOM_101");
            Console.WriteLine("--------------------------------\n");

            // KHU VỰC LẬP TRÌNH SOCKET 
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

                    // Kích hoạt đa luồng xử lý riêng biệt cho Client
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

                    string currentRoomID = "DefaultRoom";
                    string[] protocolParts = packetData.Split(';');
                    string command = protocolParts.Length > 0 ? protocolParts[0] : string.Empty;

                    if (command == "CREATE_ROOM" && protocolParts.Length >= 3)
                    {
                        string requestedUsername = protocolParts[1];
                        string requestedRoomName = protocolParts[2];
                        currentRoomID = CreateRoom(requestedUsername, requestedRoomName);
                        Console.WriteLine($"[ROOM] Da tao phong moi: {currentRoomID}");
                        AddClientToRoom(client, currentRoomID);
                        SendMessage(client, $"ROOM_CREATED;{currentRoomID}");
                        continue;
                    }

                    if (protocolParts.Length >= 2)
                    {
                        if (command == "DRAW" || command == "CLEAR_CANVAS" || command == "USER_LIST")
                        {
                            currentRoomID = protocolParts[1];
                        }
                        else if (command == "JOIN_ROOM" && protocolParts.Length >= 3)
                        {
                            currentRoomID = protocolParts[2];
                        }
                    }

                    bool isFirstTimeInRoom = AddClientToRoom(client, currentRoomID);
                    if (isFirstTimeInRoom)
                    {
                        Console.WriteLine($"[HỆ THỐNG] Người dùng '{username}' đã vào Phòng: {currentRoomID}");
                        DongBoLichSuChoClientMoi(client, currentRoomID);
                    }

                    if (packetData.StartsWith("DRAW"))
                    {
                        LuuNetVe(currentRoomID, username, packetData);
                    }

                    BroadcastData(currentRoomID, message, client);
                }
            }
            catch (Exception)
            {
                // Bắt lỗi ngắt kết nối đột ngột của thiết bị mạng
            }
            finally
            {
                RemoveClientFromRoom(client);
                client.Close();
                Console.WriteLine($"[NGẮT KẾT NỐI] Mot Client da thoat. Trong he thong con lai: {GetTotalClientCount()} nguoi.");
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

        // Hàm 2: ghi bản ghi nét vẽ khi nhận được tín hiệu qua mạng
        public static void LuuNetVe(string roomID ,string nguoiVe, string chuoiToaDo)
        {
            using var conn = new SqliteConnection(_dbPath);
            conn.Open();

            var sqlInsert = "INSERT INTO DrawHistory (RoomID, Username, PacketData) VALUES (@room, @user, @packet)";

            using var cmd = new SqliteCommand(sqlInsert, conn);
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

        // Gửi thông điệp tới tất cả client trong cùng phòng, ngoại trừ sender
        private static void BroadcastData(string roomID, string message, TcpClient sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            List<TcpClient> roomClients;

            lock (roomLock)
            {
                if (!roomChannels.TryGetValue(roomID, out roomClients))
                    return;
                roomClients = new List<TcpClient>(roomClients);
            }

            List<TcpClient> deadClients = null;

            foreach (TcpClient client in roomClients)
            {
                if (client == sender)
                    continue;

                try
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"[Broadcast Phong {roomID}] Da chuyen tiep tin nhan.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Broadcast Error - Phong {roomID}] {ex.Message}");
                    (deadClients ??= new List<TcpClient>()).Add(client);
                }
            }

            // Dọn các client đã rớt kết nối khỏi room một cách an toàn
            if (deadClients != null)
            {
                lock (roomLock)
                {
                    if (roomChannels.TryGetValue(roomID, out var roomList))
                    {
                        foreach (var dead in deadClients)
                        {
                            roomList.Remove(dead);
                            clientRooms.Remove(dead);
                        }
                    }
                }
            }
        }

        private static string CreateRoom(string creator, string roomName)
        {
            string roomId;
            lock (roomLock)
            {
                do
                {
                    roomId = $"ROOM_{random.Next(1000, 9999)}";
                } while (roomChannels.ContainsKey(roomId));

                roomChannels[roomId] = new List<TcpClient>();
            }
            Console.WriteLine($"[ROOM] Phong moi duoc tao: {roomId} | Ten phong: {roomName} | Nguoi tao: {creator}");
            return roomId;
        }

        private static bool AddClientToRoom(TcpClient client, string roomID)
        {
            lock (roomLock)
            {
                if (clientRooms.ContainsKey(client) && clientRooms[client] == roomID)
                    return false;

                if (clientRooms.ContainsKey(client))
                {
                    string oldRoom = clientRooms[client];
                    if (roomChannels.TryGetValue(oldRoom, out var oldList))
                    {
                        oldList.Remove(client);
                        Console.WriteLine($"[ROOM] Client da roi phong cu: {oldRoom}");
                    }
                }

                clientRooms[client] = roomID;
                if (!roomChannels.ContainsKey(roomID))
                {
                    roomChannels[roomID] = new List<TcpClient>();
                }

                if (!roomChannels[roomID].Contains(client))
                {
                    roomChannels[roomID].Add(client);
                    Console.WriteLine($"[ROOM] Client da duoc them vao phong: {roomID}");
                }
            }

            return true;
        }

        private static void RemoveClientFromRoom(TcpClient client)
        {
            lock (roomLock)
            {
                if (!clientRooms.TryGetValue(client, out var roomID))
                    return;

                clientRooms.Remove(client);
                if (roomChannels.TryGetValue(roomID, out var roomList))
                {
                    roomList.Remove(client);
                    Console.