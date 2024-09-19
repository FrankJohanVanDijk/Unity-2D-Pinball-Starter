using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverLifetime : MonoBehaviour
{

    public float lifeTime;

    public bool startCounter = true;

    // Update is called once per frame
    void Update()
    {
        if(startCounter)
        {
            Destroy(gameObject, lifeTime);
        }
    }
}
