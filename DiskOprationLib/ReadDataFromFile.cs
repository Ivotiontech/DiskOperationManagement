using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiskOprationLib
{
    public static class ReadDataFromFile
    {
        public static dynamic ReadFileForSpeceficData(string fileFullPath)
        {
            try
            {
                var fileinfo = new FileInfo(fileFullPath);
                // Read all lines from the file
                string jsonData = File.ReadAllText(fileFullPath);

                // Deserialize the JSON data into a dynamic object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);

                // Process each line to find the specific data

                return jsonObject;
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
    }
}
