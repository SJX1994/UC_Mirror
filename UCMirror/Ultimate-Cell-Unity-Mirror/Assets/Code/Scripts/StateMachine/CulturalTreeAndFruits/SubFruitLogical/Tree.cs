using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public List<Sprite> randomDispaly;
    SpriteRenderer treeSpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        treeSpriteRenderer = transform.Find("TreeDisplay").GetComponent<SpriteRenderer>();
        treeSpriteRenderer.sprite = randomDispaly[Random.Range(0, randomDispaly.Count)];
    }
}
