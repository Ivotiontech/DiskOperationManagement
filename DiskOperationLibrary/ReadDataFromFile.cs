using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskOperationLibrary
{
    public static class ReadDataFromFile
    {
        public static dynamic ReadFileForSpeceficData(string fileFullPath)
        {
            string filePath = fileFullPath;

            try
            {
                // Read all lines from the file
                string jsonData = File.ReadAllText(filePath);

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
