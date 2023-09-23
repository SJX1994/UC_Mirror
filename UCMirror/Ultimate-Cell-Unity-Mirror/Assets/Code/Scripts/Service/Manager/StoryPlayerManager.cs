using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class StoryPlayerManager : MonoBehaviour
{
    // 通讯对象
    private GameObject sceneLoader;
    private CommunicationInteractionManager CommunicationManager;
    private BroadcastClass broadcastClass;

    private void Start()
    {
        // 暂时获取方式
        sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;

        // 全局通信方法管理
        CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();

        // 全局通信事件注册类
        broadcastClass = sceneLoader.GetComponent<BroadcastClass>();

        broadcastClass.OnStoryEnd += StoryEnd;

        CommunicationManager.OnStoryLoadingComplete(1);
    }

    /// <summary>
    /// 动画播放入口
    /// </summary>
    /// <param name="StoryId"></param>
    public void PLayStory(int StoryId) 
    {
        // 全局暂停
        FindObjectOfType<GlobalControlManager>().OnPause();

        // 动画播放触发
        CommunicationManager.OnStoryPlay(StoryId);

    }

    /// <summary>
    /// 动画播放结束触发事件
    /// </summary>
    /// <param name="EndType"></param>
    private void StoryEnd(int EndType)
    {
        if (EndType == 1)
        {
            // 继续游戏
            FindObjectOfType<GlobalControlManager>().OnContinues();
        }
    }

}
