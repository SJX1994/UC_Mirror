
using Common;
using GameFrameWork;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResManager : SingTon<ResManager>
{
    private const string V1 = "20%";
    private const string V2 = "40%";
    private const string V3 = "60%";
    private const string V4 = "80%";

    public class Hero
    {
        /// <summary>
        /// _beConfig               �Ƿ�����ʹ��
        /// _name                   ��ɫ��
        /// _image                  ��ɫͼƬ
        /// _blood                  Ѫ��
        /// _unlock                 ����״̬
        /// _unlockName             δ����ʱ�������
        /// _activeSkillImg         ��������ͼƬ
        /// _passiveSkillImg        ��������ͼƬ
        /// _unlockSkillImg         δ��������ͼƬ
        /// _activeSkillName        ������������
        /// _activeSkillProfile     ������������
        /// _passiveSkillName       ������������
        /// _passiveSkillProfile    ������������
        /// _heroTags               Ӣ�۱�ǩ
        /// _unlockHeroTags         δ����ʱ��ı�ǩ
        /// </summary>
        public bool _beConfig {  set; get; }
        public string _name {  set; get; }
        public Sprite _image {  set; get; }
        public int _blood {  set; get; }
        public bool _unlock {  set; get; }
        public string _unlockName {  set; get; }
        public Sprite _activeSkillImg {  set; get; }
        public Sprite _passiveSkillImg {  set; get; }
        public Sprite _unlockSkillImg {  set; get; }
        public string _activeSkillName {  set; get; }
        public string _activeSkillProfile {  set; get; }
        public string _passiveSkillName {  set; get; }
        public string _passiveSkillProfile {  set; get; }
        public string[] _heroTags {  set; get; }
        public string[] _unlockHeroTags {  set; get; }
        public Hero(bool beConfig,string name, Sprite image, int blood, bool unlock, string unlockName, Sprite activeSkillImg, Sprite passiveSkillImg,
            Sprite unlockSkillImg, string activeSkillName, string activeSkillProfile, string passiveSkillName, string passiveSkillProfile, string[] heroTags, string[] unlockHeroTags)
        {
            _beConfig = beConfig;
            _name = name;
            _image = image;
            _blood = blood;
            _unlock = unlock;
            _unlockName = unlockName;
            _activeSkillImg = activeSkillImg;
            _passiveSkillImg = passiveSkillImg;
            _unlockSkillImg = unlockSkillImg;
            _activeSkillName = activeSkillName;
            _activeSkillProfile = activeSkillProfile;
            _passiveSkillName = passiveSkillName;
            _passiveSkillProfile = passiveSkillProfile;
            _heroTags = heroTags;
            _unlockHeroTags = unlockHeroTags;
        }
    }

    public class IsConfigHeros
    {
        public Image HeroImg { get; set; }
        public TextMeshProUGUI Probability { get; set; }
        public IsConfigHeros(Image _heroImg, TextMeshProUGUI _probability)
        {
            HeroImg = _heroImg;
            Probability = _probability;
        }
    }

    public void Test()
    {
        Debug.Log("888");
    }

    public static object[] GetIsConfigHero()
    {

        Image tempTex = ABManager.Instance.LoadResource<Image>("spritepreferb", "T_SaiEr");
            // Resources.Load<Image>("Img/T_SaiEr");
        object[] hero0 = new object[2] { tempTex, V1 };
        object[] hero1 = new object[2] { tempTex, V2 };
        object[] hero2 = new object[2] { tempTex, V3 };
        object[] hero3 = new object[2] { tempTex, V4 };

        object[] configHero = new object[4] { hero0, hero1, hero2, hero3 };
        return configHero;
    }

    public static object[] GetAllHero()
    {

        Sprite UltimateTex = ABManager.Instance.LoadResource<Sprite>("spritepreferb", "T_SaiEr");
        // Resources.Load("Img/T_SaiEr", typeof(Sprite)) as Sprite;
        Sprite activeSkillImg = ABManager.Instance.LoadResource<Sprite>("spritepreferb", "SkillIcon_0");
        // Resources.Load("Img/SkillIcon_0", typeof(Sprite)) as Sprite;
        Sprite passiveSkillImg = ABManager.Instance.LoadResource<Sprite>("spritepreferb", "SkillIcon_0");
        // Resources.Load("Img/SkillIcon_0", typeof(Sprite)) as Sprite;
        Sprite unlockSkillImg = ABManager.Instance.LoadResource<Sprite>("spritepreferb", "BeConfigItemBG");
        // Resources.Load("Img/ConfigItemBG", typeof(Sprite)) as Sprite;
        string[] UltimateTag = new string[2] { "Berserker", "Master" };
        string[] WeekTag = new string[2] { "Invasion", "Wise" };
        string[] unlockTag = new string[2] { "???", "???" };

        Hero Ultimate = new Hero(false, "Ultimate", UltimateTex, 2, true, "Magical", activeSkillImg, passiveSkillImg, unlockSkillImg, "Accelerated", "Accelerate attacks on surrounding enemies", "Tenacious", "Can persist for 3 seconds even when the health reaches a point", UltimateTag, unlockTag);
        Hero Week = new Hero(false, "Weel", UltimateTex, 2, true, "ha-ha", activeSkillImg, passiveSkillImg, unlockSkillImg, "Add Blood", "Accelerate attacks on surrounding enemies", "Revitalization", "Can persist for 3 seconds even when the health reaches a point", WeekTag, unlockTag);
        Hero Boom = new Hero(false,"Boom", UltimateTex, 2, true, "Hot and explosive", activeSkillImg, passiveSkillImg, unlockSkillImg, "Explosion", "Accelerate attacks on surrounding enemies", "Energy", "Can persist for 3 seconds even when the health reaches a point", UltimateTag, unlockTag);
        Hero ZhaoZhao = new Hero(false,"Zhao",UltimateTex,2, false, "docile", activeSkillImg, passiveSkillImg, unlockSkillImg, "Collect Souls", "Accelerate attacks on surrounding enemies", "tenacious", "Can persist for 3 seconds even when the health reaches a point", UltimateTag, unlockTag);
        Hero DuoDuo = new Hero(false,"Duo", UltimateTex, 2, false, "niubility", activeSkillImg, passiveSkillImg, unlockSkillImg, "lalal", "Accelerate attacks on surrounding enemies", "tenacious", "Can persist for 3 seconds even when the health reaches a point", UltimateTag, unlockTag);

        object[] heros = new object[5] { Ultimate, Week, Boom, ZhaoZhao, DuoDuo };
        return heros;
    }



    public class Basic
    {
        /// <summary>
        /// ������òͼƬ
        /// ����
        /// �Ա�ö�٣�
        /// ����
        /// ��ǩ
        /// ��ϵ�����ֵ䣩
        /// </summary>
        public Sprite _image;
        public int _age;
        public enum _gender
        {
            Male,
            Famale,
        }
        public string[] _hobby;
        public string[] _label;
        public string _bgStory;
        public Dictionary<string, string> network;
        public Basic(Sprite image, int age, string[] hobby, string[] label, string bgStory, Dictionary<string, string> network)
        {
            _image = image;
            _age = age;
            _hobby = hobby;
            _label = label;
            _bgStory = bgStory;
            this.network = network;
        }
    }

    public class Info
    {
        /// <summary>
        /// Ѫ��
        /// ������
        /// �ٶ�
        /// ����Ƶ��
        /// ������Χ
        /// </summary>
        public int _blood;
        public int _attack;
        public float _speed;
        public float _atkSpeed;
        public float _atkRange;
        public Info(int blood, int attack, float speed, float atkSpeed, float atkRange)
        {
            _blood = blood;
            _attack = attack;
            _speed = speed;
            _atkSpeed = atkSpeed;
            _atkRange = atkRange;
        }
    }

    public class SkillInfo
    {
        /// <summary>
        /// ��������icon
        /// ����������
        /// ����������Ϣ
        /// �������ܵȼ�
        /// ����������ֵ
        /// ��������icon
        /// ����������
        /// ����������Ϣ
        /// �������ܵȼ�
        /// ����������ֵ
        /// </summary>
        public Sprite _activeSkillImg;
        public string _activeSkillName;
        public string _activeSkillInfo;
        public int _activeSkillLevel;
        public float _activeSkillValue;

        public Sprite _passiveSkillImg;
        public string _passiveSkillName;
        public string _passiveSkillInfo;
        public int _passiveSkillLevel;
        public float _passiveSkillValue;
        public SkillInfo(Sprite activeSkillImg, string activeSkillName, string activeSkillInfo, int activeSkillLevel, float activeSkillValue, Sprite passiveSkillImg, string passiveSkillName, string passiveSkillInfo, int passiveSkillLevel, float passiveSkillValue)
        {
            _activeSkillImg = activeSkillImg;
            _activeSkillName = activeSkillName;
            _activeSkillInfo = activeSkillInfo;
            _activeSkillLevel = activeSkillLevel;
            _activeSkillValue = activeSkillValue;
            _passiveSkillImg = passiveSkillImg;
            _passiveSkillName = passiveSkillName;
            _passiveSkillInfo = passiveSkillInfo;
            _passiveSkillLevel = passiveSkillLevel;
            _passiveSkillValue = passiveSkillValue;
        }
    }

    /// <summary>
    /// ��ɫ���ݽṹ
    /// </summary>
    public class MyHero
    {
        /// <summary>
        /// ��ɫID
        /// ��ɫ��
        /// ����״̬
        /// ����״̬
        /// ������Ϣ
        /// ��ֵ��Ϣ
        /// ������Ϣ
        /// </summary>
        public int _heroId;
        public string _heroName;
        public bool _beConfig;
        public bool _unlock;
        public Basic _basic;
        public Basic _lockBasic;
        public Info _info;
        public SkillInfo _skillInfo;
    }
}
