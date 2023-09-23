using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class CommunicatExample : MonoBehaviour
{
    public Camera CameraStory;
    private Camera CameraMain;
    public List<PlayableDirector> playableDirectors;
    private PlayableDirector m_activeDirector;

    // 通讯对象
    private GameObject sceneLoader;
    private CommunicationInteractionManager CommunicationManager;
    private BroadcastClass broadcastClass;

    // TODO delete
    // private List<GameObject> UIs = new();

    void Start()
    {
        CameraStory.gameObject.SetActive(false);
        CameraMain = (Camera) FindObjectOfType(typeof(Camera));
        if(CameraMain)
        {
            CameraStory.gameObject.SetActive(false);
        }else
        {
            CameraStory.gameObject.SetActive(true);
        }
        
        foreach (var director in playableDirectors)
        {
            director.gameObject.SetActive(false);
        }


        // 暂时获取方式
        sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;

        // 全局通信方法管理
        CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();

        // 全局通信事件注册类
        broadcastClass = sceneLoader.GetComponent<BroadcastClass>();

        broadcastClass.OnStoryPlay += StoryFire;
    }

    public void PlayStory(int info) 
    {
        CommunicationManager.OnStoryPlay(info);
    }

    public void StoryFire(int storyIndex)
    {
        
        // UIs.Add(GameObject.Find("LegionUi(Clone)")) ;
        // UIs.Add(GameObject.Find("CanvasManager_StayMachine(Clone)")) ;
        // if(UIs[0] != null)
        // {
        //     foreach (var ui in UIs)
        //     {
        //         ui.SetActive(false);
        //     }
        // }
        

        Time.timeScale = 0;

        CameraMain = (Camera) FindObjectOfType(typeof(Camera));
        if(CameraMain != CameraStory)
        {
            CameraMain.gameObject.SetActive(false);
            CameraStory.gameObject.SetActive(true);
        }else
        {
            CameraStory = CameraMain;
        }
        
        m_activeDirector = playableDirectors[storyIndex];
        m_activeDirector.gameObject.SetActive(true);
        m_activeDirector.stopped += StoryFinished;
    }

    private void StoryFinished(PlayableDirector aDirector)
    {
        // if (UIs[0] != null) 
        // {
        //     foreach (var ui in UIs)
        //     {
        //         ui.SetActive(true);
        //     }
        // }

        Debug.Log("故事：" + m_activeDirector.gameObject.name+ "播放完成");
        if(CameraMain != CameraStory)
        {  
            CameraMain.gameObject.SetActive(true);
            CameraStory.gameObject.SetActive(false);
        }else
        {
            CameraMain.gameObject.SetActive(true);
        }
        
        m_activeDirector.gameObject.SetActive(false);

        CommunicationManager.OnStoryEnd(1);
    }
}
