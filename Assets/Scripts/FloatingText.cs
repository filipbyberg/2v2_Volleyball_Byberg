using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is made to destroy floating text animation over agent
public class FloatingText : MonoBehaviour
{
    public float DestroyTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }

}
