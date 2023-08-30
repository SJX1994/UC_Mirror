using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.cygnusprojects.TalentTree;
using System.Linq;
using DG.Tweening;
public class FruitsPicker : MonoBehaviour
{
    public RectTransform fruitColllectUI0;
    public RectTransform fruitColllectUI1;
    public RectTransform fruitColllectUI2;
    public RectTransform fruitColllectUI3;
    List<FruitPickerData> fruitColllectUIs = new List<FruitPickerData>();
    List<TalentUI> talentUIs = new List<TalentUI>();
    Sprite baseSprite;
    Color baseOutterColor;
    string baseName;
    // Start is called before the first frame update
    void Start()
    {
        talentUIs = FindObjectsOfType<TalentUI>().ToList();
        foreach(var talentUI in talentUIs)
        {
            talentUI.OnCollected += TalentUI_OnCollected;
            talentUI.OnRevert += TalentUI_OnRevert;
        }
        InitFruitPickerData(fruitColllectUI0);
        InitFruitPickerData(fruitColllectUI1);
        InitFruitPickerData(fruitColllectUI2);
        InitFruitPickerData(fruitColllectUI3);
        baseSprite = fruitColllectUIs[0].collecter.GetComponent<Image>().sprite;
        baseOutterColor = fruitColllectUIs[0].outter.GetComponent<Image>().color;
        baseName = fruitColllectUIs[0].fruitNamer.GetComponent<Text>().text;
    }
    private void TalentUI_OnRevert(TalentUI fruitReverted)
    {
        foreach(var fruitColllectUI in fruitColllectUIs)
        {
            if(fruitColllectUI.talentUI == fruitReverted)
            {
                OnCancel(fruitColllectUI);
                break;
            }
        }
    }
    private void TalentUI_OnCollected(TalentUI fruitCollected)
    {   
        
        if(fruitColllectUIs.All(value => value.isCollected == true))
        {
            float duration = 1.5f;
            float strength = 12.5f;
            int vibrato = 10;
            float randomness = 90f;
            transform.DOShakePosition(duration, new Vector3(strength, 0f, 0f), vibrato, randomness);
            
            return;
        }
        foreach(var fruitColllectUI in fruitColllectUIs)
        {
            if(!fruitColllectUI.isCollected)
            {
                fruitColllectUI.collecter.GetComponent<Image>().sprite = fruitCollected.IconField.sprite;
                fruitColllectUI.outter.GetComponent<Image>().color = fruitCollected.TierIndicator.color;
                fruitColllectUI.fruitNamer.GetComponent<Text>().text = fruitCollected.NameLabel.text;
                fruitColllectUI.cancelButton.gameObject.SetActive(true);
                fruitColllectUI.isCollected = true;
                fruitColllectUI.talentUI = fruitCollected;
                fruitCollected.collectButton.enabled = false;
                break;
            }
        }
    }
    void OnCancel(FruitPickerData cancelTarget)
    {
        cancelTarget.collecter.GetComponent<Image>().sprite = baseSprite;
        cancelTarget.outter.GetComponent<Image>().color = baseOutterColor;
        cancelTarget.fruitNamer.GetComponent<Text>().text = baseName;
        cancelTarget.cancelButton.gameObject.SetActive(false);
        cancelTarget.isCollected = false;
        cancelTarget.talentUI.collectButton.enabled = true;
        cancelTarget.talentUI = null;
    }
#region 数据方法
    void InitFruitPickerData(RectTransform fruitColllectUI)
    {
        FruitPickerData f0 = new FruitPickerData();
        f0.collecter = fruitColllectUI;
        f0.outter = FindChildObjectByName(fruitColllectUI,"Outter").GetComponent<RectTransform>();
        f0.fruitNamer = FindChildObjectByName(fruitColllectUI,"Name").GetComponent<RectTransform>();
        f0.cancelButton = FindChildObjectByName(fruitColllectUI,"Cancel").GetComponent<Button>();
        f0.cancelButton.gameObject.SetActive(false);
        f0.isCollected = false;
        f0.cancelButton.onClick.AddListener(() =>
        {
            // 当按钮被点击时执行的回调函数
            OnCancel(f0);
        });
        fruitColllectUIs.Add(f0);
    }
    GameObject FindChildObjectByName(RectTransform parent, string name)
    {
        int childCount = parent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            
            // Check if the child GameObject has a RectTransform component
            if (child.TryGetComponent<RectTransform>(out RectTransform childRectTransform))
            {
                // Check if the child GameObject has the specified name
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }
            
            // Recursively search for the child GameObject in its children
            GameObject foundChild = FindChildObjectByName(childRectTransform, name);
            if (foundChild != null)
            {
                return foundChild;
            }
        }
        
        return null; // Child GameObject with the specified name not found
    }
#endregion
}
class FruitPickerData
{
    public RectTransform collecter;
    public RectTransform outter;
    public RectTransform fruitNamer;
    public Button cancelButton;

    public TalentUI talentUI;
    public bool isCollected = false;
}