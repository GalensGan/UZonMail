using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    public abstract class ManagerBase
    {
        public ManagerBase(Config config) { this.Config = config; }
        protected Config Config { get; set; }

        public bool Save(string path, Object obj)
        {
            // 检查父目录是否存在
            string dir = Path.GetDirectoryName(path);
            Directory.CreateDirectory(dir);

            string content = JsonConvert.SerializeObject(obj);
            using (Stream stream = File.Create(path))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                    writer.Close();
                }
                stream.Close();
            }
            return true;
        }

        public abstract bool Save();

        public TOut ReadData<TOut, TJson>(string path) where TJson : JContainer
        {
            // 判断文件是否存在
            if (!File.Exists(path)) return default;

            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                Object obj = JsonConvert.DeserializeObject(content);
                reader.Close();

                if (obj is TJson Tobj)
                {
                    return (TOut)Tobj.ToObject(typeof(TOut));
                }

            }
            return default;
        }

        public List<string> GetTableNames(DataTable table)
        {
            List<string> names = new List<string>();
            IEnumerator enumerator = table.Columns.GetEnumerator();
            while (enumerator.MoveNext())
            {
                names.Add((enumerator.Current as DataColumn).ColumnName);
            }
            return names;
        }
    }
}
