using System.Collections.Generic;
using UnityEngine;

public class CommunicationInteraction: MonoBehaviour
{
    private Dictionary<int, UnitInfoClass> unitInfoState = new();

    private Dictionary<int, UnitInfoClass> VirusInfoState = new();

    private BroadcastClass broadcastClass;

    private CommunicationInteractionManager CommunicationManager;

    private GameObject sceneLoader;

    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("CommunicationInteraction Start"+ transform.name);
        // TODO ��ʱ��ȡ��ʽ
        sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;

        // ȫ��ͨ�ŷ�������
        CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();

        // ȫ��ͨ���¼�ע����
        broadcastClass = sceneLoader.GetComponent<BroadcastClass>();

        // ����ש����Ϣ����
        broadcastClass.TetrisInfoCreate += OnListenTetrisCreate;

        // ����ש����Ϣ����
        broadcastClass.TetrisInfoUpdate += OnListenTetrisUpdate;

        // ����½�����
        broadcastClass.VirusInfoCreate += OnlistenVirusCreate;

        // �����Ϣ���¼���
        broadcastClass.VirusInfoUpdate += OnlistenVirusUpdate;

    }

    void OnlistenVirusCreate(List<UnitInfoClass> infoList)
    {
        foreach (UnitInfoClass info in infoList)
        {
            if (!VirusInfoState.ContainsKey(info.UnitIndexId) && info.CreateUnit)
            {
                VirusInfoState.Add(info.UnitIndexId, info);

                Debug.Log("�����Ϣ���ճɹ� " + "UnitIndexId:" + info.UnitIndexId + "," + "NewTetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
            }
            else
            {
                Debug.LogError("Unit has been create, please check Tetris logic");
            }
        }
    }

    void OnlistenVirusUpdate(List<UnitInfoClass> infoList)
    {
        foreach (UnitInfoClass info in infoList)
        {
            if (VirusInfoState.TryGetValue(info.UnitIndexId, out UnitInfoClass value))
            {
                VirusInfoState[info.UnitIndexId] = info;

                Debug.Log("�����Ϣ���³ɹ� " + "UnitIndexId:" + info.UnitIndexId + "," + "NewTetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
            }
            else
            {
                Debug.LogError("Unit has not been created yet, please check Tetris logic");
            }
        }

    }

    /// <summary>
    /// ש�����ɼ���
    /// </summary>
    /// <param name="info"></param>
    void OnListenTetrisCreate(List<UnitInfoClass> infoList) 
    {
        foreach (UnitInfoClass info in infoList) 
        {
            if (!unitInfoState.ContainsKey(info.UnitIndexId) && info.CreateUnit)
            {
                unitInfoState.Add(info.UnitIndexId, info);

                Debug.Log("������Ϣ���ճɹ� " + "UnitIndexId:" + info.UnitIndexId + "," + "NewTetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
            }
            else 
            {
                Debug.LogError("Unit has been create, please check Tetris logic");
            }
        }
    }

    /// <summary>
    /// ש����Ϣ���¼���
    /// </summary>
    /// <param name="info"></param>
    void OnListenTetrisUpdate(List<UnitInfoClass> infoList)
    {
        foreach (UnitInfoClass info in infoList)
        {
            if (unitInfoState.TryGetValue(info.UnitIndexId, out UnitInfoClass value))
            {
                unitInfoState[info.UnitIndexId] = info;

                Debug.Log("������Ϣ���³ɹ� " + "UnitIndexId:" + info.UnitIndexId + "," + "NewTetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
            }
            else
            {
                Debug.LogError("Unit has not been created yet, please check Tetris logic");
            }
        }
    }

    /// <summary>
    /// Unit����ʱ������ͨ��
    /// </summary>
    /// <param name="UnitIndexId"></param>
    void OnUnitDie(int UnitIndexId) 
    {
        CommunicationManager.UnitDieInfoProcess(UnitIndexId);
    }
}
