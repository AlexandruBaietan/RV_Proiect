using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustToCameraDistance : MonoBehaviour
{
    // Start is called before the first frame update

    
    void Start()
    {
        
        Vector3 dir =transform.position- (Camera.main.transform.position);

        transform.localScale = new Vector3(transform.localScale.x,dir.magnitude*1.5f,transform.localScale.z);

        transform.localPosition = transform.localPosition - dir.magnitude/2*new Vector3(0,0,1);
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
