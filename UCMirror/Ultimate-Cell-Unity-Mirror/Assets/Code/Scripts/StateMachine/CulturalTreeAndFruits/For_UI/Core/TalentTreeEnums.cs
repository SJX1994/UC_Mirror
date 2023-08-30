using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.cygnusprojects.TalentTree
{
	public enum NodeType
    {
        Talent,
    }

    public enum PropertyType
    {
        Int,
        Float,
        Bool,
        String,
    }

    public enum ConditionType
    {
        HasLevel,
        MaxedOut,
        TierCompleted,
    }

    public enum ConnectionType
    {
        Required,
        Optional
    }
}
