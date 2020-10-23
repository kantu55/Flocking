using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMove : MonoBehaviour
{
    Vector3[] pos;
    int index = 0;
    float changeTimer = 5;

    void Start()
    {
        pos = new Vector3[] 
        {
            new Vector3(-7, 0.5f, -7),
            new Vector3(7, 0.5f, -7),
            new Vector3(-7, 0.5f, 7),
            new Vector3(7, 0.5f, 7)
        };
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = pos[index];
        changeTimer -= Time.deltaTime;
        if(changeTimer <= 0)
        {
            index++;
            if (index > 3)
            {
                index = 0;
            }
            changeTimer = 5;
        }
    }
}
