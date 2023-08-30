using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.cygnusprojects.TalentTree
{
    public class TalentTreeConnectionStatus
    {
        public TalentTreeConnectionBase Connection;
        public bool IsActive
        {
            get
            {
                if (Connection != null)
                {
                    if (Connection.fromNode != null && Connection.toNode != null)
                    {
                        int fromLevel = Connection.fromNode.GetLevel(false);
                        int toLevel = Connection.toNode.GetLevel(false);
                        return (fromLevel > 0) && (toLevel > 0);
                    }
                }
                return false;
            }
        }
    }
}
