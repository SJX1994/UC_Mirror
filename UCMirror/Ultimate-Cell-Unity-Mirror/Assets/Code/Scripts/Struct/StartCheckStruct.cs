
using Mirror;

public struct StartCheckClass : NetworkMessage
{
    // 版本号
    public string PackageVersion;

    // 国家
    public string country;

    // 地区
    public string regionName;

    // 城市
    public string city;

    // 验证结果返回值
    public bool StartCheckBool;
}