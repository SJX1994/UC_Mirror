using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ResManager;

public class HeroManager : MonoBehaviour
{
    /// <summary>
    /// 英雄全局唯一ID累加
    /// </summary>
    private int HeroIndexId = 90000;

    /// <summary>
    /// 全局英雄管理暂存
    /// </summary>
    public Dictionary<int, HeroInfoClass> HeroControl = new();

    // 通信管理器
    private CommunicationInteractionManager CommunicationManager;

    private void Start()
    {

        // 通信获取
        // 暂时获取方式
        CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();
    }

    /// <summary>
    /// 设置英雄死亡事件
    /// </summary>
    public void SetHeroDie() 
    {
        
    }

    /// <summary>
    /// 查找英雄砖块
    /// </summary>
    /// <param name="tetrisId"></param>
    /// <returns></returns>
    public Dictionary<int, TetrisClass> FindHeroBlocks(int tetrisId, bool killhero = false) 
    {
        Dictionary<int, TetrisClass> returnDic = new();

        foreach (int key in HeroControl.Keys) 
        {
            if (HeroControl[key].HeroInfo.ContainsKey(tetrisId)) 
            {
                returnDic = HeroControl[key].HeroInfo;

                if (killhero)
                {
                    KillHero(HeroControl[key].IndexId);

                    return returnDic;
                }
            }
        }

        return returnDic;
    }

    public void KillHero(int HeroId)
    {
        List<int> commList = new();

        commList.Add(HeroId);

        CommunicationManager.SetUnitDie(commList);

    }

    /// <summary>
    /// 新建英雄
    /// </summary>
    /// <returns></returns>
    public HeroInfoClass CreateHero(Dictionary<int, TetrisClass> _TetrisDic, string heroName) 
    {
        HeroInfoClass heroInfo = new();

        heroInfo.HeroConfigId = this.FindHeroName(heroName);

        heroInfo.HeroName = heroName;

        heroInfo.IndexId = HeroIndexId;

        HeroIndexId++;

        Dictionary<int, TetrisClass> _returnClass = new();

        // 一定要进行处理
        foreach (var h in _TetrisDic.Keys)
        {
            TetrisClass returnTetrisClass = new TetrisClass();

            returnTetrisClass.UnitIndexId = TetrisService.Instance.sumIndex;

            TetrisService.Instance.sumIndex++;

            returnTetrisClass.posx = _TetrisDic[h].posx;

            returnTetrisClass.posy = _TetrisDic[h].posy;

            returnTetrisClass.rotepoint = _TetrisDic[h].rotepoint;

            returnTetrisClass.Color = EventType.UnitColor.yellow;

            returnTetrisClass.BlockLevel = _TetrisDic[h].BlockLevel;

            returnTetrisClass.BlocksGameObject = _TetrisDic[h].BlocksGameObject;

            returnTetrisClass.unitType = _TetrisDic[h].unitType;

            _returnClass.Add(returnTetrisClass.UnitIndexId, returnTetrisClass);
        }

        heroInfo.HeroInfo = _returnClass;

        var infox = _returnClass.First().Value.posx;

        var infoy = _returnClass.First().Value.posy;

        heroInfo.HeroPos =
                    new Vector3(
                        (float)-14.5 + infox * (float)1.5
                            , (float)0.6
                                , (float)-8.5 + infoy * (float)1.5);

        // 将新建的英雄加入全局管理
        HeroControl.Add(heroInfo.IndexId, heroInfo);

        return heroInfo;
    }

    private int FindHeroName(string heroName) 
    {
        var returnInt = 0;

        if (heroName == "爆爆") 
        {
            returnInt = 3001;
        }

        if (heroName == "找找")
        {
            returnInt = 3002;
        }

        if (heroName == "维克")
        {
            returnInt = 3003;
        }

        return returnInt;
    }

    /// <summary>
    /// 生成随机英雄
    /// </summary>
    /// <returns></returns>
    private int RandomHero() 
    {
        int heroId = 1001;

        var random = Random.Range(0, 10);

        //TODO Load from Excel

        //根据概率生成英雄
        if (random >= 0 && random < 5) 
        {
            heroId = 1001;
        }

        if (random >= 5 && random < 10)
        {
            heroId = 1002;
        }

        return heroId;
    }

    public void HeroActionChange() 
    {
        /*var width = FindObjectOfType<TetrisBlocksManager>()._width;

        var height = FindObjectOfType<TetrisBlocksManager>()._height;

        List<HeroActionClass> info = new();

        foreach (int heroIndexId in HeroControl.Keys) 
        {
            HeroActionClass heroAction = new();

            heroAction.HeroIndexId = heroIndexId;

            var randomHeight = Random.Range(0, height);

            heroAction.HeroTargetPos =
                new Vector3(
                    (float)-14.5 + width * (float)1.5
                        , (float)0.6
                            , (float)-8.5 + randomHeight * (float)1.5);

            info.Add(heroAction);
        }

        CommunicationManager.HeroActionChange(info);*/
    }
}
