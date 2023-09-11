using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using Mirror;
public struct IdelHolderCreatMessage : NetworkMessage
{
        public Player player;
        public string name;
}
