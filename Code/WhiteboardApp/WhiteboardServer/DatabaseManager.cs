using System;
using Microsoft.Data.Sqlite; 

namespace WhiteboardServer
{
    public static class DatabaseManager
    {
        private const string ConnectionString = "Data Source=whiteboard.db";

        public static void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                // Câu lệnh SQL tạo bảng có cột RoomID 
                string createTableSql = @"
                    CREATE TABLE IF NOT EXISTS DrawLines (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        RoomID TEXT NOT NULL,
                        Username TEXT,
                        X1 INTEGER NOT NULL,
                        Y1 INTEGER NOT NULL,
                        X2 INTEGER NOT NULL,
                        Y2 INTEGER NOT NULL,
                        ColorHex TEXT NOT NULL,
                        Thickness INTEGER NOT NULL,
                        CreatedTime DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";

                using (var command = new SqliteCommand(createTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }
                Console.WriteLine("[SQLite] Da khoi tao cau truc co so du lieu RoomID thanh cong!");
            }
        }
    }
}