using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private Movement player;

    public float newSpeed;
    public float newMainMovementCoordinate;
    public Vector3 newDirection;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();
    }

    public void StartOfRotation()
    {
        if (newSpeed > 0)
            player.SetSpeed(newSpeed);
        player.SetMovementDirection(newDirection, newMainMovementCoordinate);
    }

    public void EndOfRotation()
    {
        player.AddForceInDirection(newDirection);
    }
}
