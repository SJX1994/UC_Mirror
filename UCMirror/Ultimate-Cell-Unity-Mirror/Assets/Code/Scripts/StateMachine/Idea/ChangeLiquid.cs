using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Mirror;
using UC_PlayerData;
public class ChangeLiquid : NetworkBehaviour
{
    /// <summary>
    /// 液体
    /// </summary>
    public GameObject buttom;
    /// <summary>
    /// 液面
    /// </summary>
    public GameObject top;
    /// <summary>
    /// 完成指示灯
    /// </summary>
    public GameObject ready;
    private IdelBox ideaBox;
    float fade;
    Vector3 originPositionTop;
   
    void Start()
    {

        buttom = transform.Find("liquidSet").Find("liquid_bottom").gameObject;
        top = transform.Find("liquidSet").Find("liquid_top").gameObject;
        ready = transform.Find("IdelReady").gameObject;

        ready.SetActive(true);
        originPositionTop = top.GetComponent<RectTransform>().anchoredPosition;
        ideaBox = transform.GetComponent<IdelBox>();
        top.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        buttom.GetComponent<Image>().fillAmount = 1;
        
    }
   
        
    public void DoCount()
    {
        top.GetComponent<RectTransform>().anchoredPosition = originPositionTop;
        ready.SetActive(false);
        ideaBox.Idel.SetActive(false);
        top.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f,0f,0f),6f);
        DOVirtual.Float(0, 1, 6f, (float value) =>
        {
            fade = value;
            buttom.GetComponent<Image>().fillAmount = value;
        }).onComplete=() =>
        {
             ready.SetActive(true);
             ideaBox.Idel.SetActive(true);
             ideaBox.OnGameObjCreate();
        };
        
    }
    [Client]
    public void Client_DoCount()
    {
        top.GetComponent<RectTransform>().anchoredPosition = originPositionTop;
        ready.SetActive(false);
        ideaBox.Idel.SetActive(false);
        top.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f,0f,0f),6f);
        DOVirtual.Float(0, 1, 6.3f, (float value) =>
        {
            fade = value;
            buttom.GetComponent<Image>().fillAmount = value;
            foreach(var tetris in FindObjectsOfType<TetrisBlockSimple>())
            {
                if(tetris.player != Player.NotReady)return;
                // 是否需要在 客户端idelBox 还没获取的时候 隐藏 俄罗斯方块组
            }
        }).onComplete=() =>
        {
             ready.SetActive(true);
             ideaBox.Idel.SetActive(true);
             ideaBox.ClientGetTetrisGroupID();
        };
        
    }
    public void ChangeColor(Color color)
    {
        if(!ideaBox.idelUI.hiden)
        {
            buttom.GetComponent<Image>().DOColor(color,0.5f);
            top.GetComponent<Image>().DOColor(color,0.5f);
        }else
        {
            Color colorHiden = new(color.r,color.g,color.b,Dispaly.HidenAlpha);
            buttom.GetComponent<Image>().DOColor(colorHiden,0.5f);
            top.GetComponent<Image>().DOColor(colorHiden,0.5f);
        }
        
    }
    


  
}
