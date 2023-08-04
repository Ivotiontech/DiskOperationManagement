using System.Data;
using System;
using System.IO;
using PacketDotNet;
using SharpPcap;
using System.Runtime.CompilerServices;
using System.Management;
using DiskOperationLibrary;
using Newtonsoft.Json;

namespace DiskOperationService
{
    public class Worker : BackgroundService
    {
        int isStart = 0;
        static string externaldrive = string.Empty;
        private readonly ILogger<Worker> _logger;
        private readonly string fileFullName = @"C:\Aditya_Project\DiskOperationManagement\DiskOperationManagement\DiskOperationService\LicenseKey.txt";
        static string fileToRead = "C:\\Aditya_Project\\DiskOperationManagement\\DiskOperationManagement\\DiskOperationService\\FileDataRead";
        static string filesLogs = "C:\\Aditya_Project\\DiskOperationManagement\\DiskOperationManagement\\DiskOperationService\\logs";
        private static List<dynamic> dynamicsList;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            dynamicsList = new List<dynamic>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    int index = 0;
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(1000, stoppingToken);
                    var jsonData = ReadDataFromFile.ReadFileForSpeceficData(fileFullName);
                    var param = new { lic = jsonData.lic };
                    var dt = await DiskOperationApiRequest.PostDiskOperationApi(param, "get-license-data");

                    DriveInfo[] drives = DriveInfo.GetDrives();

                    //foreach (dynamic path in dt.data.path)
                    //{
                    //var cd = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JContainer)path).Last).Value;
                    var drive = GetDriveFromFilePath(fileToRead);
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
                            FileSystemWatcher watcher = new FileSystemWatcher(fileToRead);

                            // Set the properties to monitor
                            watcher.IncludeSubdirectories = true; // Monitor subdirectories as well
                            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;

                            // Set the events to track
                            watcher.Created += OnCreated;
                            watcher.Changed += OnChanged;
                            watcher.Deleted += OnDeleted;
                            watcher.Renamed += OnRenamed;

                            // Start monitoring
                            watcher.EnableRaisingEvents = true;

                            //Console.WriteLine("Press enter to stop monitoring.");
                            //Console.ReadLine();

                            //// Stop monitoring
                            //watcher.EnableRaisingEvents = false;
                        }
                        else if (drive.DriveType == DriveType.Removable)
                        {

                            // Create a new FileSystemWatcher instance
                            FileSystemWatcher watcher = new FileSystemWatcher(fileToRead);

                            // Set the properties to monitor
                            watcher.IncludeSubdirectories = true; // Monitor subdirectories as well
                            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;

                            // Set the events to track
                            watcher.Created += OnCreated1;

                            // Start monitoring
                            watcher.EnableRaisingEvents = true;
                        }

                    }
                    index++;
                    //}
                    //await Task.Delay(5000, stoppingToken);
                    //if (dynamicsList.Any())
                    //{
                    //    OnUpdateLog();
                    //}
                    isStart = 1;
                    //Console.WriteLine("Press enter to stop monitoring.");
                    //Console.ReadLine();


                    // Stop monitoring
                    //watcher.EnableRaisingEvents = false;


                }

            }
            catch (Exception ex)
            {

                throw ex;
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
                string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
                string myfile = filesLogs + "//log" + DateTime.UtcNow.Ticks + ".json";

                // Appending the given texts
                try
                {
                    // Appending the given texts
                    File.WriteAllText(myfile, jsonString);
                    EncryptFileCommand encryptFile = new EncryptFileCommand();
                    encryptFile.EncryptFile(myfile);
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
                string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
                string myfile = filesLogs + "//log" + DateTime.UtcNow.Ticks + ".json";

                // Appending the given texts
                try
                {
                    // Appending the given texts
                    File.WriteAllText(myfile, jsonString);
                    EncryptFileCommand encryptFile = new EncryptFileCommand();
                    encryptFile.EncryptFile(myfile);
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
                    User = "User that generate log",
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
                string myfile = filesLogs + "//log" + DateTime.UtcNow.Ticks + ".json";

                // Appending the given texts
                try
                {
                    // Appending the given texts
                    File.WriteAllText(myfile, jsonString);
                    EncryptFileCommand encryptFile = new EncryptFileCommand();
                    encryptFile.EncryptFile(myfile);
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
                    User = "User that generate log",
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
                string myfile = filesLogs + "//log" + DateTime.UtcNow.Ticks + ".json";

                // Appending the given texts
                try
                {
                    // Appending the given texts
                    File.WriteAllText(myfile, jsonString);
                    EncryptFileCommand encryptFile = new EncryptFileCommand();
                    encryptFile.EncryptFile(myfile);
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
                    User = "User that generate log",
                    File_name = e.ChangeType + " - " + e.FullPath,
                    Time = DateTime.UtcNow
                };
                string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
                string myfile = filesLogs + "//log" + DateTime.UtcNow.Ticks + ".json";

                // Appending the given texts
                try
                {
                    // Appending the given texts
                    File.WriteAllText(myfile, jsonString);
                    EncryptFileCommand encryptFile = new EncryptFileCommand();
                    encryptFile.EncryptFile(myfile);
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

        private static void OnUpdateLog()
        {

            string jsonString = JsonConvert.SerializeObject(dynamicsList, Formatting.Indented);
            string myfile = filesLogs + "//log" + DateTime.UtcNow.Ticks + ".json";

            // Appending the given texts
            try
            {
                File.WriteAllText(myfile, jsonString);
                EncryptFileCommand encryptFile = new EncryptFileCommand();
                encryptFile.EncryptFile(myfile);
                dynamicsList.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}