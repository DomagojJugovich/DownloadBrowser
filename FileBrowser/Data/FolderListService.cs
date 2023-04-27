using Microsoft.AspNetCore.Hosting.Server;

namespace FileBrowser.Data
{
    public class FolderListService
    {
      

        public IEnumerable<String> GetFiles()
        {

            string rootpath = AppDomain.CurrentDomain.BaseDirectory;

            rootpath =  System.IO.Path.Combine(rootpath, "wwwroot");
            rootpath =  System.IO.Path.Combine(rootpath, "packages");

            return Directory.EnumerateFiles(rootpath);
        
        }
    }
}