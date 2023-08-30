using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

public class FruitsManager : Singleton<FruitsManager>
{
    public RectTransform fruitDescription;
    Text fruitNameText;
    Text fruitDescriptionText;
    Text fruitExplanationText;
    public List<FruitDataStructureTemplate> cellAvailableFruits;
    [HideInInspector]
    public List<FruitDataStructureTemplate> cellEquippedFruits;

    public List<FruitDataStructureTemplate> virusAvailableFruits;
    [HideInInspector]
    public List<FruitDataStructureTemplate> virusEquippedFruits;
    public List<Fruit> cellFruits;
    public List<Fruit> virusFruits;
    public UnityAction OnFruitsLoaded;
    // 设置要使用的 Sorting Layer 名称
      public string sortingLayerName = "YourSortingLayerName";
      // 设置要使用的 Sorting Order
      public int sortingOrder = 0;
    // Start is called before the first frame update
    void Start()
    {
        transform.parent.GetComponent<Canvas>().sortingLayerName = sortingLayerName;
        transform.parent.GetComponent<Canvas>().sortingOrder = sortingOrder;
        if(fruitDescription)
        {
            fruitNameText = fruitDescription.Find("Text_FruitName").GetComponent<Text>();
            fruitDescriptionText = fruitDescription.Find("Text_FruitDescription").GetComponent<Text>();
            fruitExplanationText = fruitDescription.Find("Text_FruitExplanation").GetComponent<Text>();
            fruitNameText.color = Color.gray;
            fruitDescriptionText.color = Color.gray;
            fruitExplanationText.color = Color.gray;
        }
        Invoke(nameof(Init), 0.1f);
    }
    void Init()
    {
        EquipCellSkill(0);
        EquipCellSkill(1);
        EquipVirusSkill(0);

        CellLoadSkill();
        VirusLoadSkill();
        UseSkill();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    // 塞尔装配技能
    public void EquipCellSkill(int index)
    {
        if (index >= 0 && index < cellAvailableFruits.Count)
        {
            FruitDataStructureTemplate skillToEquip = Instantiate(cellAvailableFruits[index]);
            if (!cellEquippedFruits.Contains(skillToEquip))
            {
                cellEquippedFruits.Add(skillToEquip);
            }
        }
    }

    // 外尔装配技能
    public void EquipVirusSkill(int index)
    {
        if (index >= 0 && index < virusAvailableFruits.Count)
        {
            FruitDataStructureTemplate skillToEquip = Instantiate(virusAvailableFruits[index]);
            if (!virusEquippedFruits.Contains(skillToEquip))
            {
                virusEquippedFruits.Add(skillToEquip);
            }
        }
    }
    // 塞尔加载技能
    public void CellLoadSkill()
    {
        for(int i = 0; i < cellEquippedFruits.Count; i++)
        {
            if(cellFruits[i].fruitDataStructureTemplate == null)
            {
                cellFruits[i].fruitDataStructureTemplate = cellEquippedFruits[i];
                cellFruits[i].delayTime = i * 0.1f;
            }
        }
    }
    // 外尔加载技能
    public void VirusLoadSkill()
    {
        for(int i = 0; i < virusEquippedFruits.Count; i++)
        {
            if(virusFruits[i].fruitDataStructureTemplate == null)
            {
                virusFruits[i].fruitDataStructureTemplate = virusEquippedFruits[i];
                virusFruits[i].delayTime = i * 0.1f;
            }
        }
    }
    public void UseSkill()
    {
        fruitDescription.gameObject.SetActive(false);
        
        List<Fruit> allFruitUI = new();
        allFruitUI.AddRange(cellFruits);
        allFruitUI.AddRange(virusFruits);
        foreach(Fruit fruit in allFruitUI)
        {
            fruit.OnFruitHover+=OnFruitHover;
        }
        OnFruitsLoaded?.Invoke();
    }
    private Tweener tweenerPosGo;
    private Tweener tweenerScaleGo;
    void OnFruitHover(Fruit fruit,bool isMouseHovering)
    {
        fruitNameText.color = Color.gray;
        fruitDescriptionText.color = Color.gray;
        fruitExplanationText.color = Color.gray;
        fruitDescription.gameObject.SetActive(false);
        fruitDescription.position = fruit.transform.position;
        fruitDescription.localScale = Vector3.zero;

        if(isMouseHovering)
        {
            
            fruitDescription.gameObject.SetActive(true);
            
            tweenerPosGo = fruitDescription.DOAnchorPos(Vector2.zero, 0.5f);
            tweenerScaleGo = fruitDescription.DOScale(Vector3.one, 0.5f);

            fruitNameText.gameObject.SetActive(true);
            fruitDescriptionText.gameObject.SetActive(true);
            fruitExplanationText.gameObject.SetActive(true);
            tweenerScaleGo.OnComplete(() =>
            {
                fruitNameText.text = StringChanger(fruit.fruitDataStructureTemplate.fruitName);
                fruitDescriptionText.text = StringChanger(fruit.fruitDataStructureTemplate.fruitDescription);
                fruitExplanationText.text = StringChanger(fruit.fruitDataStructureTemplate.fruitExplanation);
                fruitNameText.transform.localScale = Vector3.zero;
                fruitDescriptionText.transform.localScale = Vector3.zero;
                fruitExplanationText.transform.localScale = Vector3.zero;
                
                
                fruitNameText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)
                .OnComplete(() =>{

                    fruitNameText.color = fruit.fruitDataStructureTemplate.FruitColor()[0] + Color.white * 0.4f;

                    fruitDescriptionText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)
                    .OnComplete(() =>{

                        fruitDescriptionText.color = fruit.fruitDataStructureTemplate.FruitColor()[0] + Color.white * 0.4f;

                        fruitExplanationText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>{

                            fruitExplanationText.color = fruit.fruitDataStructureTemplate.FruitColor()[0] ;

                        });
                    });
                });
                
            });
        

        }else{
            tweenerPosGo.Kill();
            tweenerScaleGo.Kill();
            fruitDescription.gameObject.SetActive(false);
            fruitNameText.color = Color.gray;
            fruitDescriptionText.color = Color.gray;
            fruitExplanationText.color = Color.gray;
            fruitNameText.gameObject.SetActive(false);
            fruitDescriptionText.gameObject.SetActive(false);
            fruitExplanationText.gameObject.SetActive(false);
            
        }

    }
    string StringChanger(string str)
    {
        return str.Replace(",", "\n").Replace("，", "\n").Replace("!", "\n").Replace("！", "\n");
    }
}
