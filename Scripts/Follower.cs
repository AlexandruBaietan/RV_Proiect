using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform t;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(t!=null)
        {
            transform.position = Vector3.Lerp(transform.position,t.position,0.2f);
        }
    }
}
