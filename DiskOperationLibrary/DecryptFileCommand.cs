using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiskOperationLibrary
{
    public class DecryptFileCommand
    {
        readonly CspParameters _cspp = new CspParameters();
        RSACryptoServiceProvider _rsa;

        const string DecrFolder = @"F:\Aditya\Disk_Operations_Monitoring\DiskOperationManagement\DiskOperationService\logs";
        const string KeyName = "Key01";

        private void CreateAsmKeys()
        {
            // Stores a key pair in the key container.
            _cspp.KeyContainerName = KeyName;
            _rsa = new RSACryptoServiceProvider(_cspp)
            {
                PersistKeyInCsp = true
            };
        }
        public void DecryptFile(string fileInfo)
        {
            try
            {
                CreateAsmKeys();

                // Create instance of Aes for
                // symmetric decryption of the data.
                Aes aes = Aes.Create();
                var fileName = new FileInfo(fileInfo);
                var fileFullName = Path.GetFullPath(fileInfo);
                var fileFullNameChanged = fileName.Directory.FullName + "\\" + Path.GetFileNameWithoutExtension(fileInfo) + 1 + fileName.Extension;
                // Create byte arrays to get the length of
                // the encrypted key and IV.
                // These values were stored as 4 bytes each
                // at the beginning of the encrypted package.
                byte[] LenK = new byte[4];
                byte[] LenIV = new byte[4];

                // Construct the file name for the decrypted file.
                string outFile =
                    Path.ChangeExtension(fileFullNameChanged, fileName.Extension);

                // Use FileStream objects to read the encrypted
                // file (inFs) and save the decrypted file (outFs).
                using (var inFs = new FileStream(fileFullName, FileMode.Open))
                {
                    inFs.Seek(0, SeekOrigin.Begin);
                    inFs.Read(LenK, 0, 3);
                    inFs.Seek(4, SeekOrigin.Begin);
                    inFs.Read(LenIV, 0, 3);

                    // Convert the lengths to integer values.
                    int lenK = BitConverter.ToInt32(LenK, 0);
                    int lenIV = BitConverter.ToInt32(LenIV, 0);

                    // Determine the start position of
                    // the cipher text (startC)
                    // and its length(lenC).
                    int startC = lenK + lenIV + 8;
                    int lenC = (int)inFs.Length - startC;

                    // Create the byte arrays for
                    // the encrypted Aes key,
                    // the IV, and the cipher text.
                    byte[] KeyEncrypted = new byte[lenK];
                    byte[] IV = new byte[lenIV];

                    // Extract the key and IV
                    // starting from index 8
                    // after the length values.
                    inFs.Seek(8, SeekOrigin.Begin);
                    inFs.Read(KeyEncrypted, 0, lenK);
                    inFs.Seek(8 + lenK, SeekOrigin.Begin);
                    inFs.Read(IV, 0, lenIV);

                    Directory.CreateDirectory(DecrFolder);
                    // Use RSACryptoServiceProvider
                    // to decrypt the AES key.
                    byte[] KeyDecrypted = _rsa.Decrypt(KeyEncrypted, false);

                    // Decrypt the key.
                    ICryptoTransform transform = aes.CreateDecryptor(KeyDecrypted, IV);

                    // Decrypt the cipher text from
                    // from the FileSteam of the encrypted
                    // file (inFs) into the FileStream
                    // for the decrypted file (outFs).
                    using (var outFs = new FileStream(outFile, FileMode.Create))
                    {
                        int count = 0;
                        int offset = 0;

                        // blockSizeBytes can be any arbitrary size.
                        int blockSizeBytes = aes.BlockSize / 8;
                        byte[] data = new byte[blockSizeBytes];

                        // By decrypting a chunk a time,
                        // you can save memory and
                        // accommodate large files.

                        // Start at the beginning
                        // of the cipher text.
                        inFs.Seek(startC, SeekOrigin.Begin);
                        using (var outStreamDecrypted =
                            new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                        {
                            do
                            {
                                count = inFs.Read(data, 0, blockSizeBytes);
                                offset += count;
                                outStreamDecrypted.Write(data, 0, count);
                            } while (count > 0);

                            outStreamDecrypted.FlushFinalBlock();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
