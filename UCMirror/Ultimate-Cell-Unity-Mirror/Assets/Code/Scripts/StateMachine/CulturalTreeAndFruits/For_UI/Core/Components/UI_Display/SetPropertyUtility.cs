using UnityEngine;
/// <summary>
/// Tool script taken from the UI source as it's set to Internal for some reason. So to use in the extensions project it is needed here also.
/// 工具脚本取自UI源，因为它由于某种原因设置为内部。因此，在扩展项目中使用它也是必需的。
/// </summary>
/// 
namespace com.cygnusprojects.TalentTree
{
    internal static class SetPropertyUtility
    {
        public static bool SetClass<T>(ref T currentValue, T newValue) where T: class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }
    }
}
