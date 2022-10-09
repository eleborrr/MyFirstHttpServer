using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace googleHW
{
    internal class FileInspector
    {
        
        public byte[] getFile(string rawUrl, string _filePath)
        {
            byte[] buffer = null;
            var filePath = _filePath + rawUrl;

            if (Directory.Exists(filePath))
            {
                filePath = filePath + "/index.html";
                if (File.Exists(filePath))
                    buffer = File.ReadAllBytes(filePath);

            }
            else if (File.Exists(filePath))
                buffer = File.ReadAllBytes(filePath);
            return buffer;
        }

        public string getContentType(string rawUrl)
        {
            var images = new string[] { "jpg", "jpeg", "png" };
            var text = new string[] { "html", "css", "xml" };
            var fileExtension = rawUrl.Split(".").Last();
            if (images.Contains(fileExtension))
                return "image/" + fileExtension;
            else if (text.Contains(fileExtension))
                return "text/" + fileExtension;
            else return "text/html";
        }
    }
}
