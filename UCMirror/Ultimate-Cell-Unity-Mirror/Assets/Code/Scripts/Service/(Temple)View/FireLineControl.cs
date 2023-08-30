using UnityEngine;

public class FireLineControl : MonoBehaviour
{
    [Header("模拟火线的向左或向右移动: A向左 D向右")]
    public bool FireLineMoveLeft = false;

    public bool FireLineMoveRight = false;

    [Header("模拟敌人的生成操作： S生成")]
    public bool EnemyCreate = false;

    public GameObject[] EnemyInfo; 

    void Update()
    {
    }

    void NewEnemy()
    {
        var randomObj = EnemyInfo[Random.Range(0, EnemyInfo.Length)];

        var newEnemy = Instantiate(randomObj, transform);

        var random = Random.Range((float)0, (float)20);

        newEnemy.transform.position += new Vector3(0, random, 0);
    }
}
