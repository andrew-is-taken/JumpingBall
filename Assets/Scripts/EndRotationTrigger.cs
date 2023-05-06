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
        RespawnDirection = new Vector2(Parent.newDirection.x, Parent.newDirection.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Parent.EndOfRotation(transform.position, RespawnDirection);
    }
}
