using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StoryInputManager : Singleton<StoryInputManager>
{
  
    public TimelineMode timelineMode = TimelineMode.Playing;
    private PlayableDirector activeDirector;
    public enum TimelineMode
	{
		Playing,//过场动画, 
		DialogueMoment, // 等待输入
	}

    private void Update()
	{
        switch(this.timelineMode)
        {
            case TimelineMode.Playing:
                

            break;
            case TimelineMode.DialogueMoment:
                if(Input.GetMouseButtonUp(0))
				{
					ResumeTimeline();
				}
            break;
        }
    }

    public void PauseTimeline(PlayableDirector whichOne)
	{
        if (whichOne.playableGraph.IsValid())
		{
            activeDirector = whichOne;
            activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
            timelineMode = TimelineMode.DialogueMoment; // InputManager 将等待输入然后继续
            StoryUIManager.Instance.TogglePressSpacebarMessage(true);
        }
		
	}
    public void ResumeTimeline()
	{
		StoryUIManager.Instance.TogglePressSpacebarMessage(false);
		StoryUIManager.Instance.ToggleDialoguePanel(false);
		activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
		timelineMode = TimelineMode.Playing;
	}
}
