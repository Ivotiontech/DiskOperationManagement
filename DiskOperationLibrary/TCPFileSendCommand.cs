using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DiskOperationLibrary
{
    
    public static class TCPFileSendCommand
    {
        static Dictionary<string, string> riga = new Dictionary<string, string>();
        static string host = "192.168.1.11";
        static int port = 5141;
        static int buffer_size = 4096;
        static byte[] separator = Encoding.ASCII.GetBytes("\n");
        static int max_connections = 99;
        static int active_connections = 0;
        static string Log_path = "F:\\Aditya\\Disk_Operations_Monitoring\\DiskOperationManagement\\DiskOperationService\\logs";
        static Socket server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static List<byte> json_buffer = new List<byte>();

        public static void TCPSendFileData()
        {
            server_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            server_socket.Bind(new IPEndPoint(IPAddress.Parse(host), port));
            server_socket.Listen(5);
            Console.WriteLine("Server in listen porta " + port);

            while (true)
            {
                Socket client_socket = server_socket.Accept();
                Thread client_thread = new Thread(() => HandleClient(client_socket));
                client_thread.Start();
            }
        }
        static void LogMessage(string logfile, string message)
        {
            using (StreamWriter writer = File.AppendText(logfile))
            {
                writer.WriteLine(message);
            }
        }

        static void HandleClient(Socket client_socket)
        {
            byte[] data = new byte[buffer_size];
            while (true)
            {
                try
                {
                    int bytesRead = client_socket.Receive(data);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    List<byte> bytes = separator.ToList(); 
                    json_buffer.AddRange(data.Take(bytesRead));
                    while (json_buffer.Contains(bytes.FirstOrDefault()))
                    {
                        int separatorIndex = json_buffer.IndexOf(bytes.FirstOrDefault());
                        byte[] jsonBytes = json_buffer.Take(separatorIndex).ToArray();
                        json_buffer.RemoveRange(0, separatorIndex + separator.Length);
                        string jsonData = Encoding.UTF8.GetString(jsonBytes);
                        try
                        {
                            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonData);
                            try
                            {
                                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
                                try
                                {
                                    File.WriteAllText(Log_path, jsonString);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error: " + ex.Message);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error: " + ex.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

    }
}
