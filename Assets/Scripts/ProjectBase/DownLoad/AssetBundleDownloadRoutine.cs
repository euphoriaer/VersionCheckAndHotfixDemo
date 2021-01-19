using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/// <summary>
/// 下载器
/// </summary>
public class AssetBundleDownloadRoutine : MonoBehaviour
{
    private List<DownLoadDataEnety> m_List = new List<DownLoadDataEnety>();
    private DownLoadDataEnety m_CurrentDownloadData;

    private int needDownloadCount;
   
    public int NeedDownloadCount { get => needDownloadCount; private set => needDownloadCount = value; }

    /// <summary>
    /// 当前下载器已经下载的总数量
    /// </summary>
    public int CompleteCount { get => completeCount; private set => completeCount = value; }

    /// <summary>
    /// 当前下载器已经下载的总大小
    /// </summary>
    public int DownloadSize { get => m_downloadSize; }
    /// <summary>
    /// 当前文件下载的大小
    /// </summary>
    public int CurrentDownLoadSize { get => m_currentDownLoadSize; }

    private int completeCount;

    private int m_downloadSize;

    private int m_currentDownLoadSize;

    /// <summary>
    /// 是否开始下载
    /// </summary>
    public bool IsStarDownLoad
    {
        private set;
        get;
    }
    /// <summary>
    /// 添加下载对象
    /// </summary>
    /// <param name="downLoadDataEnety"></param>
    public void AddDownload(DownLoadDataEnety downLoadDataEnety)
    {

        m_List.Add(downLoadDataEnety);
    }

    public void StarDownload()
    {
        IsStarDownLoad = true;
        needDownloadCount = m_List.Count;
    }
    private void Update()
    {
        if (IsStarDownLoad)
        {
            IsStarDownLoad = false;
            StartCoroutine(DownloadData());
        }


    }

    private IEnumerator DownloadData()
    {
        if (needDownloadCount == 0)
        {
            yield break;
        }

        m_CurrentDownloadData = m_List[0];
        m_currentDownLoadSize = m_List[0].Size;
        //短路径 用来创建文件夹
        string path = m_CurrentDownloadData.FullName.Substring(0, m_CurrentDownloadData.FullName.LastIndexOf('\\'));//第一个 \对第二个\进行转义，否则无法定位 \    
        string dataurl = DownLoadMgr.Getinstate().resourcesURL+ m_CurrentDownloadData.FullName.Replace('\\','/');//资源下载路径,将路径\ 转为下载网址的 /
        
        Debug.Log("下载器所下载的资源链接：" + dataurl);
        

        //本地路径 创建文件夹
        string locaFilePath = DownLoadMgr.Getinstate().LocalFilePath + path;
        Debug.Log("创建文件夹"+ locaFilePath);
        if (!Directory.Exists(locaFilePath))
        {
            Directory.CreateDirectory(locaFilePath);
        }
    
        UnityWebRequest webRequest = UnityWebRequest.Get(dataurl);
        yield return webRequest;// 等待资源下载
        webRequest.SendWebRequest();
        float timeOut = Time.time;
        float progress = webRequest.downloadProgress;//下载进入0-1
        if (webRequest.isNetworkError || webRequest.isHttpError)                                                             //如果出错
        {
            Debug.Log(webRequest.error); //输出 错误信息
        }
        else
        {
            while (!webRequest.isDone) //只要下载没有完成，一直执行此循环
            {
                timeOut = Time.time;
                progress = webRequest.downloadProgress;
                Debug.Log("正在下载：");
                yield return 0;
            }

            if (webRequest.isDone) //如果下载完成了
            {
                //print("下载完成："+ dataurl);
                //CompleteCount++;
                //m_downloadSize += m_currentDownLoadSize;
            }
        }
        //存文件
        if (webRequest!=null&&webRequest.error==null)
        {

            byte[] results = webRequest.downloadHandler.data;
            //Debug.Log("下载完成的文件路径：" + DownLoadMgr.Getinstate().LocalFilePath + m_CurrentDownloadData.FullName.Replace('\\', '/'));
            IOUtil.CreateFile(DownLoadMgr.Getinstate().LocalFilePath + m_CurrentDownloadData.FullName.Replace('\\', '/'), results, webRequest.downloadHandler.data.Length);


        }

        m_downloadSize += m_currentDownLoadSize;//大小增加
        completeCount++;//数量增加
        //下载成功,重置
        m_currentDownLoadSize = 0;

        //修改版本文件，没有则创建版本文件
        DownLoadMgr.Getinstate().ModifyLocalData(m_CurrentDownloadData);

        m_List.RemoveAt(0);//移除第一个下载完成的任务
        
        if (m_List.Count==0)
        {
            m_List.Clear();
        }
        else
        {
            IsStarDownLoad = true;
        }
        

    }
}