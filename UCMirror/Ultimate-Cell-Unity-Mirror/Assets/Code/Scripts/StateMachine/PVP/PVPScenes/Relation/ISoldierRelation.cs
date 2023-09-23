public interface ISoldierRelation
{
    public bool NeedRender { get; set; }
    // 关系开始表现
    void SoldiersStartRelation(SoldierBehaviors from,SoldierBehaviors to);
    // 关系持续表现
    void SoldiersUpdateRelation(SoldierBehaviors from,SoldierBehaviors to);
    // 关系结束表现
    void SoldiersEndRelation(SoldierBehaviors from,SoldierBehaviors to);
}