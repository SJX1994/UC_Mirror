using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginMainSceneManager : MonoBehaviour
{
    #region 交互组件
    [Header("页面切换组件")]
    // 登陆页面
    public GameObject loginPage;

    // 新建账号页面
    public GameObject CreateAccountPage;

    // 用户协议页面
    public GameObject AgreementPage;

    // 检查版本页面
    public GameObject StartCheckPage;

    public GameObject MiddlePage;

    public GameObject ButtomPage;

    [Header("输入组件")]
    // 登陆页面用户账号输入
    public TextMeshProUGUI AccountInput;
    
    // 登陆页面用户密码输入
    public TextMeshProUGUI PasswordInput;

    // 新建账号界面用户账号输入
    public TextMeshProUGUI AccountCreate;

    // 新建账号界面用户密码输入
    public TextMeshProUGUI PasswordCreate;

    // 新建账号界面用户密码确认
    public TextMeshProUGUI PasswordCreateReset;

    [Header("按钮组件")]
    // 连接按钮
    public Button StartButton;

    // 登陆按钮
    public Button LoginButton;

    // 新建账号界面切换按钮
    public Button CreateAccountButton;

    // 记住我下次不用敲账号和密码
    public Toggle RemberMe;

    // 同意用户协议
    public Toggle IAgree;

    // 查看用户协议按钮
    public Button AgreementDetailButton;

    // 同意用户协议按钮
    public Button AgreementAgreeButton;

    // 退出用户协议按钮
    public Button AgreementBackButton;

    [Header("交互组件")]
    // 用户输入不正确的用户名图片颜色变为红色（#F18E8B）/蓝色（#8BE6F1）
    public Image AccountPass;

    // 用户输入不正确的密码图片颜色变为红色
    public Image PasswordPass;

    // 用户输入不正确的密码图片颜色变为红色
    public Image PasswordResetPass;

    // 初始化检查信息文字组件
    public TextMeshProUGUI StartCheckInfo;

    [Header("页面切换组件")]
    public GameObject SceneLoad;

    // 通讯组件
    private BroadcastClass broadcastClass;

    private CommunicationInteractionManager CommunicationManager;

    public GameObject lanNetWork;
    #endregion

    #region 生命周期

    private void Start()
    {
        if (!lanNetWork.activeSelf)
        {
            lanNetWork.SetActive(true);
        }

        // 进入微观世界按钮点击
        StartButton.onClick.AddListener(() => { StartGame(); });

        // 登陆按钮点击函数
        LoginButton.onClick.AddListener(() => { LoginButtonClick(); });

        // 通信获取
        // TODO 暂时获取方式
        var obj = GameObject.Find("LanNetWorkManager").gameObject;

        CommunicationManager = obj.GetComponent<CommunicationInteractionManager>();

        broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        broadcastClass.StratCheckReturn += StartCheckReturn;

        broadcastClass.ServerStoped += ServerStoped;

    }
    #endregion

    #region 逻辑方法
    void LoginButtonClick() 
    {
        var sceneload = SceneLoad.GetComponent<MainSceneControlManager>();

        sceneload.LoadMainBasicScene();
    }
    #endregion

    #region 监听事件
    void StartCheckReturn(int info) 
    {
        if (info == 1)
        {
            Debug.Log("Version Check Success");

            StartCheckPage.SetActive(false);

            MiddlePage.SetActive(true);

            ButtomPage.SetActive(true);
        }
        else if (info == 0) 
        {
            StartCheckInfo.text = "Please download the latest version of the client";
        }
    }

    /// <summary>
    /// 服务器未正确连接
    /// </summary>
    /// <param name="info"></param>
    void ServerStoped(int info) 
    {
        StartCheckInfo.text = "服务器未正确连接";
    }

    /// <summary>
    /// 开始游戏按钮点击
    /// </summary>
    void StartGame() 
    {
        CommunicationManager.StartGame(0);
    }
    #endregion
}