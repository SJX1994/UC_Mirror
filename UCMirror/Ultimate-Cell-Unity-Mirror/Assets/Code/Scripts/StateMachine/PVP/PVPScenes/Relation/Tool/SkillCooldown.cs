using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.Events;
public class SkillCooldown : MonoBehaviour
{
      public Image innerImage;
      public Image outterImage;
      public TextMeshProUGUI textMeshPro;
      float startNumber;
      float endNumber;
      SkillCooldownTimer skillCooldownTimer;
      public MoraleTemplate.SoldierType soldierType;
      public UnityAction<MoraleTemplate.SoldierType> CooldownReady;
      void Start()
      {
            skillCooldownTimer = FindObjectOfType<SkillCooldownTimer>();
            innerImage = transform.Find("Inner").GetComponent<Image>();
            outterImage = transform.Find("Outer").GetComponent<Image>();
            textMeshPro = transform.Find("Counter").GetComponent<TextMeshProUGUI>();
            startNumber = 1.5f;
            endNumber = 0.0f;
            NotActive(true);
      }

      void Active()
      {
            outterImage.sprite = skillCooldownTimer.outerActiveSprite;
            innerImage.color = Color.white;
            textMeshPro.gameObject.SetActive(false);
            if(CooldownReady!=null)
            {
                  CooldownReady(soldierType);
            }
      }
      public void NotActive(bool coolDown = false)
      {
            outterImage.sprite = skillCooldownTimer.outerNotActiveSprite;
            innerImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            textMeshPro.text = startNumber.ToString();
            textMeshPro.gameObject.SetActive(true);
            if(coolDown)
            {
                  DOTween.To(() => startNumber, x => textMeshPro.text = Math.Round(x, 1).ToString(), endNumber, startNumber).onComplete = () =>
                  {
                        Active();
                  };
            }
            
      }
}