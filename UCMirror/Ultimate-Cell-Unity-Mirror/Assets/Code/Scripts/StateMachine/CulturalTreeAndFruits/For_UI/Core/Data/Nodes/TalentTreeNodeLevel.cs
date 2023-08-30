using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.cygnusprojects.TalentTree
{
    public class TalentTreeNodeLevel 
    {
        #region Variables
        public int Level;
        public int Cost;
        #endregion

        #region Implementation
        public TalentTreeNodeLevel(int level, int cost)
        {
            Level = level;
            Cost = cost;
        }
        #endregion

    }
}
