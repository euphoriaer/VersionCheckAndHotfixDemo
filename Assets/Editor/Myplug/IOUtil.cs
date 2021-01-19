using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


   public static class IOUtil
    {

        public static void CreatTextFile(string filePath,string content)
        {
            File.Delete(filePath);//文件存在则删除
            using(FileStream fs = File.Create(filePath))
            {
                using(StreamWriter sw=new StreamWriter(fs))
                {
                    sw.WriteLine(content.ToString());
                }
            }
        }
    }

