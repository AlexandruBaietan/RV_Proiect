using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pins : MonoBehaviour
{
    // Start is called before the first frame update


    //the reference is used to know if the pin has fallen or not (it is on the tip of the gameobject)
    public Transform _reference;

    //this is to know if the pins has fallen
    Vector3 originalRefVector;

    //original position of the gameobject and its local rotation
    Quaternion originalLocalRotation;
    Vector3 originalLocalPosition;

    public Collider[] cols;

    // know if has fallen
    public bool fallen =false;
    public bool hasFallenOnce = false;

    void OnEnable()
    {
        //initializase initial positions and rotations
        originalLocalPosition = transform.localPosition;
        originalRefVector=_reference.position- transform.position;
        
        originalLocalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //check if falling
        Vector3 vnew = _reference.position - transform.position;

        if ( Vector3.Angle(vnew,originalRefVector)  >35)
        {
            fallen = true;
            
            if (hasFallenOnce == false)
            {
                Invoke("hidePin", 2);

            }
            hasFallenOnce = true;

        }
        else
        {
            fallen = false;

        }

       
       

    }


    IEnumerator resetposCorr()
    {
        CancelInvoke();
        showPin();

        //disable physics to reposition the pins
        for (int ii=0;ii<cols.Length;ii++)
        {
            cols[ii].enabled = false;
            
        }

        GetComponent<Rigidbody>().useGravity = false;
        
        float elapsed = 0;

        //corrutine main actions
        while (elapsed<3.5f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPosition, 0.1f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalLocalRotation, 0.1f);
            
            yield return new WaitForFixedUpdate();

            showPin();

            elapsed += Time.fixedDeltaTime;
        }

        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;


        //re-stablish physics
        for (int ii = 0; ii < cols.Length; ii++)
        {
            cols[ii].enabled = true;
        }

        GetComponent<Rigidbody>().useGravity =true;

        hasFallenOnce = false;
    }


    public void resetPos()
    {
        StartCoroutine(resetposCorr());
    }

    public void hidePin()
    {

        //disable physics to reposition the pins
        for (int ii = 0; ii < cols.Length; ii++)
        {
            cols[ii].enabled = false;

        }

        //hide renderer
        GetComponent<MeshRenderer>().enabled = false;

        //disable gravity
        GetComponent<Rigidbody>().useGravity = false;
    }


    public void showPin()
    {

        //show renderer
        GetComponent<MeshRenderer>().enabled = true;

    }

}
