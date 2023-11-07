using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UC_PlayerData;
using System.Linq;
using TMPro;
public class FloatingWord_FaceToCamera : MonoBehaviour
{
    public Player player = Player.NotReady;
    public int floatingWord_Id = 0;
    public Vector3 originPos;
    private TextMeshPro textMeshPro;
    public TextMeshPro TextMeshPro
    {
        get
        {
            if(!textMeshPro)textMeshPro = GetComponent<TextMeshPro>();
            return textMeshPro;
        }
        set
        {
            if(!textMeshPro)textMeshPro = GetComponent<TextMeshPro>();
            textMeshPro = value;
        }
    }
    public string textToShow = "";
    void Update () 
    {
        transform.rotation = Camera.main.transform.rotation;
    }
    void Awake()
    {
        originPos = transform.position;    
    }
    public void Active()
    {
        transform.position = originPos;
        TextMeshPro.text = textToShow;
        TextMeshPro.fontSize = transform.GetComponent<TextMeshPro>().fontSize + Random.Range(0,1);
        UIData.player1FloatingWordIDTemp += 1;
        floatingWord_Id = UIData.player1FloatingWordIDTemp;
        Tween moveUp = transform.DOMoveZ(transform.position.z + floatingWord_Id * 0.3f , floatingWord_Id * 0.3f).SetEase(Ease.OutSine);
    }
}
