using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRotationTrigger : MonoBehaviour
{
    private Rotation Parent;

    private void Start()
    {
        Parent = GetComponentInParent<Rotation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Parent.StartOfRotation();
    }
}
