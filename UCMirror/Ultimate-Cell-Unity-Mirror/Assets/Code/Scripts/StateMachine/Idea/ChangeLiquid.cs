using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Mirror;
using UC_PlayerData;
using UnityEngine.Events;
public class ChangeLiquid : NetworkBehaviour
{
    float inIdelbox_UpLevelCountdown = Referee.InIdelbox_UpLevelCountdown;
    float inIdelbox_CreatCountdown = Referee.InIdelbox_CreatCountdown;
    float upLevelBlinkAnimationStrengthIncrement = 1f;
    public GameObject columnLiquid;
    public GameObject ColumnLiquid
    {
        get
        {
            if(columnLiquid)return columnLiquid;
            columnLiquid = transform.Find("liquidSet").Find("liquid_Mask").Find("liquid_bottom").gameObject;
            return columnLiquid;
        }
    }
    public GameObject topLiquid;
    public GameObject TopLiquid
    {
        get
        {
            if(topLiquid)return topLiquid;
            topLiquid = transform.Find("liquidSet").Find("liquid_Mask").Find("liquid_top").gameObject;
            return topLiquid;
        }
    }
    public GameObject readyLight;
    public GameObject ReadyLight
    {
        get
        {
            if(readyLight)return readyLight;
            readyLight = transform.Find("IdelReady").gameObject;
            return readyLight;
        }
    }
    public GameObject Halo
    {
        get
        {
            if(ReadyLight.transform.childCount > 0)
            {
                return ReadyLight.transform.GetChild(0).gameObject;
            }
            return null;
        }
    }
    private IdelBox ideaBox;
    float fade;
    int level = 0;
    int Level
    {
        get
        {
            if(level == 0)level = 1;
            return level;
        }
        set
        {
            level = value;
            OnLevelUp?.Invoke(level);
        }
    }
    public UnityAction<int> OnLevelUp;
    Vector3 originPositionTop;
    Sequence blinkSequence;
    float blinkDuration = 3f;
    float resetStartBlinkIntensity;
    float startBlinkIntensity = 0.25f;
    float StartBlinkIntensity
    {
        get
        {
            float maxStartBlinkIntensity = 0.62f;
            if(startBlinkIntensity > maxStartBlinkIntensity)startBlinkIntensity = maxStartBlinkIntensity;
            return startBlinkIntensity;
        }
    }
    float resetEndBlinkIntensity;
    float endBlinkIntensity = 0.3f;
    float EndBlinkIntensity
    {
        get
        {
            float maxEndBlinkIntensity = 1f;
            if(endBlinkIntensity > maxEndBlinkIntensity)endBlinkIntensity = maxEndBlinkIntensity;
            return endBlinkIntensity;
        }
    }
    float haloblinkDuration = 3.2f;
    float resetHaloStartBlinkIntensity;
    float haloStartBlinkIntensity = 0.0f;
    float HaloStartBlinkIntensity
    {
        get
        {
            float maxHaloStartBlinkIntensity = 0.32f;
            if(haloStartBlinkIntensity > maxHaloStartBlinkIntensity)haloStartBlinkIntensity = maxHaloStartBlinkIntensity;
            return haloStartBlinkIntensity;
        }
    }
    float resetHaloEndBlinkIntensity;
    float haloEndBlinkIntensity = 0.32f;
    float HaloEndBlinkIntensity
    {
        get
        {
            float maxHaloEndBlinkIntensity = 0.51f;
            if(haloEndBlinkIntensity > maxHaloEndBlinkIntensity)haloEndBlinkIntensity = maxHaloEndBlinkIntensity;
            return haloEndBlinkIntensity;
        }
    }
    Sequence blinkAnimationSequence;
    Sequence haloBlinkAnimationSequence;
    Tween topTween_levelUp;
    Tween fillLiquidTween_levelUp;
    Tween topTween_createInIdelBox;
    Tween fillLiquidTween_createInIdelBox;
    bool counting = false;
    void Start()
    {
        ReadyLight.SetActive(true);
        originPositionTop = TopLiquid.GetComponent<RectTransform>().anchoredPosition;
        ideaBox = transform.GetComponent<IdelBox>();
        TopLiquid.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        ColumnLiquid.GetComponent<Image>().fillAmount = 1;
        resetStartBlinkIntensity = StartBlinkIntensity;
        resetEndBlinkIntensity = EndBlinkIntensity;
        resetHaloStartBlinkIntensity = HaloStartBlinkIntensity;
        resetHaloEndBlinkIntensity = HaloEndBlinkIntensity;
        blinkAnimationSequence = DOTween.Sequence();
        haloBlinkAnimationSequence = DOTween.Sequence();
        topTween_levelUp = DOTween.Sequence();
        fillLiquidTween_levelUp = DOTween.Sequence();
        topTween_createInIdelBox = DOTween.Sequence();
        fillLiquidTween_createInIdelBox = DOTween.Sequence();
        UpLevelBlinkAnimation();
    }
    void UpLevelBlinkAnimation()
    {
        if(blinkAnimationSequence!=null)
        {
            blinkAnimationSequence?.Kill();
            blinkAnimationSequence = null;
        }
        if(blinkAnimationSequence!=null)
        {
            haloBlinkAnimationSequence?.Kill();
            haloBlinkAnimationSequence = null;
        }
        int loopForever = -1;
        startBlinkIntensity += upLevelBlinkAnimationStrengthIncrement * Level;
        endBlinkIntensity += upLevelBlinkAnimationStrengthIncrement * Level;
        blinkAnimationSequence = DOTween.Sequence();
        Image readyLightImage = ReadyLight.GetComponent<Image>();
        readyLightImage.color = new Color(readyLightImage.color.r,readyLightImage.color.g,readyLightImage.color.b,StartBlinkIntensity * Level);
        blinkAnimationSequence.Append(readyLightImage.DOFade(StartBlinkIntensity, blinkDuration / 2));
        blinkAnimationSequence.Append(readyLightImage.DOFade(EndBlinkIntensity, blinkDuration / 2));
        blinkAnimationSequence.SetLoops(loopForever,LoopType.Yoyo); 
        if( ReadyLight.transform.childCount <= 0 )return;
        Image haloImage = Halo.GetComponent<Image>();
        haloImage.color = new Color(haloImage.color.r,haloImage.color.g,haloImage.color.b,HaloStartBlinkIntensity * Level);
        haloStartBlinkIntensity += upLevelBlinkAnimationStrengthIncrement * Level;
        haloEndBlinkIntensity += upLevelBlinkAnimationStrengthIncrement * Level;
        haloBlinkAnimationSequence = DOTween.Sequence();
        haloBlinkAnimationSequence.Append(haloImage.DOFade(HaloStartBlinkIntensity, haloblinkDuration / 2));
        haloBlinkAnimationSequence.Append(haloImage.DOFade(HaloEndBlinkIntensity, haloblinkDuration / 2));
        haloBlinkAnimationSequence.SetLoops(loopForever,LoopType.Yoyo); 
        
    }
    public void DoCount()
    {
        
        if(counting)
        {
            if(!IsInvoking(nameof(DoCount)))
            Invoke(nameof(DoCount),0.1f);
            return;
        }
        counting = true;
        if(IsInvoking(nameof(DoCount)))CancelInvoke(nameof(DoCount));
        
        ResetLiquidLoader();
        ResetLiquidLoader_UpLevel();
        TopLiquid.GetComponent<RectTransform>().anchoredPosition = originPositionTop;
        ReadyLight.SetActive(false);
        // ideaBox.idelContainer.SetActive(false);
        ideaBox.ClickEvent.SetActive(false);
        Vector3 finallyReachesPosition = new Vector3(-28.3f,390f,0f);
        float fillAmountStart = 0;
        float fillAmountEnd = 1;
        float preventionKillTweenWhenUsing = 0.1f;
        topTween_createInIdelBox = TopLiquid.GetComponent<RectTransform>().DOAnchorPos(finallyReachesPosition,inIdelbox_CreatCountdown - preventionKillTweenWhenUsing);
        fillLiquidTween_createInIdelBox = DOVirtual.Float(fillAmountStart, fillAmountEnd, inIdelbox_CreatCountdown, (TweenCallback<float>)((float value) =>
        {
            fade = value;
            this.ColumnLiquid.GetComponent<Image>().fillAmount = value;
        }));
        fillLiquidTween_createInIdelBox.onComplete=() =>
        {
            ReadyLight.SetActive(true);
            ideaBox.ClickEvent.SetActive(true);
            // ideaBox.idelContainer.SetActive(true);
            ideaBox.OnGameObjCreate();
            Invoke(nameof(DoCount_UpLevel),preventionKillTweenWhenUsing);
        };
    }
    public void DoCount_UpLevel()
    {
        counting = false;
        ResetLiquidLoader_UpLevel();
        int maxLevel = 3;
        if(level >= maxLevel)return;
        float preventionKillTweenWhenUsing = 0.1f;
        // TopLiquid.GetComponent<RectTransform>().anchoredPosition = originPositionTop;
        // Vector3 finallyReachesPosition = new Vector3(-28.3f,390f,0f);
        float fillAmountStart = 0;
        float fillAmountEnd = 1;
        // topTween_levelUp = TopLiquid.GetComponent<RectTransform>().DOAnchorPos(finallyReachesPosition,inIdelbox_UpLevelCountdown);
        fillLiquidTween_levelUp = DOVirtual.Float(fillAmountStart, fillAmountEnd, inIdelbox_UpLevelCountdown, (TweenCallback<float>)((float value) =>
        {
            // fade = value;
            // this.ColumnLiquid.GetComponent<Image>().fillAmount = value;
        }));
        Invoke(nameof(UpLevelBlinkAnimation),preventionKillTweenWhenUsing);
        fillLiquidTween_levelUp.onComplete=() =>
        {
            Level++;
            Invoke(nameof(DoCount_UpLevel),preventionKillTweenWhenUsing);
        };
    }
    public void ResetLiquidLoader_UpLevel()
    {
        if(topTween_levelUp!=null)
        {
            topTween_levelUp?.Kill(); 
            topTween_levelUp = null;
        }
        if(fillLiquidTween_levelUp!=null)
        {
            fillLiquidTween_levelUp?.Kill(); 
            fillLiquidTween_levelUp = null;
        }
        if(fillLiquidTween_createInIdelBox!=null)
        {
            fillLiquidTween_createInIdelBox?.Kill(); 
            fillLiquidTween_createInIdelBox = null;
        }
        if(topTween_createInIdelBox!=null)
        {
            topTween_createInIdelBox?.Kill(); 
            topTween_createInIdelBox = null;
        }
        if(topTween_levelUp!=null || fillLiquidTween_levelUp!=null || fillLiquidTween_createInIdelBox!=null || topTween_createInIdelBox!=null)
        {
            ResetLiquidLoader_UpLevel();
        }
    }
    public void ResetLiquidLoader()
    {
        level = 0;
        List<Tween> tweens = new List<Tween>();
        if(blinkAnimationSequence!=null){tweens.Add(blinkAnimationSequence);}
        if(haloBlinkAnimationSequence!=null){tweens.Add(haloBlinkAnimationSequence);}
        if(topTween_levelUp!=null){tweens.Add(topTween_levelUp);}
        if(fillLiquidTween_levelUp!=null){tweens.Add(fillLiquidTween_levelUp);}
        if(topTween_createInIdelBox!=null){tweens.Add(topTween_createInIdelBox);}
        if(fillLiquidTween_createInIdelBox!=null){tweens.Add(fillLiquidTween_createInIdelBox);}
        for(int i = 0; i < tweens.Count; i++)
        {
            if(tweens[i] == null)continue;
            if(tweens[i]!=null){tweens[i]?.Kill(); tweens[i]=null;}
        }
        startBlinkIntensity = resetStartBlinkIntensity;
        endBlinkIntensity = resetEndBlinkIntensity;
        haloStartBlinkIntensity = resetHaloStartBlinkIntensity;
        haloEndBlinkIntensity = resetHaloEndBlinkIntensity;
        Image readyLightImage = ReadyLight.GetComponent<Image>();
        readyLightImage.color = new Color(readyLightImage.color.r,readyLightImage.color.g,readyLightImage.color.b,0);
        Image haloImage = Halo.GetComponent<Image>();
        haloImage.color = new Color(haloImage.color.r,haloImage.color.g,haloImage.color.b,0);
        ColumnLiquid.GetComponent<Image>().fillAmount = 0;
        TopLiquid.GetComponent<RectTransform>().anchoredPosition = originPositionTop;
    }
    [Client]
    public void Client_DoCount()
    {
        TopLiquid.GetComponent<RectTransform>().anchoredPosition = originPositionTop;
        ReadyLight.SetActive(false);
        // ideaBox.idelContainer.SetActive(false);
        TopLiquid.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-46f,59f,0f),6f);
        DOVirtual.Float(0, 1, 6.3f, (TweenCallback<float>)((float value) =>
        {
            fade = value;
            this.ColumnLiquid.GetComponent<Image>().fillAmount = value;
            foreach(var tetris in FindObjectsOfType<TetrisBlockSimple>())
            {
                if(tetris.player != Player.NotReady)return;
                // 是否需要在 客户端idelBox 还没获取的时候 隐藏 俄罗斯方块组
            }
        })).onComplete=() =>
        {
            ReadyLight.SetActive(true);
            // ideaBox.idelContainer.SetActive(true);
            ideaBox.ClientGetTetrisGroupID();
        };
        
    }
    public void ChangeColor(Color color)
    {
        if(!ideaBox.idelUI.hiden)
        {
            ColumnLiquid.GetComponent<Image>().DOColor(color,0.5f);
            TopLiquid.GetComponent<Image>().DOColor(color,0.5f);
        }else
        {
            Color colorHiden = new(color.r,color.g,color.b,Dispaly.HidenAlpha);
            ColumnLiquid.GetComponent<Image>().DOColor(colorHiden,0.5f);
            TopLiquid.GetComponent<Image>().DOColor(colorHiden,0.5f);
        }
        
    }
    


  
}
