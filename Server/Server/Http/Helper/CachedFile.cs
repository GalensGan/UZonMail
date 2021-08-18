using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Http.Helper
{
    class CachedFile
    {
        public CachedFile(string fullName)
        {
            FileInfo fileInfo = new FileInfo(fullName);
            Length = fileInfo.Length;

            // 开启新线程，读取流
            new Task(() => {
                using (StreamReader reader = new StreamReader(fileInfo.Open(FileMode.Open)))
                {
                    
                }
            }).Start();
        }

        public string FullName { get; set; }
        public long MTimeMs { get; set; }
        public byte[] Byes { get; set; } 
        public long Length { get; private set; }
    }
}
