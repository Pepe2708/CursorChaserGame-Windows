using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CCG
{
    static class NetworkClient
    {
        public static string uploadStatus = "[Right click] Upload highscore";
        public static string downloadStatus = "[Right click] Show personal highscore";
        static string address = "127.0.0.1";
        static int port = 8001;

        public static void UploadHighscore()
        {
            uploadStatus = "Connecting...";
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(address, port);
            Socket socket = tcpClient.Client;

            string message = GUI.highScore.ToString();

            Byte[] bMessage = Encoding.UTF8.GetBytes(message);

            uploadStatus = "Uploading...";
            socket.Send(bMessage);
            uploadStatus = "Upload finished!";

            tcpClient.Close();
        }

        public static void DownloadHighscore()
        {
            downloadStatus = "Connecting...";
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(address, port);
            Socket socket = tcpClient.Client;

            string message = "D";
            Byte[] bMessage = Encoding.UTF8.GetBytes(message);
            socket.Send(bMessage);

            Byte[] bAnswer = new Byte[256];
            downloadStatus = "Downloading...";
            int messageSize = socket.Receive(bAnswer);

            string answer = Encoding.UTF8.GetString(bAnswer, 0, messageSize);
            Console.WriteLine(answer);

            downloadStatus = "Download finished!";
            GUI.highScore = Convert.ToInt32(answer);
        }

        public static void ResetStatus()
        {
            uploadStatus = "[Right click] Upload highscore";
            downloadStatus = "[Right click] Show personal highscore";
        }
    }
}
