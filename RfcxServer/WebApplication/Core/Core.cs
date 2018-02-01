using System.IO;

namespace WebApplication {
    public static class Core {

        public static string FilesFolderName { get {return "files\\device1";} }

        public static string GzipFolderPath { get {
            return Path.Combine(Directory.GetCurrentDirectory(), FilesFolderName, "gzip");
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
                if (!Directory.Exists(GzipFolderPath)) {
                    Directory.CreateDirectory(GzipFolderPath);
                }
                if (!Directory.Exists(OggFolderPath)) {
                    Directory.CreateDirectory(OggFolderPath);
                }
            }
        }

        
    }
}