using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace WebApplication {
    public static class Core {


        public static Dictionary<string, int> DeviceDictionary;

        static Core() {
             DeviceDictionary = new Dictionary<string, int>();
        }

        public static string DeviceDictionaryPath { get {
            return Path.Combine(Directory.GetCurrentDirectory(), FilesFolderName, "device_dictionary");
        }}

        public static void InitDeviceDictionaryFromFile() {
            if (File.Exists(DeviceDictionaryPath)) {
                var serializer = new JsonSerializer();
                using (var str = new StreamReader(DeviceDictionaryPath)) {
                    using (var jsonReader = new JsonTextReader(str)) {
                        var dic = serializer.Deserialize(jsonReader);
                        if (dic != null) {
                            DeviceDictionary = (Dictionary<string, int>)dic;
                        }
                    }
                }
            }
        }

        public static void SaveDeviceDictionaryToFile() {
            var serializer = new JsonSerializer();
                using (var str = new StreamWriter(DeviceDictionaryPath)) {
                    using (var jsonWriter = new JsonTextWriter(str)) {
                        serializer.Serialize(jsonWriter, DeviceDictionary);
                    }
                }
        }

        public static string FilesFolderName { get {return "files";} }


        public static string AudiosFolderPath { get {
            return Path.Combine(Directory.GetCurrentDirectory(), FilesFolderName, "audios");
        }}

        public static string OggFolderPath { get {
            return Path.Combine(Directory.GetCurrentDirectory(), FilesFolderName, "ogg");
        }}

        public static string FilesFolderPath { get { 
            return Path.Combine(Directory.GetCurrentDirectory(), FilesFolderName);
        }}

        public static void MakeFilesFolder() {
            if (!Directory.Exists(FilesFolderPath)) {
                Directory.CreateDirectory(FilesFolderPath);
                if (!Directory.Exists(AudiosFolderPath)) {
                    Directory.CreateDirectory(AudiosFolderPath);
                }
                if (!Directory.Exists(OggFolderPath)) {
                    Directory.CreateDirectory(OggFolderPath);
                }
            }
        }

        public static string DeviceFolderPath(string deviceId) {
            return Path.Combine(FilesFolderPath, "device" + deviceId);
        }
        //Agregué esto
        public static string DeviceFolder(string device) {
            return Path.Combine(FilesFolderPath, device);
        }

        public static string DeviceAudiosFolderPath(string deviceId) {
            return Path.Combine(DeviceFolderPath(deviceId), "audios");
        }

        public static string DeviceOggFolderPath(string deviceId) {
            return Path.Combine(DeviceFolderPath(deviceId), "ogg");
        }

        public static void MakeDeviceFolder(string deviceId) {
            var deviceFolderPath = DeviceFolderPath(deviceId);
            if (!Directory.Exists(deviceFolderPath)) {
                Directory.CreateDirectory(deviceFolderPath);
                Directory.CreateDirectory(DeviceAudiosFolderPath(deviceId));
                Directory.CreateDirectory(DeviceOggFolderPath(deviceId));
            }
        }
        
    }
}