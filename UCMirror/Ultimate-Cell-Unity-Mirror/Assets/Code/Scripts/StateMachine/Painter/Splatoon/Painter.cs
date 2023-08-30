using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{
    public ParticleSystem inkParticle;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Shoot", 0, 3.5f);
    }
    void Shoot()
    {
        inkParticle.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
