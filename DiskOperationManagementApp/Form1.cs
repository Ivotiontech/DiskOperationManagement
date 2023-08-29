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

namespace DiskOperationManagementApp
{
    public partial class Form1 : Form
    {

        readonly CspParameters _cspp = new CspParameters();
        RSACryptoServiceProvider _rsa;
        private ServiceController serviceController;
        string serviceName = "DiskOperationManagement";

        private readonly string fileFullName = @"LicenseKey.txt";
        public Form1()
        {
            InitializeComponent();

        }

        //private void btnInstall_Click(object sender, EventArgs e)
        //{
        //    InstallWorkerService();
        //}

        //private void btnUninstall_Click(object sender, EventArgs e)
        //{
        //    string serviceName = "DiskOperationManagement";
        //    ProcessStartInfo psi = new ProcessStartInfo("sc", $"delete {serviceName}");
        //    psi.Verb = "runas"; // Run as administrator.
        //    Process.Start(psi);
        //}





        //private void CreateAsmKeys()
        //{
        //    // Stores a key pair in the key container.
        //    _cspp.KeyContainerName = KeyName;
        //    _rsa = new RSACryptoServiceProvider(_cspp)
        //    {
        //        PersistKeyInCsp = true
        //    };
        //}

        //private void Encrypt_Click(object sender, EventArgs e)
        //{
        //    CreateAsmKeys();
        //    var logfile = "F:\\Encrypt\\log.txt";
        //    EncryptFile(logfile);
        //}


        //private void EncryptFile(string fileInfo)
        //{
        //    // Create instance of Aes for
        //    // symmetric encryption of the data.
        //    var fileName = Path.GetFileName(fileInfo);
        //    var fileFullName = Path.GetFullPath(fileInfo);
        //    FileInfo file = new FileInfo(fileName);
        //    Aes aes = Aes.Create();
        //    ICryptoTransform transform = aes.CreateEncryptor();

        //    // Use RSACryptoServiceProvider to
        //    // encrypt the AES key.
        //    // rsa is previously instantiated:
        //    //    rsa = new RSACryptoServiceProvider(cspp);
        //    byte[] keyEncrypted = _rsa.Encrypt(aes.Key, false);

        //    // Create byte arrays to contain
        //    // the length values of the key and IV.
        //    int lKey = keyEncrypted.Length;
        //    byte[] LenK = BitConverter.GetBytes(lKey);
        //    int lIV = aes.IV.Length;
        //    byte[] LenIV = BitConverter.GetBytes(lIV);

        //    // Write the following to the FileStream
        //    // for the encrypted file (outFs):
        //    // - length of the key
        //    // - length of the IV
        //    // - encrypted key
        //    // - the IV
        //    // - the encrypted cipher content

        //    // Change the file's extension to ".enc"
        //    string outFile =
        //        Path.Combine(EncrFolder, Path.ChangeExtension(file.Name, ".enc"));

        //    using (var outFs = new FileStream(outFile, FileMode.Create))
        //    {
        //        outFs.Write(LenK, 0, 4);
        //        outFs.Write(LenIV, 0, 4);
        //        outFs.Write(keyEncrypted, 0, lKey);
        //        outFs.Write(aes.IV, 0, lIV);

        //        // Now write the cipher text using
        //        // a CryptoStream for encrypting.
        //        using (var outStreamEncrypted =
        //            new CryptoStream(outFs, transform, CryptoStreamMode.Write))
        //        {
        //            // By encrypting a chunk at
        //            // a time, you can save memory
        //            // and accommodate large files.
        //            int count = 0;
        //            int offset = 0;

        //            // blockSizeBytes can be any arbitrary size.
        //            int blockSizeBytes = aes.BlockSize / 8;
        //            byte[] data = new byte[blockSizeBytes];
        //            int bytesRead = 0;

        //            using (var inFs = new FileStream(fileFullName, FileMode.Open))
        //            {
        //                do
        //                {
        //                    count = inFs.Read(data, 0, blockSizeBytes);
        //                    offset += count;
        //                    outStreamEncrypted.Write(data, 0, count);
        //                    bytesRead += blockSizeBytes;
        //                } while (count > 0);
        //            }
        //            outStreamEncrypted.FlushFinalBlock();
        //        }
        //    }
        //    Directory.CreateDirectory(EncrFolder);
        //    using (var sw = new StreamWriter(PubKeyFile, false))
        //    {
        //        sw.Write(_rsa.ToXmlString(false));
        //    }
        //}

        //private void Decypt_Click(object sender, EventArgs e)
        //{
        //    var logfile = "F:\\Encrypt\\log.enc";
        //    DecryptFile(logfile);
        //}

        //private void DecryptFile(string fileInfo)
        //{
        //    _cspp.KeyContainerName = KeyName;
        //    _rsa = new RSACryptoServiceProvider(_cspp)
        //    {
        //        PersistKeyInCsp = true
        //    };

        //    // Create instance of Aes for
        //    // symmetric decryption of the data.
        //    Aes aes = Aes.Create();
        //    var fileName = Path.GetFileName(fileInfo);
        //    var fileFullName = Path.GetFullPath(fileInfo);
        //    // Create byte arrays to get the length of
        //    // the encrypted key and IV.
        //    // These values were stored as 4 bytes each
        //    // at the beginning of the encrypted package.
        //    byte[] LenK = new byte[4];
        //    byte[] LenIV = new byte[4];

        //    // Construct the file name for the decrypted file.
        //    string outFile =
        //        Path.ChangeExtension(fileFullName.Replace("log","log1"), ".txt");

        //    // Use FileStream objects to read the encrypted
        //    // file (inFs) and save the decrypted file (outFs).
        //    using (var inFs = new FileStream(fileFullName, FileMode.Open))
        //    {
        //        inFs.Seek(0, SeekOrigin.Begin);
        //        inFs.Read(LenK, 0, 3);
        //        inFs.Seek(4, SeekOrigin.Begin);
        //        inFs.Read(LenIV, 0, 3);

        //        // Convert the lengths to integer values.
        //        int lenK = BitConverter.ToInt32(LenK, 0);
        //        int lenIV = BitConverter.ToInt32(LenIV, 0);

        //        // Determine the start position of
        //        // the cipher text (startC)
        //        // and its length(lenC).
        //        int startC = lenK + lenIV + 8;
        //        int lenC = (int)inFs.Length - startC;

        //        // Create the byte arrays for
        //        // the encrypted Aes key,
        //        // the IV, and the cipher text.
        //        byte[] KeyEncrypted = new byte[lenK];
        //        byte[] IV = new byte[lenIV];

        //        // Extract the key and IV
        //        // starting from index 8
        //        // after the length values.
        //        inFs.Seek(8, SeekOrigin.Begin);
        //        inFs.Read(KeyEncrypted, 0, lenK);
        //        inFs.Seek(8 + lenK, SeekOrigin.Begin);
        //        inFs.Read(IV, 0, lenIV);

        //        Directory.CreateDirectory(DecrFolder);
        //        // Use RSACryptoServiceProvider
        //        // to decrypt the AES key.
        //        byte[] KeyDecrypted = _rsa.Decrypt(KeyEncrypted, false);

        //        // Decrypt the key.
        //        ICryptoTransform transform = aes.CreateDecryptor(KeyDecrypted, IV);

        //        // Decrypt the cipher text from
        //        // from the FileSteam of the encrypted
        //        // file (inFs) into the FileStream
        //        // for the decrypted file (outFs).
        //        using (var outFs = new FileStream(outFile, FileMode.Create))
        //        {
        //            int count = 0;
        //            int offset = 0;

        //            // blockSizeBytes can be any arbitrary size.
        //            int blockSizeBytes = aes.BlockSize / 8;
        //            byte[] data = new byte[blockSizeBytes];

        //            // By decrypting a chunk a time,
        //            // you can save memory and
        //            // accommodate large files.

        //            // Start at the beginning
        //            // of the cipher text.
        //            inFs.Seek(startC, SeekOrigin.Begin);
        //            using (var outStreamDecrypted =
        //                new CryptoStream(outFs, transform, CryptoStreamMode.Write))
        //            {
        //                do
        //                {
        //                    count = inFs.Read(data, 0, blockSizeBytes);
        //                    offset += count;
        //                    outStreamDecrypted.Write(data, 0, count);
        //                } while (count > 0);

        //                outStreamDecrypted.FlushFinalBlock();
        //            }
        //        }
        //    }
        //}

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
                var fileDirectory = fileinfo.Directory.FullName.Replace("DiskOperationManagementApp\\bin\\Debug", "DiskOperationService\\bin\\Debug\\net6.0") + "\\" + fileFullName;
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
            string servicePath = Directory.GetCurrentDirectory().Replace("DiskOperationManagementApp\\bin\\Debug", "DiskOperationService\\bin\\Debug\\net6.0\\DiskOperationService.exe");

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
                        sc.Start();
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
                        }
                        // Use the "sc create" command to install the service.
                        psi = new ProcessStartInfo("sc", $"create {serviceName} binPath= \"{servicePath}\"");
                        psi.Verb = "runas"; // Run as administrator.
                        Process.Start(psi);
                        sc.Start();
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
