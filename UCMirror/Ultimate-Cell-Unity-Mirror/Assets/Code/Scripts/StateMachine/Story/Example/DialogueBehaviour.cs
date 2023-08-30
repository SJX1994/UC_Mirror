using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
#region 数据对象
   
    public string characterName;
    public string dialogueLine;
    public int dialogueSize;
    private bool clipPlayed = false;
    public bool hasToPause = false; // 为了不跳过关键剧情，需要用户操作以继续
    private bool pauseScheduled = false;
    private PlayableDirector director;

    
#endregion 数据对象
#region 数据关系
    // 创建时
    public override void OnPlayableCreate(Playable playable)
	{
        
		director = (playable.GetGraph().GetResolver() as PlayableDirector);
		StoryUIManager.Instance.director = director;
	}
    // 当前帧信息
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if(!clipPlayed && info.weight > 0f)
		{
			StoryUIManager.Instance.SetDialogue(characterName, dialogueLine, dialogueSize);

			if(Application.isPlaying)
			{
				if(hasToPause)
				{
					pauseScheduled = true;
				}
			}

			clipPlayed = true;
		}
	}
    // 播放状态为 Playing 时停止。
	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
        
		if(pauseScheduled)
		{
			pauseScheduled = false;
            StoryInputManager.Instance.PauseTimeline(director);
		}
		else
		{
			StoryUIManager.Instance.ToggleDialoguePanel(false);
			StoryUIManager.Instance.ToggleReplayButton(false);
			StoryUIManager.Instance.ToggleSkipButton(false);
		}

		clipPlayed = false;
	}


#endregion 数据关系
#region 数据操作
#endregion 数据操作
}