using System.Collections.Generic;
using UnityEngine;

public class PuppetLineManager : MonoBehaviour
{
    private Dictionary<int, PuppetUnit> puppets = new();

    public PuppetUnit cellPuppetUnit;

    public PuppetUnit VirusPuppetUnit;

    // 通信类
    private BroadcastClass broadcastClass;

    // 通信管理器
    private CommunicationInteractionManager CommunicationManager;

    // 木偶固定信息暂存队列
    private Dictionary<int, Queue<PuppetFixedInfoClass>> puppetDicQueue = new();

    private void Start()
    {
        // 通信获取
        // 暂时获取方式
        CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

        broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        broadcastClass.CreatePuppet += CreatePuppet;

        broadcastClass.UpdatePuppetFixedInfo += PuppetUnitFixedInfoUpdate;

        broadcastClass.PuppetUnitPosUpdateClass += PuppetUnitInfoUpdate;

        cellPuppetUnit.ClientStart = true;

        cellPuppetUnit.pupperManager = this;

        VirusPuppetUnit.ClientStart = true;

        VirusPuppetUnit.pupperManager = this;

    }

    /// <summary>
    /// 新建木偶
    /// </summary>
    /// <param name="info"></param>
    void CreatePuppet(int info)
    {
        var puppet = info > 0 ? Instantiate(cellPuppetUnit.gameObject) : Instantiate(VirusPuppetUnit.gameObject);

        var puppetUnitSC = puppet.GetComponent<PuppetUnit>();

        puppetUnitSC.puppetId = info;
    }

    /// <summary>
    /// 木偶新建完成
    /// </summary>
    public void PuppetCreateDone(int info, GameObject puppet)
    {
        if (!puppets.ContainsKey(info))
        {
            puppets.Add(info, puppet.GetComponent<PuppetUnit>());

            if (puppetDicQueue.ContainsKey(info)) 
            {
                var queue = puppetDicQueue[info].Count;

                for (int i = 0; i < queue; i++) 
                {
                    var updateMsg = puppetDicQueue[info].Dequeue();

                    this.PuppetUnitFixedInfoUpdate(updateMsg);

                    Debug.Log("QueueCount" + queue);
                }
            }
        }
    }

    /// <summary>
    /// 木偶固定信息更新
    /// </summary>
    /// <param name="info"></param>
    void PuppetUnitFixedInfoUpdate(PuppetFixedInfoClass info) 
    {
        var id = info.UnitId;

        if (puppets.ContainsKey(id))
        {
            var puppet = puppets[id];

            if (info.OnPuppetAttacking)
            {
                puppet.OnAttacking();
            }

            if (info.OnPuppetAttackFinish)
            {
                puppet.OnAttackFinish();
            }

            if (info.OnPuppetShooting)
            {
                puppet.OnShooting(info.isPuppetShooting, info.PuppetShootingPos);
            }

            if (info.OnPuppetSide)
            {
                puppet.OnSide(info.PuppetSide);
            }

            if (info.OnPuppetStateChanged)
            {
                puppet.OnStateChanged(info.PuppetState1, info.PuppetState2);
            }

            if (info.OnPuppetTypeChanged)
            {
                puppet.puppetUnitType = info.PuppetTypeChanged;
            }

            if (info.OnPuppetSkinChanged)
            {
                puppet.OnChangeSkin(info.PuppetSkinChanged);
            }

            if (info.OnPuppetDestory)
            {
                puppet.OnDie(info.PuppetDestory);
            }

            if (info.OnPuppetChangeWeapon)
            {
                puppet.OnChangeWeapon(info.PuppetChangeWeapon);
            }
        }
        else 
        {
            if (!puppetDicQueue.ContainsKey(id)) 
            {
                Queue<PuppetFixedInfoClass> puppetqueue = new();

                puppetDicQueue.Add(id, puppetqueue);
            }

            puppetDicQueue[id].Enqueue(info);
        }
    }

    /// <summary>
    /// 更新木偶位置信息
    /// </summary>
    /// <param name="info"></param>
    void PuppetUnitInfoUpdate(UnitInfoUpdateStruct info)
    {
        var id = info.UnitId;

        if (puppets.ContainsKey(id))
        {
            var puppet = puppets[id];

            if (info.PosUpdate)
            {
                puppet.OnUnitPositionChanged(info.PuppetPosition);
            }

            if (info.ScaleUpdate)
            {
                puppet.OnUnitScaleChanged(info.PuppetScale);
            }

            if (info.HealthUpdate)
            {
                puppet.UpdateMatHealth(info.PuppetHealth);
            }

            if (info.FilpUpdate)
            {
                puppet.OnFilp(info.PuppetFilp);
            }

            if (info.SpeedUpdate)
            {
                puppet.OnSpeedChanged(info.PuppetSpeed);
            }

            if (info.AttackPosUpdate)
            {
                puppet.OnTargetOfAttackChange(info.PuppetAttackPosition);
            }
        }
    }
}