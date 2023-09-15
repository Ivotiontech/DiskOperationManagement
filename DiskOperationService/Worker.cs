using System.Data;
using System;
using System.IO;
using PacketDotNet;
using SharpPcap;
using System.Runtime.CompilerServices;
using System.Management;
//using Newtonsoft.Json;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Net.Sockets;
using System.Xml;
using DiskOprationLib;
using Newtonsoft.Json;
using System.Text;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace DiskOperationService
{
    public class Worker : BackgroundService
    {
        int isStart = 0;
        private FileSystemWatcher _watcher;
        static string externaldrive = string.Empty;
        private readonly ILogger<Worker> _logger;
        private static string fileFullName = @"LicenseKey.txt";
        private static string fileWorker = @"WorkerLog.txt";
        private static List<dynamic> dynamicsList;
        private static List<dynamic> exceptionTCPJsonString;
        private static string servicePath = Directory.GetCurrentDirectory().Replace("DiskOperationManagementApp", "DiskOperationService");
        private Timer _timer;
        static DateTime utcTime;
        private static IOptions<ServerConfigModel> config;
        string dateString = "23/09/2023 00:00:00";
        string format = "yyyy-MM-dd hh:mm:ss";
        DateTime parsedDate;
        IFormatProvider provider = new CultureInfo("fr-FR");
        DateTime NextTimeToExecute = DateTime.MinValue;
        dynamic pathfromApi;

        public Worker(ILogger<Worker> logger, IOptions<ServerConfigModel> _config)
        {
            _logger = logger;
            dynamicsList = new List<dynamic>();
            config = _config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Dispose();
                GetUTCDateTime();
                if (utcTime >= Convert.ToDateTime("2023-09-25"))
                {
                    StopServiceCallback(null);
                }
                else
                {
                    _timer = new Timer(StopServiceCallback, null, TimeSpan.FromDays(10), TimeSpan.FromMilliseconds(-1));
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        int index = 0;

                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                        await Task.Delay(1000, stoppingToken);

                        if (NextTimeToExecute <= DateTime.Now)
                        {
                            NextTimeToExecute = DateTime.MinValue;
                        }

                        if (NextTimeToExecute == DateTime.MinValue)
                        {
                            NextTimeToExecute = DateTime.Now.AddHours(2);
                            //NextTimeToExecute = DateTime.Now.AddSeconds(20);
                            var jsonData = ReadDataFromFile.ReadFileForSpeceficData(servicePath + "\\" + fileFullName);
                            var param = new { lic = jsonData.lic };
                            pathfromApi = await DiskOperationApiRequest.PostDiskOperationApi(param, "get-license-data");
                        }

                        DriveInfo[] drives = DriveInfo.GetDrives();
                        OnUpdateLog();
                        GetAllFileFromFolder();
                        foreach (dynamic path in pathfromApi.data.path)
                        {
                            var driveData = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JContainer)path).Last).Value;
                            var drive = GetDriveFromFilePath(driveData.ToString());
                            // Check if the drive is ready and not a network drive
                            if (drive.IsReady && drive.DriveType != DriveType.Network)
                            {

                                // Exclude external drives (DriveType.Removable)
                                if (drive.DriveType != DriveType.Removable && isStart == 0)
                                {
                                    // Perform your desired actions with the non-external drive
                                    //Console.WriteLine("Drive: " + drive.Name);
                                    string systemDrive = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 3);
                                    //if (drive.Name == systemDrive) continue;


                                    // Create a new FileSystemWatcher instance
                                    _watcher = new FileSystemWatcher(driveData.ToString());

                                    // Set the properties to monitor
                                    _watcher.IncludeSubdirectories = true; // Monitor subdirectories as well
                                    _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;

                                    // Set the events to track
                                    _watcher.Created += OnCreated;
                                    _watcher.Changed += OnChanged;
                                    _watcher.Deleted += OnDeleted;
                                    _watcher.Renamed += OnRenamed;

                                    // Start monitoring
                                    _watcher.EnableRaisingEvents = true;

                                    //Console.WriteLine("Press enter to stop monitoring.");
                                    //Console.ReadLine();

                                    //// Stop monitoring
                                    //watcher.EnableRaisingEvents = false;
                                }
                                else if (drive.DriveType == DriveType.Removable)
                                {

                                    // Create a new FileSystemWatcher instance
                                    _watcher = new FileSystemWatcher(driveData.ToString());

                                    // Set the properties to monitor
                                    _watcher.IncludeSubdirectories = true; // Monitor subdirectories as well
                                    _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;

                                    // Set the events to track
                                    _watcher.Created += OnCreatedExternal;
                                    _watcher.Changed += OnChangedExternal;
                                    _watcher.Deleted += OnDeletedExternal;
                                    _watcher.Renamed += OnRenamedExternal;

                                    // Start monitoring
                                    _watcher.EnableRaisingEvents = true;
                                }

                            }
                            index++;
                           
                        }
                        isStart = 1;
                        await Task.Delay(1000, stoppingToken);
                    }
               }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                //throw ex;
            }

        }

        public void GetUTCDateTime() {
            string apiUrl = "http://worldtimeapi.org/api/timezone/UTC";
            try
            {
                var httpClient = new HttpClient();
                // The endpoint you want to request
                HttpResponseMessage response = httpClient.GetAsync($"{apiUrl}").Result;
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    var data = (JObject)JsonConvert.DeserializeObject(jsonResponse);
                    string timeZone = data["utc_datetime"].Value<string>();

                    string[] dateString = timeZone.Split('/');
                    utcTime = Convert.ToDateTime(timeZone);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static DriveInfo GetDriveFromFilePath(string filePath)
        {
            // Get the root directory (drive) of the file path
            string driveRoot = Path.GetPathRoot(filePath);

            // Get all available drives
            DriveInfo[] drives = DriveInfo.GetDrives();

            // Find the drive letter that matches the file path
            foreach (DriveInfo driveInfo in drives)
            {
                if (string.Equals(driveRoot, driveInfo.RootDirectory.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    return driveInfo;
                }
            }

            // If no matching drive is found, return an empty string or throw an exception, depending on your requirement.
            // For simplicity, we will return an empty string in this example.
            return null;
        }

        private void StopServiceCallback(object state)
        {
            _logger.LogInformation("Stopping worker service at: {time}", DateTimeOffset.Now);
            _timer?.Dispose();
            StopAsync(CancellationToken.None).Wait();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service running at: {time}", DateTimeOffset.Now);
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            isStart = 0;
            _logger.LogInformation("Service stopped at: {time}", DateTimeOffset.Now);
            return base.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            try
            {
                _watcher?.Dispose();
            }
            catch
            {
                ;
            }
            _watcher = null;
        }


        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                var data = new
                {
                    User = Environment.MachineName,
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                
                // Appending the given texts
                try
                {
                    TCPFileUpload(jsonString);
                }
                catch (Exception ex)
                {
                    dynamicsList.Add(data);
                }
            }

        }
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                var data = new
                {
                    User = Environment.MachineName,
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

                // Appending the given texts
                try
                {
                    TCPFileUpload(jsonString);
                }
                catch (Exception ex)
                {
                    dynamicsList.Add(data);
                }
            }

        }
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                var data = new
                {
                    User = Environment.MachineName,
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

                // Appending the given texts
                try
                {
                    TCPFileUpload(jsonString);
                }
                catch (Exception ex)
                {
                    dynamicsList.Add(data);
                }
            }
        }
        private static void OnRenamed(object sender, RenamedEventArgs e)
        {

            if (e.OldFullPath.EndsWith("\\"))
            {
                var data = new
                {
                    User = Environment.MachineName,
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

                // Appending the given texts
                try
                {
                    TCPFileUpload(jsonString);
                }
                catch (Exception ex)
                {
                    dynamicsList.Add(data);
                }
            }
            else
            {
                var data = new
                {
                    User = Environment.MachineName,
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

                // Appending the given texts
                try
                {
                    TCPFileUpload(jsonString);
                }
                catch (Exception ex)
                {
                    dynamicsList.Add(data);
                }
            }
        }
        private static void OnCreated1(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                string myfile = @"C:/log.txt";

                if (Path.GetPathRoot(e.FullPath) == Path.GetPathRoot(externaldrive))
                {
                    Console.WriteLine($"File {e.Name} was copied to the external drive.");
                    using (StreamWriter sw = File.AppendText(myfile))
                    {
                        sw.WriteLine($"File {e.Name} was copied to the external drive.");
                    }
                }

                // Appending the given texts
                //Console.WriteLine($"Folder/File created: {e.FullPath}");
            }

        }

        private static void TCPFileUpload(string filePath)
        {
            string serverIP = config.Value.serverIP;
            int serverPort = config.Value.serverPort;
            var path = servicePath + "\\" + fileWorker;
            try
            {
                using (TcpClient client = new TcpClient(serverIP, serverPort))
                {
                    Console.WriteLine("Connected to the server.");
                    byte[] dataBytes = Encoding.UTF8.GetBytes(filePath.ToString() + "--");
                    using (NetworkStream networkStream = client.GetStream())
                    using (MemoryStream fileStream = new MemoryStream(dataBytes))
                    {
                        // Read the file and send its content over the network stream
                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            networkStream.Write(buffer, 0, bytesRead);
                        }
                        
                        if (!File.Exists(path)) {
                            File.Create(path);
                        }
                        var successData = @"File sent successfully on this " + DateTime.Now;
                        File.AppendAllText(path, successData + Environment.NewLine);
                        Console.WriteLine("File sent successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains(path))
                {
                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(filePath);
                    exceptionTCPJsonString.Add(jsonObject);
                }
            }
        }
        private static void OnUpdateLog()
        {
            string jsonString = JsonConvert.SerializeObject(dynamicsList, Newtonsoft.Json.Formatting.Indented);
            // Appending the given texts
            try
            {
                if (dynamicsList.Count != 0)
                {
                    TCPFileUpload(jsonString);
                    dynamicsList.Clear();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
        }

        private static void GetAllFileFromFolder()
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(exceptionTCPJsonString, Newtonsoft.Json.Formatting.Indented);
                if (exceptionTCPJsonString != null)
                {
                    TCPFileUpload(jsonString);
                    exceptionTCPJsonString.Clear();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }

        }

        private static void OnCreatedExternal(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                var data = new
                {
                    User = "External -" + Environment.MachineName,
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

                // Appending the given texts
                try
                {
                    TCPFileUpload(jsonString);
                }
                catch (Exception ex)
                {
                    dynamicsList.Add(data);
                }
            }

        }
        private static void OnChangedExternal(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                var data = new
                {
                    User = "External -" + Environment.MachineName,
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

                // Appending the given texts
                try
                {
                    TCPFileUpload(jsonString);
                }
                catch (Exception ex)
                {
                    dynamicsList.Add(data);
                }
            }

        }
        private static void OnDeletedExternal(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                var data = new
                {
                    User = "External -" + Environment.MachineName,
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

                // Appending the given texts
                try
                {
                    TCPFileUpload(jsonString);
                }
                catch (Exception ex)
                {
                    dynamicsList.Add(data);
                }
            }
        }
        private static void OnRenamedExternal(object sender, RenamedEventArgs e)
        {

            if (e.OldFullPath.EndsWith("\\"))
            {
                var data = new
                {
                    User = "External -" + Environment.MachineName,
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

                // Appending the given texts
                try
                {
                    TCPFileUpload(jsonString);
                }
                catch (Exception ex)
                {
                    dynamicsList.Add(data);
                }
            }
            else
            {
                var data = new
                {
                    User = "External -" + Environment.MachineName,
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

                // Appending the given texts
                try
                {
                    TCPFileUpload(jsonString);
                }
                catch (Exception ex)
                {
                    dynamicsList.Add(data);
                }
            }
        }

    }
}