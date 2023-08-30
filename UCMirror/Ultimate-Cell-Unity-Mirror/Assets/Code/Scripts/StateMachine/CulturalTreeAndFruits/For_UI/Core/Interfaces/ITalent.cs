using System.Collections.Generic;

namespace com.cygnusprojects.TalentTree
{
	public interface ITalent  
	{
        string Name { get; set; }
        string Description { get; set; }
        Tier Tier { get; set; }
        bool IsEnabled { get; }
        int MaxLevel { get; }
        int Level { get; }
        string Explanation { get; set; }
        List<TalentTreeCost> Cost { get; set; }

        void Buy();
        void Apply();
        void Revert();
        TalentTreeNodeNextLevel GetCostForNextLevel();
        /*List<Property> Properties { get; set; }

        Property GetProperty(string Name);
        void SetProperty(Property Property);*/
    }
}
