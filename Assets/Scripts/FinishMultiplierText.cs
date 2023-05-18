using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishMultiplierText : MonoBehaviour
{
    public float xOffset; // offset where the text will be placed after rotation

    void Start()
    {
        if(transform.up != Vector3.up)
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            print(transform.localRotation.eulerAngles.z);
            if (transform.localRotation.eulerAngles.z == 180 || transform.localRotation.eulerAngles.z < 0.1f) // if finish line is horizontal
                transform.localPosition = new Vector3(xOffset, 0, 0);
        }
    }
}
