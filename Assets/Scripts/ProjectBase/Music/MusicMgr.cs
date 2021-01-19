using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 2D音效
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    private AudioSource bkMusic = null;
    private float bkValue = 1;


    private GameObject soundObj = null;
    private List<AudioSource> soundList = new List<AudioSource>();
    private float soundValue=1;

    public MusicMgr()
    {
        MonoMgr.Getinstate().AddUpdateListener(Update);

    }
    private void Update()
    {
        for (int i = soundList.Count-1; i>=0; --i)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBkMusic(string name)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BkMusic";
            bkMusic = obj.AddComponent<AudioSource>();
        }
        //异步加载背景音乐 加载完成后 播放
        ResMgr.Getinstate().LoadAsync<AudioClip>("Music/BK" + name, (Clip) =>
        {
            bkMusic.clip = Clip;
            bkMusic.loop = true;
            bkMusic.volume = bkValue;
            bkMusic.Play();

        });
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBkMusic()
    {
        if (bkMusic == null)
        {
            return;
        }
        bkMusic.Pause();
    }
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBkMusic()
    {
        if (bkMusic == null)
        {
            return;
        }
        bkMusic.Stop();
    }
    /// <summary>
    /// 改变背景音乐大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBKValue(float v)
    {
        bkValue = v;
        if (bkMusic == null)
        {
            return;
        }
        bkMusic.volume = bkValue;

    }
    /// <summary>
    /// 播放音效 回调这个音效，可以在外部主动停止 使用方法，public AudioSource sound (s)=>{sound=s} 形参s代表这个音效，此时对sound操作即可
    /// </summary>
    /// <param name="name"></param>
    public void PlaySound(string name,bool isLoop,UnityAction<AudioSource> callBack=null )
    {
        if (soundObj == null)
        {
            soundObj = new GameObject();
            soundObj.name = "Sound";

        }
       
       //当音效资源异步加载结束后，再添加一个音效
        ResMgr.Getinstate().LoadAsync<AudioClip>("Music/Sound" + name, (Clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = Clip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            soundList.Add(source);
            if (callBack!=null)
            {
                callBack(source);
            }

        });


    }
    /// <summary>
    /// 改变音效大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundValue(float value)
    {
        soundValue = value;
        for (int i = 0; i < soundList.Count; ++i)
        {
            soundList[i].volume = value;

        }
    }
    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
}
