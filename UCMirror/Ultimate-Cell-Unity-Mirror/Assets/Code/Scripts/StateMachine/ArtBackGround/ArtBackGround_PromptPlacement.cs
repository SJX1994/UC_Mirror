using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using DG.Tweening;

public class ArtBackGround_PromptPlacement : MonoBehaviour
{
    MeshRenderer meshRenderer;
    private Material material;
    private float originalAlpha;
    Sequence blinkSequence;
    void Start()
    {
        
        Invoke(nameof(Init),0.3f);
    }
    void Init()
    {
        meshRenderer = transform.GetComponent<MeshRenderer>();
        // meshRenderer.sharedMaterial.DOFade(1.0f,0.5f);
        Invoke(nameof(OnTimeBeforStartFinish_FromKeyTimeCounter),Referee.InIdelbox_CreatFirstCountdown + 1f);
        Blinking();
    }
    void Blinking()
    {
        float blinkDuration = 0.5f; // 闪烁动画的总持续时间
        int blinkCount = -1; // -1表示无限闪烁，大于0的数字表示闪烁次数

        // 获取材质
        material = meshRenderer.material;

        // 记录初始透明度
        originalAlpha = material.color.a;

        // 创建一个Tween对象实现闪烁效果
        blinkSequence = DOTween.Sequence();

        // 添加闪烁动画的关键帧
        blinkSequence
            .Append(meshRenderer.sharedMaterial.DOFade(1.0f,blinkDuration))
            .Append(meshRenderer.sharedMaterial.DOFade(0.5f,blinkDuration))
            .SetLoops(blinkCount, LoopType.Restart)
            .OnComplete(() => meshRenderer.enabled = true);

        // 启动闪烁动画
        blinkSequence.Play();
        
    }
    void OnTimeBeforStartFinish_FromKeyTimeCounter()
    {
        blinkSequence?.Kill();
        meshRenderer.sharedMaterial.DOFade(0.0f,3.5f);
    }
}