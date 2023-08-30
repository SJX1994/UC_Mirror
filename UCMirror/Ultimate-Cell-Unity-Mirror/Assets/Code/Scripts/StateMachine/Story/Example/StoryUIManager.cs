using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
public class StoryUIManager : Singleton<StoryUIManager>
{
    public Text charNameText, dialogueLineText;
    public GameObject toggleSpacebarMessage, dialoguePanel,skipButton,replayButton;
    [HideInInspector]
    public PlayableDirector director;
    
    public void SetDialogue(string charName, string lineOfDialogue, int sizeOfDialogue)
	{
		charNameText.text = charName;
		dialogueLineText.text = lineOfDialogue;
		dialogueLineText.fontSize = sizeOfDialogue;

		ToggleDialoguePanel(true);
		ToggleSkipButton(true);
		ToggleReplayButton(true);
	}
    public void MoveToStoryStart()
    {
		director.time = 0f;
    }
    public void MoveToStoryFram()
    {
		director.time = director.duration;
		ToggleSkipButton(false);
		ToggleReplayButton(false);
    }
    public void ToggleReplayButton(bool active)
    {
	replayButton.SetActive(active);
    }	
    public void ToggleSkipButton(bool active)
    {
	skipButton.SetActive(active);
    }
    public void ToggleDialoguePanel(bool active)
	{
		dialoguePanel.SetActive(active);
	}
    public void TogglePressSpacebarMessage(bool active)
	{
		toggleSpacebarMessage.SetActive(active);
	}
}
