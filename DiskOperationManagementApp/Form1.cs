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

namespace DiskOperationManagementApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            InstallWorkerService();
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            string serviceName = "DiskOperationManagement";
            ProcessStartInfo psi = new ProcessStartInfo("sc", $"delete {serviceName}");
            psi.Verb = "runas"; // Run as administrator.
            Process.Start(psi);
        }

        private void InstallWorkerService()
        {
            // Replace "DiskOperationManagement" with your desired service name.
            string serviceName = "DiskOperationManagement";

            // Replace "PathToYourWorkerServiceExecutable" with the full path to the Worker Service executable.
            string servicePath = "E:\\IvotionTech\\Project\\DiskOperationManagement\\DiskOperationService\\bin\\Debug\\net6.0\\DiskOperationService.exe";

            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (!ServiceExists(serviceName))
                    {
                        // Use the "sc create" command to install the service.
                        ProcessStartInfo psi = new ProcessStartInfo("sc", $"create {serviceName} binPath= \"{servicePath}\"");
                        psi.Verb = "runas"; // Run as administrator.
                        Process.Start(psi);
                    }
                    else
                    {
                        MessageBox.Show("Service already exists.");
                    }
                }
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
