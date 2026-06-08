using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using Microsoft.Data.Sqlite;

namespace WhiteboardServer
{
    class Program
    {
        // Cấu hình kết nối tới file database cục bộ
        private static string _dbPath = "Data Source=whiteboard.db";
        
        // Danh sách lưu trữ các kết nối client kết nối tới hệ thống (Dành cho TV4 và TV5)
        private static List<TcpClient> list_clients = new List<TcpClient>();

        static void Main(string[] args)
        {
            Console.WriteLine(">>> Bat dau khoi tao he thong Server...");

            // Tu dong kiem tra va tao bang du lieu ban dau
            TaoDatabaseForm();

            // --- KHU VỰC CHẠY THỬ NGHIỆM ĐỘC LẬP TẠI NHÀ ---
            Console.WriteLine("\n--- TIEN HANH KIEM TRA DATA ---");
            
            string duLieuTest = "DRAW;150,200;155,202;#FF0000;3";
            LuuNetVe("NguyenVanAn", duLieuTest);
            
            DocLichSuPhong();
            Console.WriteLine("--------------------------------\n");

            // --- KHU VỰC LẬP TRÌNH SOCKET (Kế hoạch của Thành viên 4 & 5) ---
            Console.WriteLine("[SERVER] Khoi dong Socket cho Client ket noi...");
            // Code khoi tao TcpListener cua thanh vien 4 viet tiep tai day
        }

        // Hàm 1: Khởi tạo tệp tin và tạo cấu trúc bảng chứa nét vẽ
        private static void TaoDatabaseForm()
        {
            using var conn = new SqliteConnection(_dbPath);
            conn.Open();

            var sqlCheckTable = @"
                CREATE TABLE IF NOT EXISTS DrawHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    PacketData TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );";

            using var cmd = new SqliteCommand(sqlCheckTable, conn);
            cmd.ExecuteNonQuery();
            
            Console.WriteLine("[SQLite] Check va khoi tao bang DrawHistory xong.");
        }

        // Hàm 2: Thêm mới bản ghi nét vẽ khi nhận được tín hiệu qua mạng
        public static void LuuNetVe(string nguoiVe, string chuoiToaDo)
        {
            using var conn = new SqliteConnection(_dbPath);
            conn.Open();

            var sqlInsert = "INSERT INTO DrawHistory (Username, PacketData) VALUES (@user, @packet)";
            
            using var cmd = new SqliteCommand(sqlInsert, conn);
            // Gán tham số trực tiếp để tối ưu hóa truy vấn
            cmd.Parameters.AddWithValue("@user", nguoiVe);
            cmd.Parameters.AddWithValue("@packet", chuoiToaDo);
            
            cmd.ExecuteNonQuery();
            Console.WriteLine($"[SQLite] Da ghi net ve cua: {nguoiVe}");
        }

        // Hàm 3: Truy vấn ngược toàn bộ lịch sử để đồng bộ phòng vẽ
        public static void DocLichSuPhong()
        {
            Console.WriteLine("[SQLite] Lay danh sach lich su tu file db...");
            
            using var conn = new SqliteConnection(_dbPath);
            conn.Open();

            var sqlSelect = "SELECT Id, Username, PacketData FROM DrawHistory ORDER BY Id ASC";
            
            using var cmd = new SqliteCommand(sqlSelect, conn);
            using var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                int maSo = reader.GetInt32(0);
                string tenUser = reader.GetString(1);
                string thongTinMa = reader.GetString(2);
                
                Console.WriteLine($"   => Record #{maSo} | User: {tenUser} | Data: {thongTinMa}");
            }
        }
    }
}