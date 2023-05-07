using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private Movement player;
    private float newMainMovementCoordinate;

    public float newSpeed;
    public bool inverted;
    public Vector3 newDirection;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();
        newMainMovementCoordinate = newDirection.x != 0 ? transform.position.y : transform.position.x;
    }

    public void StartOfRotation()
    {
        if (newSpeed > 0)
            player.SetSpeed(newSpeed);
        player.SetMovementDirection(newDirection, newMainMovementCoordinate, inverted);
    }

    public void EndOfRotation(Vector2 pos, Vector2 addDir)
    {
        player.AddForceInDirection(newDirection);
        player.SetCheckpoint(pos, newDirection, addDir, newSpeed, inverted);
    }
}
