using System.Collections.Generic;
using UnityEngine;

namespace com.cygnusprojects.TalentTree
{
	public interface ITier
	{
        string Name { get; set; }
        Color EditorColor { get; set; }
        string Description { get; set; }
        bool IsMaxedOut { get; }
        /*List<Property> Properties { get;  }

        Property GetProperty(string Name);
        void SetProperty(Property Property);*/
    }
}
