using Mirror;
using UnityEngine;

public class G2C_LoginManager : NetworkBehaviour
{
    [Header("通讯组件")]
    public G2C_CommunicationManager comm;

    public G2C_BroadcastClass broad;

    public void Start()
    {
        NetworkServer.RegisterHandler<StartCheckClass>(StratCheck); // 注册对自定义消息的处理方法
    }

    /// <summary>
    /// 初始化检查版本信息及ip归属地
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="message"></param>
    public void StratCheck(NetworkConnection conn, StartCheckClass message) 
    {
        // TODO 用户登陆偏好存储 异地验证
        Debug.Log(conn.identity + ": " + message.country+ "," + message.city + "," + message.regionName + "," + conn.connectionId);

        if (message.PackageVersion == "v1.5.1") 
        {
            G2C_StartCheckReturn(conn, true);

            // TODO 移送至登陆验证程序
            comm.PlayerAddRoom(conn.connectionId);
            Debug.Log("player add room: " + conn.connectionId);
        }
        else
        {
            G2C_StartCheckReturn(conn, false);
        }
    }

    /// <summary>
    /// 验证成功返回程序
    /// </summary>
    /// <param name="conn"></param>
    void G2C_StartCheckReturn(NetworkConnection conn, bool success)
    {
        StartCheckClass msg = new StartCheckClass() 
        {
            StartCheckBool = success
        };

        conn.Send(msg, conn.connectionId);
    }
}