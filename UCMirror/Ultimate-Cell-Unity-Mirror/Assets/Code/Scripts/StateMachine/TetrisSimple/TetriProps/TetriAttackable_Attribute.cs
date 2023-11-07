using UnityEngine;
using UC_PlayerData;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
public class TetriAttackable_Attribute : NetworkBehaviour
{
#region 数据对象
    public State state = State.Normal;
    public enum State
    {
        Normal,
        Dead,
    }
    [SerializeField]
    public int maxHealth = 300;

    int currentHP = 300;
    float shaderHP = 1;
    [SerializeField]
    public BeenAttackedDisplay beenAttackedDisplay = BeenAttackedDisplay.NotReady;
    public enum BeenAttackedDisplay
    {
        NotReady,
        Obstacle,
    }
    ParticleSystem beenAttackedParticleSystem;
    ParticleSystem BeenAttackedParticleSystem
    {
        get
        {
            if(!beenAttackedParticleSystem)
            {
                string pathName = "Prefabs/DefaultBeenAttacked";
                switch(beenAttackedDisplay)
                {
                    case BeenAttackedDisplay.Obstacle:
                        pathName = "Prefabs/ObstacleBeenAttacked";
                        break;
                }
                beenAttackedParticleSystem = Resources.Load<ParticleSystem>(pathName);
            }
            return beenAttackedParticleSystem;
        }
    }
    private GameObject icon;
    public GameObject Icon
    {
        get
        {
            if(!icon)icon = transform.GetChild(0).Find("Display").gameObject;
            return icon;
        }
    }
    private MaterialPropertyBlock spinePropertyBlock_hp;
    public Tween tween_BeenAttack;
    BlocksProps blocksProps;
    public BlocksProps BlocksProps
    {
        get
        {
            if(!blocksProps)blocksProps = FindObjectOfType<BlocksProps>();
            return blocksProps;
        }
    }
    public float maybeHaveTheTetriBallProbability = 0.5f;
#endregion 数据对象
#region 数据关系
    void Start()
    {
        currentHP = maxHealth;
        UpdateMatHealth(1);
    }
#endregion 数据关系
#region 数据操作
    public void SufferAttackSimple(int damage,UnitAttackProp whoAttacking)
    {
        OnBeenAttackedDisplay();
        currentHP -= damage;
        shaderHP = UC_Tool.Remap((float)currentHP, 0, (float)maxHealth, 0, 1);
        UpdateMatHealth(shaderHP);
        if(currentHP <= 0)
        {
            state = State.Dead;
            OnDead(whoAttacking);
        }

    }
    void OnBeenAttackedDisplay()
    {
        if(Local())
        {
            tween_BeenAttack?.Kill();
            Color startColor = Color.white;
            Color endColor = Color.gray;
            Icon.GetComponent<SpriteRenderer>().color = startColor;
            tween_BeenAttack = DOVirtual.Color(endColor,startColor, 0.3f, (TweenCallback<Color>)((Color value) =>
            {
                Icon.GetComponent<SpriteRenderer>().color = value;
            }));
        }else
        {
            Server_OnBeenAttackedDisplay();
        }
        
        
    }

    public void UpdateMatHealth(float SetFloat)
    {
        if(Local())
        {
            if (spinePropertyBlock_hp == null)
            {
                spinePropertyBlock_hp = new MaterialPropertyBlock();
            }
            if(SetFloat == spinePropertyBlock_hp.GetFloat("_Porcess"))return;
            Icon.GetComponent<Renderer>().GetPropertyBlock(spinePropertyBlock_hp);
            spinePropertyBlock_hp.SetFloat("_Porcess", SetFloat);
            Icon.GetComponent<Renderer>().SetPropertyBlock(spinePropertyBlock_hp);
        }else
        {
            if(!isServer)return;
            Server_UpdateMatHealth(SetFloat);
        }
        
    }

    void OnDead(UnitAttackProp whoAttacking)
    {
        whoAttacking.UnitSimple.Display_AllWin(whoAttacking.UnitSimple.TetriUnitSimple.TetriBlock);
        if(TryGetComponent(out TetriObstacle tetriObstacle))
        {
            float randomValue = Random.value;
            if (randomValue < maybeHaveTheTetriBallProbability)
            {
                BlocksProps.GenerateChainBall_SpecificCoordinates(tetriObstacle.posId);
            }
            tetriObstacle.Collect();
        }
    }
#endregion 数据操作
#region 联网数据操作
    bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    [Server]
    void Server_UpdateMatHealth(float SetFloat)
    {
        if (spinePropertyBlock_hp == null)
        {
            spinePropertyBlock_hp = new MaterialPropertyBlock();
        }
        if(SetFloat == spinePropertyBlock_hp.GetFloat("_Porcess"))return;
        Icon.GetComponent<Renderer>().GetPropertyBlock(spinePropertyBlock_hp);
        spinePropertyBlock_hp.SetFloat("_Porcess", SetFloat);
        Icon.GetComponent<Renderer>().SetPropertyBlock(spinePropertyBlock_hp);
        Client_UpdateMatHealth(SetFloat);
    }
    [ClientRpc]
    void Client_UpdateMatHealth(float SetFloat)
    {
        if (spinePropertyBlock_hp == null)
        {
            spinePropertyBlock_hp = new MaterialPropertyBlock();
        }
        if(SetFloat == spinePropertyBlock_hp.GetFloat("_Porcess"))return;
        Icon.GetComponent<Renderer>().GetPropertyBlock(spinePropertyBlock_hp);
        spinePropertyBlock_hp.SetFloat("_Porcess", SetFloat);
        Icon.GetComponent<Renderer>().SetPropertyBlock(spinePropertyBlock_hp);
    }
    [Server]
    void Server_OnBeenAttackedDisplay()
    {
        tween_BeenAttack?.Kill();
        Color startColor = Color.white;
        Color endColor = Color.gray;
        Icon.GetComponent<SpriteRenderer>().color = startColor;
        tween_BeenAttack = DOVirtual.Color(endColor,startColor, 0.3f, (TweenCallback<Color>)((Color value) =>
        {
            Icon.GetComponent<SpriteRenderer>().color = value;
        }));
        Client_OnBeenAttackedDisplay();
    }
    [ClientRpc]
    void Client_OnBeenAttackedDisplay()
    {
        tween_BeenAttack?.Kill();
        Color startColor = Color.white;
        Color endColor = Color.gray;
        Icon.GetComponent<SpriteRenderer>().color = startColor;
        tween_BeenAttack = DOVirtual.Color(endColor,startColor, 0.3f, (TweenCallback<Color>)((Color value) =>
        {
            Icon.GetComponent<SpriteRenderer>().color = value;
        }));
    }
#endregion 联网数据操作
}