public interface ISoldierRelation
{
    public bool NeedRender { get; set; }
    // 关系开始表现
    void SoldiersStartRelation(Soldier from,Soldier to);
    // 关系持续表现
    void SoldiersUpdateRelation(Soldier from,Soldier to);
    // 关系结束表现
    void SoldiersEndRelation(Soldier from,Soldier to);
}