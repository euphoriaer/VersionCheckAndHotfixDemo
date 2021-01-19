using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadTest : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        DownLoadMgr.Getinstate().InitCheckVersion("http://localhost:8081/AssetBundles/StandaloneWindows/VersionFile.txt",successCheck);
        //StartCoroutine(DownLoadVersion("http://localhost:8081/StandaloneWindows/VersionFile.txt"));
    }

    private void successCheck()
    {
        throw new NotImplementedException();
    }

    //private IEnumerator DownLoadVersion(string url)
    //{
    //    UnityWebRequest uwr = UnityWebRequest.Get(url);
    //    yield return uwr;//finish 等待资源下载
    //    uwr.SendWebRequest();
    //    if (uwr.isNetworkError || uwr.isHttpError)                                                             //如果出错
    //    {
    //        Debug.Log(uwr.error); //输出 错误信息
    //    }
    //    else
    //    {
    //        while (!uwr.isDone) //只要下载没有完成，一直执行此循环
    //        {
    //            Debug.Log("正在下载"+uwr.downloadProgress);
    //            yield return 0;
    //        }

    //        if (uwr.isDone) //如果下载完成了
    //        {
    //            print("完成");

    //        }

    //        string results = uwr.downloadHandler.text;
    //        Debug.Log(results);
    //    }
    //}
}