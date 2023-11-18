using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Helpers
{
    public interface IStorageJSONService<T>
    {
        Task<T> ReadFromFileAsync(string directoryName, string fileName);
        Task WriteToDataFileAsync(string directoryName, string fileName, T data);
    }

    public class StorageJSONService<T>
    {
        private readonly IStorageUtility storageUtility;

        public StorageJSONService(IStorageUtility storageUtility)
        {
            this.storageUtility = storageUtility;
        }

        /// <summary>
        /// Loads data from a file
        /// </summary>
        /// <param name="fileName">Name of the file to read.</param>
        /// <returns>Data object</returns>
        public async Task<T> ReadFromFileAsync(string directoryName, string fileName)
        {
            //T loadedFile = default(T);
            T loadedFile = (T)Activator.CreateInstance(typeof(T));
            string tempStr = "";
            try
            {
                tempStr = await storageUtility.ReadFromDataFileAsync(directoryName, fileName);
                var temploadedFile = JsonConvert.DeserializeObject<T>(tempStr);
                if (temploadedFile != null)
                    loadedFile = temploadedFile;
            }
            catch
            {
                //ApplicationState.ErrorLog.Add(new ErrorLog("LoadFromFile", e.Message));
            }

            return loadedFile;
        }

        public static T LoadFromString(string SourceString)
        {
            T loadedFile = (T)Activator.CreateInstance(typeof(T));
            try
            {
                loadedFile = JsonConvert.DeserializeObject<T>(SourceString);
            }
            catch
            {
                //ApplicationState.ErrorLog.Add(new ErrorLog("LoadFromFile", e.Message));
            }

            return loadedFile;
        }

        /// <summary>
        /// Saves data to a file.
        /// </summary>
        /// <param name="fileName">Name of the file to write to</param>
        /// <param name="data">The data to save</param>
        public async Task WriteToDataFileAsync(string directoryName, string fileName, T data)
        {
            try
            {
                string output = JsonConvert.SerializeObject(data);
                await storageUtility.WriteToDataFileAsync(directoryName, fileName, output);
            }
            catch(Exception e)
            {
                // Add desired error handling for your application
                // ApplicationState.ErrorLog.Add(new ErrorLog("SaveToFile", e.Message));
            }
        }

    }
}
