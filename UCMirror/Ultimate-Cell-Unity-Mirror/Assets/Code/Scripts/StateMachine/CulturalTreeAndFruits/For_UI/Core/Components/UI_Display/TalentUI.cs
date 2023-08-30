using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace com.cygnusprojects.TalentTree
{
	public class TalentUI : MonoBehaviour 
	{
        #region Variables
        public List<TalentTreeConnectionBase> toConns;
        public List<TalentTreeConnectionBase> fromConns;
        public TalentTreeNodeBase Talent;
        public TalentusEngine Engine;
        public Text NameLabel;
        public Text LevelIndicator;
        public Text CostLabel;
        public Image IconField;

        public Image TierIndicator;
        public Image TalentBackground;
        public Color EnabledColor = Color.white;
        public Color DisabledColor = Color.gray;

        public Button buyButton;
        public Button revertButton;
        public Button collectButton;

        public UnityAction<TalentUI> OnBuy;
        public UnityAction<TalentUI> OnRevert;
        public UnityAction<TalentUI> OnCollected;

        private Sprite ImageUI;
        private Sprite DisabledImageUI;
        

        #endregion

        #region Unity Methods                
        private void TalentTree_TreeEvaluated(object sender, TalentTreeGraph.TreeEvaluatedEventArgs e)
        {
            UpdateUI();
        }

        private void OnEnable()
        {
            if (Engine != null)
                Engine.TalentTree.TreeEvaluated += TalentTree_TreeEvaluated;

            if (buyButton != null)
                buyButton.gameObject.SetActive(false);
            if (revertButton != null)
                revertButton.gameObject.SetActive(false);

            // Cache the Images as Sprites
            if (Talent != null)
            {
                if (IconField != null) ImageUI = Talent.ImageAsSprite;
                if (Talent.DisabledImage != null) DisabledImageUI = Talent.DisabledImageAsSprite;
            }

            UpdateUI();
        }

        private void OnDisable()
        {
            if (Engine != null)
                Engine.TalentTree.TreeEvaluated -= TalentTree_TreeEvaluated;
        }

        void UpdateUI()
        {            
            if (Talent != null)
            {               
                if (NameLabel != null)
                {
                    NameLabel.text = Talent.Name;
                }
                if (IconField != null)
                {
                    IconField.sprite = ImageUI; //Talent.ImageAsSprite;
                    if (!Talent.isValid && Talent.DisabledImage != null)
                    {
                        IconField.sprite = DisabledImageUI; //Talent.DisabledImageAsSprite;
                    }
                }
                if (LevelIndicator != null)
                {
                    LevelIndicator.text = string.Format("{0}/{1}",Talent.Level, Talent.MaxLevel);
                }
                if (CostLabel != null)
                {
                    CostLabel.text = string.Format("{0}", Talent.GetCostForNextLevel().Cost);
                    if(Talent.GetCostForNextLevel().Cost == 0)
                    {
                        CostLabel.text = string.Format("已拥有");
                    }
                }
                if(collectButton != null)
                {
                    if(Talent.GetCostForNextLevel().Cost == 0)
                    {
                        CostLabel.text = string.Format("已拥有");
                        collectButton.gameObject.SetActive(true);
                    }else
                    {
                        collectButton.gameObject.SetActive(false);
                    }
                }
                if (TierIndicator != null)
                {
                    TierIndicator.color = Talent.Tier.EditorColor;
                }
                if (TalentBackground != null)
                {
                    if (Talent.isValid)
                    {
                        TalentBackground.color = EnabledColor;
                    }
                    else
                    {
                        TalentBackground.color = DisabledColor;
                    }
                }
                if (buyButton != null)
                {
                    if (Talent.isValid)
                    {
                        buyButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        buyButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void DoBuy()
        {
            Engine.BuyTalent(Talent);
            OnBuy?.Invoke(this);
            if (revertButton != null)
            {
                revertButton.gameObject.SetActive(true);
            }
        }

        public void DoRevert()
        {
            Engine.RevertTalent(Talent);
            OnRevert?.Invoke(this);
            if (buyButton != null)
            {
                buyButton.gameObject.SetActive(true);
            }
            if (revertButton != null)
            {
                revertButton.gameObject.SetActive(false);
            }
        }
		
        public void DoCollect()
        {
            OnCollected?.Invoke(this);
        }
        #endregion
	}
}
