using UnityEngine;

public class LocalPlayerRoomControlManager : MonoBehaviour 
{
    [Header("本地模拟房间")]
    public GameObject LocalPlayerRoom;

    [Header("本地模拟通讯")]
    public GameObject NetMessage;

    // 通信类
    private BroadcastClass broadcastClass;

    // 通信管理器
    private CommunicationInteractionManager CommunicationManager;

    private void Start()
    {
        // 通信获取
        // 暂时获取方式
        CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

        broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        if (CommunicationManager.ServerState == 2) 
        {
            LocalPlayerRoom.SetActive(true);
        }
    }
}