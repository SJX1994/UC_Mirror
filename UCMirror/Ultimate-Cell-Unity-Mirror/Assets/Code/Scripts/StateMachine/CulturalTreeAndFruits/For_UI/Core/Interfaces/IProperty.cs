namespace com.cygnusprojects.TalentTree
{
	public interface IProperty 
	{
        PropertyType PropertyType { get; }
        string Name { get; set; }
        int IntValue { get; set; }
        bool BoolValue { get; set; }
        float FloatValue { get; set; }
        string StringValue { get; set; }
    }
}
