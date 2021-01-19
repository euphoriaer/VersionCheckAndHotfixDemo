using System.IO;
using UnityEngine;
public static class IOUtil
{

    /// <summary>
    /// 创建txt文件的方法
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="content"></param>
    public static void CreatTextFile(string filePath, string content)
    {
        File.Delete(filePath);//文件存在则删除
        using (FileStream fs = File.Create(filePath))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(content.ToString());
            }
        }
    }

    /// <summary>
    /// 创建文件的方法
    /// </summary>
    /// <param name="path"></param>
    /// <param name="bytes"></param>
    /// <param name="length"></param>
    public static void CreateFile(string path, byte[] bytes, int length)
    {
        Stream sw;
        FileInfo file = new FileInfo(path);
        if (!file.Exists)
        {
            sw = file.Create();
        }
        else
        {
            Debug.Log("存在文件");
            return;
        }

        sw.Write(bytes, 0, length);
        sw.Close();
        sw.Dispose();
    }


}