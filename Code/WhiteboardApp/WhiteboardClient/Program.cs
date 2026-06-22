using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace WhiteboardClient
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Khởi chạy luồng mạng ngầm song song với giao diện Form1
            Task.Run(async () => {
                await NetworkClient.StartClientAsync();
            });

            // Mở giao diện bảng vẽ
            Application.Run(new Form1());
        }
    }
}
