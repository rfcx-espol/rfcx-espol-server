using System.IO;

namespace WebApplication {
    public static class Core {

        public static string FilesFolderName { get {return "files";} }
        public static string FilesFolderPath { get { 
            return Path.Combine(Directory.GetCurrentDirectory(), FilesFolderName);
        }}

        public static void MakeFilesFolder() {
            if (!Directory.Exists(FilesFolderPath)) {
                Directory.CreateDirectory(FilesFolderPath);
            }
        }

        
    }
}