

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using com.cygnusprojects.TalentTree;
using System.Linq;
using Unity.VisualScripting;

public class TreeGrowingDispaly : MonoBehaviour
{
# region 数据对象
    public UnityEngine.UI.Image DebugImage;
    public List<TalentUILineRenderer> conns = new();
    private List<Tween> tweens0 = new List<Tween>();
    private List<Tween> tweens1 = new List<Tween>();
    public List<TalentUI> fruits = new();
# endregion 数据对象
# region 数据关系
    // Start is called before the first frame update
    void Start()
    {
        if(fruits.Count>0)
        {
            foreach(var fruit in fruits)
            {
                fruit.OnBuy += OnBuyDisplay;
                fruit.OnRevert += OnRevertDisplay;
                
            }
        }
        
        // 方法0：
        // foreach(var conn in conns)
        // {
            
        //     conn.GetAnimation();
        //     for (int i = 0; i < conn.Points.Length; i++)
        //     {
        //         if(i%2==0)
        //         {
        //             tweens0.Add(conn.DoAnimation(i));
        //         }else
        //         {
        //             tweens1.Add(conn.DoAnimation(i));
        //         }
                
        //     }
           
        // }
        // 方法1：
        // for(int i = 0; i < conns.Count; i++)
        // {
        //     if(i%2==0)
        //     {
        //         conns[i].GetAnimation();
        //         for (int j = 0; j < conns[i].Points.Length; j++)
        //         {
                    
        //             tweens0.Add(conns[i].DoAnimation(j));
        //         }
        //     }else
        //     {
        //         conns[i].GetAnimation();
        //         for (int j = 0; j < conns[i].Points.Length; j++)
        //         {
                    
        //             tweens1.Add(conns[i].DoAnimation(j));
        //         }
        //     }
        // }
        // PlayNextTween0();
        // PlayNextTween1();

        
        foreach(var conn in conns)
        {
            conn.gameObject.SetActive(true);
            conn.GetAnimation();
        }
        Invoke(nameof(LateStart), 0.1f);
    }
    void LateStart()
    {
        foreach(var fruit in fruits)
        {
            UnityEngine.UI.Text textUI_Buy = fruit.buyButton.transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
             UnityEngine.UI.Text textUI_Sell = fruit.revertButton.transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
             textUI_Sell.text = "遗忘.";
            if(fruit.Talent.GetCostForNextLevel().Cost == 0)
            {
                textUI_Buy.text = "回忆";
            }else
            {
                textUI_Buy.text = "注入";
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
# endregion 数据关系
# region 数据操作
    void OnBuyDisplay(TalentUI talentUI)
    {
        UnityEngine.UI.Text textUI = talentUI.buyButton.transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
        if(talentUI.Talent.IsMaxedOut)
        {
            textUI.text = "回忆";
        }else
        {
            textUI.text = "注入";
        }
        foreach(var fruit in fruits)
        {
            if(fruit != talentUI)
            {
                fruit.revertButton.gameObject.SetActive(false);
            }
            if(fruit.Talent.GetCostForNextLevel().Cost == 0)
            {
                List<TalentTreeConnectionBase>conns = fruit.toConns;
                foreach(var conn in conns)
                {
                    TalentUI talentUItemp = fruits.Find(fruit => fruit.Talent == conn.toNode);
                    if(talentUItemp)
                    {
                        talentUItemp.buyButton.gameObject.SetActive(true);
                    }
                }
            }
        }
        
        List<TalentTreeConnectionBase> connsToTemp = new();
        if(talentUI.toConns.Count>0)
        {
            connsToTemp = talentUI.toConns;
            // Debug.Log("bug" + connsToTemp[0].name);
            foreach(var conn in conns)
            {
                if(connsToTemp.Contains(conn.Connection))
                {
                    
                    for (int i = 0; i < conn.Points.Length; i++)
                    {
                        
                        Tween tween = conn.DoAnimation(i);
                        tween.Play();
                        
                    }
                }
            }
            // 只显示下一层的BUY
            foreach(var conn in connsToTemp)
            {
                TalentUI talentUItemp = fruits.Find(fruit => fruit.Talent == conn.toNode);
                if(talentUItemp)
                {
                    talentUItemp.buyButton.gameObject.SetActive(true);
                }
            }
        }
        

    }
    void OnRevertDisplay(TalentUI talentUI)
    {
        // 上一级回退按钮点亮
            List<TalentTreeConnectionBase> connsFromTemp = new();
            if(talentUI.fromConns.Count>0)
            {
                connsFromTemp = talentUI.fromConns;
                List<TalentTreeNodeBase> nodesFromTemp = new();
                foreach(var connFromTemp in connsFromTemp)
                {
                    nodesFromTemp.Add(connFromTemp.fromNode);
                }
                foreach(var fruit in fruits)
                {
                    if(nodesFromTemp.Contains(fruit.Talent))
                    {
                        fruit.revertButton.gameObject.SetActive(true);
                    }
                }
            }
        // 回退动画
           RepeatRevert(talentUI);
    }
    private void RepeatRevert(TalentUI talentUI)
    {
        List<TalentTreeConnectionBase> connsToTemp = new();
        if(talentUI.toConns.Count>0)
        {
            connsToTemp = talentUI.toConns;
            // Debug.Log("bug" + connsToTemp[0].name);
            foreach(var conn in conns)
            {
                
                if(connsToTemp.Contains(conn.Connection))
                {
                    
                    for (int i = 0; i < conn.Points.Length; i++)
                    {
                        
                        Tween tween = conn.UnDoAnimation(i);
                        tween.Play();
                        
                    }
                }
            }
            foreach (var conn in connsToTemp)
            {
                if(conn.toNode != null)
                {
                    TalentUI talentUItemp = fruits.Find(fruit => fruit.Talent == conn.toNode);
                    if(talentUItemp)
                    {
                        talentUItemp.revertButton.gameObject.SetActive(false);
                        talentUItemp.buyButton.gameObject.SetActive(false);

                        if(talentUItemp!=talentUI)
                        {
                            talentUItemp.Engine.RevertTalent(talentUItemp.Talent);
                            talentUItemp.OnRevert?.Invoke(talentUItemp);
                            RepeatRevert(talentUItemp);
                        }
                        
                    }
                }
            }
        }
    }
    private void PlayNextTween1()
    {
        
        // 如果没有剩余的Tween，则退出
        if (tweens1.Count <= 0)
        {
            Debug.Log("Tween 所有完成");
            return;
        }

        // 获取下一个Tween
        Tween nextTween = tweens1[0];
        tweens1.RemoveAt(0);
        // 开始播放Tween
        nextTween.Restart();

        // 在Tween的OnComplete回调中触发播放下一个Tween
        nextTween.OnComplete(() => PlayNextTween1());

        
    }
    private void PlayNextTween0()
    {
        
        // 如果没有剩余的Tween，则退出
        if (tweens0.Count <= 0)
        {
            Debug.Log("Tween 所有完成");
            return;
        }

        // 获取下一个Tween
        Tween nextTween = tweens0[0];
        tweens0.RemoveAt(0);
        // 开始播放Tween
        nextTween.Restart();

        // 在Tween的OnComplete回调中触发播放下一个Tween
        nextTween.OnComplete(() => PlayNextTween0());

        
    }
    
# endregion 数据操作
}
