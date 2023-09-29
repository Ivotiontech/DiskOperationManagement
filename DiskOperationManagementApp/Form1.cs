using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.ServiceProcess;
using System.Security.Cryptography;
using System.Reflection.Emit;
using System.IO;
using DiskOprationLib;
using Newtonsoft.Json;
using System.ServiceProcess;
using System.Threading;

namespace DiskOperationManagementApp
{
    public partial class Form1 : Form
    {

        readonly CspParameters _cspp = new CspParameters();
        RSACryptoServiceProvider _rsa;
        private ServiceController serviceController;
        string serviceName = "LegalDLP-Beta";

        private readonly string fileFullName = @"LicenseKey.txt";
        public Form1()
        {
            InitializeComponent();

        }

        private async void btnAccess_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            btnAccess.Enabled = false;
            var fileinfo = new FileInfo(fileFullName);
            var lic = txtAccessKey.Text;

            //var jsonData = ReadDataFromFile.ReadFileForSpeceficData(fileFullName);
            var param = new { lic = lic };
            var dt = await DiskOperationApiRequest.PostDiskOperationApi(param, "get-license-data");
            var statusCode = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)dt).First.Next).Value).Value;
            if (statusCode.ToString() == "200")
            {
                var fileDirectory = fileinfo.Directory.FullName.Replace("LegalloggerApp", "LegalDLP-Beta") + "\\" + fileFullName;
                string jsonString = JsonConvert.SerializeObject(param, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(fileDirectory, jsonString.ToString());
                //EncryptFileCommand encryptFile = new EncryptFileCommand();
                //encryptFile.EncryptFile(fileDirectory);
                InstallWorkerService();
                //this.Close();
            }
            else
            {
                MessageBox.Show("Not a valid licence key");
            }
        }

        private void InstallWorkerService()
        {
            string servicePath = Directory.GetCurrentDirectory().Replace("LegalloggerApp", "LegalDLP-Beta\\LegalDLP-Beta.exe");

            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    if (!ServiceExists(serviceName))
                    {
                        // Use the "sc create" command to install the service.
                        psi = new ProcessStartInfo("sc", $"create {serviceName} binPath= \"{servicePath}\"");
                        psi.Verb = "runas"; // Run as administrator.
                        Process.Start(psi);
                        //MessageBox.Show("1");
                        Thread.Sleep(5000);
                        sc.Start();
                        Thread.Sleep(5000);
                        //MessageBox.Show("2");
                        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                        MessageBox.Show("Service started successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {

                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                            psi = new ProcessStartInfo("sc", $"delete {serviceName}");
                            psi.Verb = "runas"; // Run as administrator.
                            Process.Start(psi);
                        }
                        else
                        {
                            psi = new ProcessStartInfo("sc", $"delete {serviceName}");
                            psi.Verb = "runas"; // Run as administrator.
                            Process.Start(psi);
                            Thread.Sleep(5000);
                        }
                        // Use the "sc create" command to install the service.
                        psi = new ProcessStartInfo("sc", $"create {serviceName} binPath= \"{servicePath}\"");
                        psi.Verb = "runas"; // Run as administrator.
                        Process.Start(psi);
                        //MessageBox.Show("3");
                        Thread.Sleep(5000);
                        sc.Start();
                        //MessageBox.Show("4");
                        Thread.Sleep(5000);
                        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        MessageBox.Show("Service started successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        private bool ServiceExists(string serviceName)
        {
            return ServiceController.GetServices().Any(service => service.ServiceName == serviceName);
        }
    }
}
