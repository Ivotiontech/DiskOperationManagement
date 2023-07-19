using System.Data;
using System;
using System.IO;
using PacketDotNet;
using SharpPcap;
using System.Runtime.CompilerServices;
using System.Management;

namespace DiskOperationService
{
    public class Worker : BackgroundService
    {
        int isStart = 0;
        static string externaldrive = string.Empty;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);

                //string externaldrive = string.Empty;
                //WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_LogicalDisk'");

                //// Create a management event watcher and subscribe to events
                //ManagementEventWatcher Managewatcher = new ManagementEventWatcher(query);
                //Managewatcher.EventArrived += (sender, eventArgs) =>
                //{
                //    // Extract the instance of the event
                //    ManagementBaseObject instance = (ManagementBaseObject)eventArgs.NewEvent["TargetInstance"];

                //    // Check if the drive type is "2" (Removable disk)
                //    if (instance.Properties["DriveType"].Value.ToString() == "2")
                //    {
                //        // Extract the drive letter
                //        externaldrive = instance.Properties["DeviceID"].Value.ToString();

                //        // Perform your desired actions when an external drive is connected
                //        Console.WriteLine("External drive connected: " + externaldrive);

                //        if (!string.IsNullOrEmpty(externaldrive))
                //        {
                //            if (System.IO.Directory.Exists(externaldrive)) {
                //                FileSystemWatcher watcher1 = new FileSystemWatcher(externaldrive);

                //                // Set the properties to monitor
                //                watcher1.IncludeSubdirectories = true; // Monitor subdirectories as well
                //                watcher1.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;

                //                // Set the events to track
                //                watcher1.Created += OnCreated1;

                //                // Start monitoring
                //                watcher1.EnableRaisingEvents = true;
                //            }

                //        }


                //    }
                //};
                //Managewatcher.Start();

                //Console.WriteLine("Listening for external drive events. Press Enter to exit.");

                //// Stop listening for events
                ////Managewatcher.Stop();

                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    // Check if the drive is ready and not a network drive
                    if (drive.IsReady && drive.DriveType != DriveType.Network)
                    {
                        // Exclude external drives (DriveType.Removable)
                        if (drive.DriveType != DriveType.Removable && isStart == 0)
                        {
                            // Perform your desired actions with the non-external drive
                            //Console.WriteLine("Drive: " + drive.Name);
                            string systemDrive = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 3);
                            if (drive.Name == systemDrive) continue;

                            // Specify the directory path to monitor
                            string directoryPath = drive.Name;

                            // Create a new FileSystemWatcher instance
                            FileSystemWatcher watcher = new FileSystemWatcher(directoryPath);

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
                            externaldrive = drive.Name;

                            // Create a new FileSystemWatcher instance
                            FileSystemWatcher watcher = new FileSystemWatcher(externaldrive);

                            // Set the properties to monitor
                            watcher.IncludeSubdirectories = true; // Monitor subdirectories as well
                            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;

                            // Set the events to track
                            watcher.Created += OnCreated1;

                            // Start monitoring
                            watcher.EnableRaisingEvents = true;
                        }
                    }
                }
                isStart = 1;
                //Console.WriteLine("Press enter to stop monitoring.");
                //Console.ReadLine();


                // Stop monitoring
                //watcher.EnableRaisingEvents = false;


            }
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
                string myfile = @"C:/log.txt";

                // Appending the given texts
                using (StreamWriter sw = File.AppendText(myfile))
                {
                    sw.WriteLine($"Folder created: {e.FullPath}");
                }

                Console.WriteLine($"Folder created: {e.FullPath}");
            }

        }
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                string myfile = @"C:/log.txt";

                // Appending the given texts
                using (StreamWriter sw = File.AppendText(myfile))
                {
                    sw.WriteLine($"Folder created: {e.FullPath}");
                }
                Console.WriteLine($"Folder modified: {e.FullPath}");
            }

        }
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                string myfile = @"C:/log.txt";

                // Appending the given texts
                using (StreamWriter sw = File.AppendText(myfile))
                {
                    sw.WriteLine($"Folder created: {e.FullPath}");
                }
                Console.WriteLine($"Folder deleted: {e.FullPath}");
            }
        }
        private static void OnRenamed(object sender, RenamedEventArgs e)
        {

            if (e.OldFullPath.EndsWith("\\"))
            {
                string myfile = @"C:/log.txt";

                // Appending the given texts
                using (StreamWriter sw = File.AppendText(myfile))
                {
                    sw.WriteLine($"Folder created: {e.FullPath}");
                }
                Console.WriteLine($"Folder renamed: {e.OldFullPath} -> {e.FullPath}");
            }
            else
            {
                string myfile = @"C:/log.txt";

                // Appending the given texts
                using (StreamWriter sw = File.AppendText(myfile))
                {
                    sw.WriteLine($"Folder created: {e.FullPath}");
                }
                Console.WriteLine($"File renamed: {e.OldFullPath} -> {e.FullPath}");
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
    }
}