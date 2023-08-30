using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class DialogueClip : PlayableAsset, ITimelineClipAsset
{
    public DialogueBehaviour dialogTemplate = new DialogueBehaviour ();
    public ClipCaps clipCaps 
    {
       get { return ClipCaps.None; }
    }
    // 基于此资产生成可播放的工厂方法
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create (graph, dialogTemplate);
        return playable;
    }
}
