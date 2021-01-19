#if UNITY_EDITOR

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class MyPlug : OdinMenuEditorWindow
{
    [MenuItem("我的工具/My Simple Editor")]
    private static void OpenWindow()
    {
        var window = GetWindow<MyPlug>();

        // Nifty little trick to quickly position the window in the middle of the editor.
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: false)
        {
            //{ "Home",                           this,                           EditorIcons.House                       }, // Draws the this.someData field in this case.
            //{ "Odin Settings",                  null,                           EditorIcons.SettingsCog                 },
            //{ "Odin Settings/Color Palettes",   ColorPaletteManager.Instance,   EditorIcons.EyeDropper                  },
            //{ "Odin Settings/AOT Generation",   AOTGenerationConfig.Instance,   EditorIcons.SmartPhone                  },
            //{ "Player Settings",                Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault()       },
            {"热更新",null },
            {"热更新/说明", MyHotFix.Getinstate() },
            {"热更新/生成版本文件", MyAssetbundle.Instance },
        };
        return tree;
    }
}

public class MyHotFix : BaseManager<MyHotFix>
{
    [InfoBox("打开AssetBundle，打包完成之后生成版本文件")]
    public string info;
}

public class MyAssetbundle : GlobalConfig<MyAssetbundle>
{
    private string EnumConvertToString(BuildTarget color)
    {
        //方法一
        //return color.ToString();

        //方法二
        return Enum.GetName(color.GetType(), color);
    }

    [InfoBox("默认为平台为：AssetBundles/StandaloneWindows (需与Assetbundle一致)")]
    public BuildTarget builPlatform;

    private string outPath;

    [Button(ButtonSizes.Large)]
    public void 删除本地已经打包的文件_后续版本检查自动删除多余文件()
    {
        DeleteDir(Application.persistentDataPath);
        Debug.Log("删除文件完成");
    }

    /// <summary>
    /// 删除文件夹内所有文件（保留目录）
    /// </summary>
    /// <param name="file"></param>
    private static void DeleteDir(string file)
    {
        try
        {
            //去除文件夹和子文件的只读属性
            //去除文件夹的只读属性
            System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
            fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;

            //去除文件的只读属性
            System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);

            //判断文件夹是否还存在
            if (Directory.Exists(file))
            {
                foreach (string f in Directory.GetFileSystemEntries(file))
                {
                    if (File.Exists(f))
                    {
                        //如果有子文件删除文件
                        File.Delete(f);
                        Console.WriteLine(f);
                    }
                    else
                    {
                        //循环递归删除子文件夹
                        DeleteDir(f);
                    }
                }

                //删除空文件夹
                Directory.Delete(file);
                Console.WriteLine(file);
            }
        }
        catch (Exception ex) // 异常处理
        {
            Console.WriteLine(ex.Message.ToString());// 异常信息
        }
    }

    [Button(ButtonSizes.Large)]
    public void 重命名文件(string 后缀名 = "assetbundle")
    {
        outPath = Application.dataPath + "/../AssetBundles/" + builPlatform.ToString();
        string FilePath = outPath + "/" + builPlatform.ToString();
        Debug.Log("重命名文件路径：" + FilePath);
        if (File.Exists(FilePath))
        {
            File.Move(FilePath, outPath + "/" + builPlatform.ToString() + "." + 后缀名);
            Debug.Log("命名结束" + outPath + "/" + builPlatform.ToString() + "." + 后缀名);
        }
        else
        {
            Debug.LogError("文件不存在");
        }
    }

    [Button(ButtonSizes.Large)]
    public void 生成版本文件()
    {
       
        重命名文件();

        outPath = Application.dataPath + "/../AssetBundles/" + builPlatform.ToString();
        Debug.Log("版本文件构建平台为" + builPlatform);
        Debug.Log("版本文件所在路径为" + outPath);
        StringBuilder sb = new StringBuilder(); //版本文件要构建的内容

        string strVersionFilePath = outPath + "/VersionFile.txt";

        if (File.Exists(strVersionFilePath))
        {
            File.Delete(strVersionFilePath);
        }

        DirectoryInfo theFolder = new DirectoryInfo(outPath);//拿到文件夹

        // DirectoryInfo[] dirInfo = theFolder.GetDirectories();//遍历文件夹下所有文件，拿到文件名

        FileInfo[] arrFiles = theFolder.GetFiles("*", SearchOption.AllDirectories);
        foreach (var item in arrFiles)
        {
            FileInfo file = item;
            string fullName = file.FullName;//全名， 包括路径扩展名例如，C：D：等

            string name = fullName.Substring(fullName.IndexOf("AssetBundles"));//相对路径

            Debug.Log("各个文件的路径：" + fullName);

            string md5 = Mima.GetMD5HashFromFile(fullName);
            if (md5 == null)
            {
                Debug.LogError("有文件没有拿到MD5：" + item.Name);
                continue;
            }
            string size = Mathf.Ceil(file.Length / 1024f).ToString();//文件大小
            //finish 检测是否是初始数据
            string strLine = string.Format("{0} {1} {2}", name, md5, size); //每个文件的版本信息

            sb.AppendLine(strLine);//写入版本文件要构建的内容中,按行写入
        }
        bool isFirstData;
        //File.WriteAllText(strVersionFilePath, sb.ToString());//存到文件中，如果不存在会自动创建
        IOUtil.CreatTextFile(strVersionFilePath, sb.ToString());
        Debug.Log("创建版本文件成功");
        //File.Create()
    }
}

#endif