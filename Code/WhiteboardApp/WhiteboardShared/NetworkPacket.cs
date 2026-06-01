using System;

namespace WhiteboardShared
{
    public class NetworkPacket
    {
        // Định nghĩa các lệnh hệ thống để tránh gõ sai chính tả
        public const string CONNECT = "CONNECT";
        public const string DISCONNECT = "DISCONNECT";
        public const string DRAW = "DRAW";

        public string Command { get; set; }
        public string Data { get; set; }

        // Hàm hỗ trợ đóng gói nhanh dữ liệu theo quy ước chuỗi kết thúc bằng \n
        public static string Pack(string command, string data)
        {
            return $"{command};{data}\n";
        }
    }
}