using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UC_PlayerData;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using Mirror;
public class Avatar_Leader_spine : NetworkBehaviour
{
#region 数据对象
    private Image dirtyTalkImage;
    public Image DirtyTalkImage
    {
        get
        {
            if(dirtyTalkImage)return dirtyTalkImage;
            dirtyTalkImage = transform.Find("DirtyTalk").GetComponent<Image>();
            return dirtyTalkImage;
        }
    }
    public LeaderData.LeaderPosition leaderPosition;
    public LeaderData.LeaderType leaderType;
    LeaderData leaderData = new();
    private SkeletonGraphic skeletonGraphic;
    public SkeletonGraphic SkeletonGraphic
    {
        get
        {
            if(skeletonGraphic)return skeletonGraphic;
            skeletonGraphic = transform.Find("Spine").GetComponent<SkeletonGraphic>();
            return skeletonGraphic;
        }
    }
    private GameObject otherLeader;
    public GameObject OtherLeader
    {
        get
        {
            if(otherLeader)return otherLeader;
            otherLeader = FindObjectsOfType<Avatar_Leader_spine>().Where(x=>x.leaderPosition != leaderPosition).FirstOrDefault().gameObject;
            return otherLeader;
        }
    }
    public Tween tween_dirtyTalk;
#endregion 数据对象
#region 联网数据对象
    int dirtyImageIndex = 0;
#endregion 联网数据对象
#region 数据关系
    void Start()
    {
        leaderData.leaderPosition = leaderPosition;
        leaderData.leaderType = leaderType;
        leaderData.leaderDirection = leaderPosition == LeaderData.LeaderPosition.Left ? LeaderData.LeaderDirection.Right : LeaderData.LeaderDirection.Left;
        leaderData.leaderState = LeaderData.LeaderState.Idle;
        LeaderData.whoTalking = LeaderData.WhoTalking.None;
        PlayAnimation("idle", true); 
        DirtyTalkImageDo(false);

    }
    public void PlayAttackAnimation_buttonClick()
    {
        if(Local())
        {
            leaderData.leaderState = LeaderData.LeaderState.Attack;
            PlayAnimation(leaderData.leaderState, false);
            DirtyTalk();
        }else
        {
            // Debug.Log("PlayAttackAnimation_buttonClick!!!");
            if(!isClient)return;
            if(leaderData.leaderPosition == LeaderData.LeaderPosition.Left && ServerLogic.local_palayer != Player.Player1)return;
            if(leaderData.leaderPosition == LeaderData.LeaderPosition.Right && ServerLogic.local_palayer != Player.Player2)return;
            leaderData.leaderState = LeaderData.LeaderState.Attack;
            Client_PlayAnimation(leaderData.leaderState, false);
        }
        
    }
#endregion 数据关系
#region 数据操作
    void PlayAnimation(LeaderData.LeaderState leaderState,bool loop)
    {
        switch(leaderState)
        {
            case LeaderData.LeaderState.Idle:
                PlayAnimation("idle", loop);
                break;
            case LeaderData.LeaderState.Attack:
                PlayAnimation("attack", loop);
                break;
            case LeaderData.LeaderState.Die:
                PlayAnimation("die", loop);
                break;
            case LeaderData.LeaderState.Move:
                PlayAnimation("move", loop);
                break;
        }
    }
    

    void PlayAnimation(string animationName, bool loop)
    {
        SkeletonGraphic.AnimationState.ClearTracks();
        Spine.TrackEntry track = SkeletonGraphic.AnimationState.SetAnimation(0, animationName, loop);
        track.Complete += AnimationCompleteCallback;
    }

    private void AnimationCompleteCallback(Spine.TrackEntry trackEntry)
    {
        leaderData.leaderState = LeaderData.LeaderState.Idle;
        PlayAnimation(leaderData.leaderState, true);
    }
    void DirtyTalk()
    {
        LeaderData.whoTalking = leaderData.leaderPosition == LeaderData.LeaderPosition.Left? LeaderData.WhoTalking.Player1: LeaderData.WhoTalking.Player2;
        if(LeaderData.whoTalking == LeaderData.WhoTalking.Player1)
        {
            DirtyTalkImageDo(true);
            OtherLeader.GetComponent<Avatar_Leader_spine>().DirtyTalkImageDo(false);
        }else if(LeaderData.whoTalking == LeaderData.WhoTalking.Player2)
        {
            DirtyTalkImageDo(true);
            OtherLeader.GetComponent<Avatar_Leader_spine>().DirtyTalkImageDo(false);
        }
        
    }
    void DirtyTalkImageDo(bool talk)
    {
        tween_dirtyTalk?.Kill();
        DirtyTalkImage.gameObject.transform.localScale = new Vector3(0,0,0);
        if(talk)
        {
            float Xflip = leaderData.leaderDirection == LeaderData.LeaderDirection.Left ? 1 : -1;
            dirtyImageIndex = Random.Range(0,4);
            DirtyTalkImage.sprite = Resources.Load<Sprite>("DirtyTalk/" + dirtyImageIndex.ToString());
            DirtyTalkImage.SetNativeSize();
            DirtyTalkImage.gameObject.SetActive(true);
            tween_dirtyTalk = DirtyTalkImage.gameObject.transform.DOScale(new Vector3(Xflip,1f,1f), Random.Range(0.2f,1.0f)).SetEase(Ease.OutBack);
        }else
        {
            DirtyTalkImage.gameObject.SetActive(false);
        }
    }
#endregion 数据操作
#region 联网数据操作
    bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    [Client]
    void Client_PlayAnimation(LeaderData.LeaderState leaderState,bool loop)
    {
        Server_PlayAnimation(leaderData.leaderState, true);
    }
    [Command(requiresAuthority = false)]
    void Server_PlayAnimation(LeaderData.LeaderState leaderState,bool loop)
    {
        PlayAnimation(leaderData.leaderState, true);
        DirtyTalk();
        Rpc_PlayAnimation(leaderData.leaderState, true);
        Rpc_DirtyTalk(dirtyImageIndex,leaderData.leaderPosition);
    }
    [ClientRpc]
    void Rpc_PlayAnimation(LeaderData.LeaderState leaderState,bool loop)
    {
        PlayAnimation(leaderData.leaderState, true);
    }
    [ClientRpc]
    void Rpc_DirtyTalk(int dirtyImageIndex,LeaderData.LeaderPosition leaderPosition)
    {
        LeaderData.whoTalking = leaderPosition == LeaderData.LeaderPosition.Left? LeaderData.WhoTalking.Player1: LeaderData.WhoTalking.Player2;
        if(LeaderData.whoTalking == LeaderData.WhoTalking.Player1)
        {
            Rpc_DirtyTalkImageDo(true,dirtyImageIndex);
            OtherLeader.GetComponent<Avatar_Leader_spine>().Rpc_DirtyTalkImageDo(false,dirtyImageIndex);
        }else if(LeaderData.whoTalking == LeaderData.WhoTalking.Player2)
        {
            Rpc_DirtyTalkImageDo(true,dirtyImageIndex);
            OtherLeader.GetComponent<Avatar_Leader_spine>().Rpc_DirtyTalkImageDo(false,dirtyImageIndex);
        }
    }
    void Rpc_DirtyTalkImageDo(bool talk, int dirtyImageIndex)
    {
        tween_dirtyTalk?.Kill();
        DirtyTalkImage.gameObject.transform.localScale = new Vector3(0,0,0);
        if(talk)
        {
            float Xflip = leaderData.leaderDirection == LeaderData.LeaderDirection.Left ? 1 : -1;
            DirtyTalkImage.sprite = Resources.Load<Sprite>("DirtyTalk/" + dirtyImageIndex.ToString());
            DirtyTalkImage.SetNativeSize();
            DirtyTalkImage.gameObject.SetActive(true);
            tween_dirtyTalk = DirtyTalkImage.gameObject.transform.DOScale(new Vector3(Xflip,1f,1f), Random.Range(0.2f,1.0f)).SetEase(Ease.OutBack);
        }else
        {
            DirtyTalkImage.gameObject.SetActive(false);
        }
    }
#endregion 联网数据操作
}
