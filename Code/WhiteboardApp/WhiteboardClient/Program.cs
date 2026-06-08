using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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

            Console.WriteLine("[SERVER] Khoi dong Socket cho Client ket noi...");

TcpListener server = new TcpListener(IPAddress.Any, 8888);
server.Start();

Console.WriteLine("[SERVER] Dang lang nghe tai cong 8888...");

while (true)
{
    TcpClient client = server.AcceptTcpClient();

    Console.WriteLine(
        $"[SERVER] Client moi ket noi: {client.Client.RemoteEndPoint}");

    lock (list_clients)
    {
        list_clients.Add(client);
    }

    Thread clientThread =
        new Thread(() => HandleClient(client));

    clientThread.Start();
}
        } 
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

        
