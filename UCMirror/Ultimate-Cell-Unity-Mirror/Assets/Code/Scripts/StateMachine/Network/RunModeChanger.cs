using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
public class RunModeChanger : Singleton<RunModeChanger>
{
    public RunMode runMode;
    // Start is called before the first frame update
    void Awake()
    {
        RunModeData.ChangeRunMode(runMode);
        UserAction.Player1UserState = UserAction.State.WatchingFight;
        UserAction.Player2UserState = UserAction.State.WatchingFight;
    }
}
