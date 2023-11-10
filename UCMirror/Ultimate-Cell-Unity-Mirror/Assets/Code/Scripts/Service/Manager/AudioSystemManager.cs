
using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioSystemManager : MonoBehaviour
{
    // 单例
    static AudioSystemManager audioSys;
    // 音乐播放
    static AudioSource musicSource;
    // 音效播放
    static AudioSource soundSource;
    // 当前音乐名
    static string nowName = null;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // 单例
    public static AudioSystemManager Instance
    {
        get
        {
            if (audioSys == null)
            {
                audioSys = new GameObject("AudioSystem").AddComponent<AudioSystemManager>();
                audioSys.gameObject.AddComponent<AudioListener>();
                audioSys.Init();
            }
            return audioSys;
        }
    }

    // 创建两个子节点
    void Init()
    {
        musicSource = new GameObject("musicSource").AddComponent<AudioSource>();
        musicSource.transform.SetParent(audioSys.transform);
        musicSource.playOnAwake = false;
        musicSource.loop = false;
        soundSource = new GameObject("soundSource").AddComponent<AudioSource>();
        soundSource.transform.SetParent(audioSys.transform);
        soundSource.playOnAwake = false;
        soundSource.loop = false;
    }

    //播放列表音乐
    public void PlayMusicList(List<KeyValuePair<string, int>> list, int index = 0)
    {
        if (list == null || index >= list.Count) return;
        PlayMusic(list[index].Key, list[index].Value, () =>
        {
            PlayMusicList(list, index + 1);
        });
    }

    static Coroutine coroutine;
    // 播放完成执行acion
    public void PlayMusic(string name, int loopTime, Action action = null)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        if (nowName == name)
        {
            //正在播放当前资源
            if (!musicSource.isPlaying)
                musicSource.Play();
        }
        else
        {
            //加载资源
            // AudioClip clip = ABManager.Instance.LoadResource<AudioClip>("audio", name);
            AudioClip clip = Resources.Load<AudioClip>(name);
                // Resources.Load<AudioClip>(PATH + fname + ".wav");
            if (clip == null)
                return;
            if (musicSource.isPlaying)
                musicSource.Stop();
            musicSource.clip = clip;
            musicSource.volume *= 0.5f;
            nowName = name;
            musicSource.Play();

            musicSource.loop = loopTime > 0;
        }
        // 开启协程,并将循环次数 - 1
        coroutine = StartCoroutine(MusicLoop(name, loopTime - 1, action));
    }
    public void PlayMusicSimple(string path,float volume)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null)return;
        if(musicSource.isPlaying)return;
        musicSource.clip = clip;
        musicSource.volume *= volume;
        musicSource.Play();
        musicSource.loop = true;

    }
    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(string name)
    {
        // AudioClip clip = ABManager.Instance.LoadResource<AudioClip>("audio", name);
        AudioClip clip = Resources.Load<AudioClip>(name);
        soundSource.PlayOneShot(clip);
    }
    public void PlaySoundSimple(string name,float volume = 1,float delay = 0)
    {
        // AudioClip clip = ABManager.Instance.LoadResource<AudioClip>("audio", name);
        AudioClip clip = Resources.Load<AudioClip>(name);
        soundSource.volume *= volume;
        if(delay == 0)
        {
            soundSource.PlayOneShot(clip);
        }else
        {
            soundSource.clip = clip;
            soundSource.PlayDelayed(delay);
        }
    }
    public void PlaySoundSimpleScaleTemp(string name ,float seconds,float volume = 1)
    {
        AudioClip clip = Resources.Load<AudioClip>(name);
        AudioSource soundSourceTemp = new GameObject("soundSourceTempScaled").AddComponent<AudioSource>();
        float destoryTime = clip.length;
        soundSourceTemp.clip = clip;
        float currentDuration = soundSourceTemp.clip.length;
        float scaleFactorTemp = currentDuration/seconds;
        soundSourceTemp.pitch = scaleFactorTemp;
        soundSourceTemp.time = 0f;
        soundSourceTemp.volume *= volume;
        soundSourceTemp.Play();

        Destroy(soundSourceTemp.gameObject,seconds + 1f);
    }
    public void PlaySoundSimpleTemp(string name,float volume = 1,float delay = 0)
    {
        AudioClip clip = Resources.Load<AudioClip>(name);
        AudioSource soundSourceTemp = new GameObject("soundSourceTemp").AddComponent<AudioSource>();
        float destoryTime = clip.length;
        soundSourceTemp.transform.SetParent(audioSys.transform);
        soundSourceTemp.playOnAwake = false;
        soundSourceTemp.loop = false;
        soundSourceTemp.clip = clip;
        soundSourceTemp.volume *= volume;
        if(delay == 0)
        {
            if(clip.length > 3)
            {
                destoryTime = 3;
                float randomStartTime = UnityEngine.Random.Range(0, clip.length-3);
                soundSourceTemp.time = randomStartTime;
                soundSourceTemp.Play();
            }else
            {
                soundSourceTemp.PlayOneShot(clip);
            }
            
        }else
        {
            if(clip.length > 3)
            {
                destoryTime = 3;
                float randomStartTime = UnityEngine.Random.Range(0, clip.length-3);
                soundSourceTemp.time = randomStartTime;
                soundSourceTemp.PlayDelayed(delay);
            }else
            {
                soundSourceTemp.PlayDelayed(delay);
            }
        }
       
        Destroy(soundSourceTemp.gameObject,destoryTime + delay + 1f);
    }

    IEnumerator MusicLoop(string path, int loopTime, Action action = null)
    {
        //等待播放完毕
        while (musicSource.isPlaying) yield return null;
        //播放次数不为0就继续循环
        if (loopTime != 0)
            PlayMusic(path, loopTime, action);
        //播放完毕且有回调则执行
        else if (action != null)
            action();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp(volume, 0, 1);
    }

    public void PlayOneShot(string name)
    {

    }

    private void OnDestroy()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}
