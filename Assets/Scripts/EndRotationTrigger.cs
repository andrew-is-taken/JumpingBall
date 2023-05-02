using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRotationTrigger : MonoBehaviour
{
    private Rotation Parent;

    public Vector2 RespawnDirection;

    private void Start()
    {
        Parent = GetComponentInParent<Rotation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Parent.EndOfRotation(transform.position, RespawnDirection);
    }
}
