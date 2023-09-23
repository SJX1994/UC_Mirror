using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillCooldownTimer : MonoBehaviour
{
    public SkillCooldown countersRedUI,countersGreenUI,counterBlueUI,counterPurpleUI;
    public Sprite outerActiveSprite;
    public Sprite outerNotActiveSprite;
    void Start()
    {
        // Debug.Log("SkillCooldownTimer Start:" + transform.name);
    }
  
}
