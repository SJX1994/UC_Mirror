using UnityEngine;

public class SinglePlayerGamesManager : MonoBehaviour
{
    private void Start()
    {
        var lanNetWork = GameObject.Find("LanNetWorkManager");

        if (lanNetWork == null)
        {
            this.gameObject.name = "LanNetWorkManager";
        }
    }
}