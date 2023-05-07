using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private Movement player; // player

    private float newMainMovementCoordinate; // new coordinate for player's movement
    public float newSpeed; // new speed for player's movement
    public Vector3 newDirection; // new direction for player's movement

    public bool inverted; // if the rotation is clockwise or counter-clockwise

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();
        newMainMovementCoordinate = newDirection.x != 0 ? transform.position.y : transform.position.x;
    }

    /// <summary>
    /// Starts the rotation process, sets the direction and speed.
    /// </summary>
    public void StartOfRotation()
    {
        if (newSpeed > 0)
            player.SetSpeed(newSpeed);
        player.SetMovementDirection(newDirection, newMainMovementCoordinate, inverted);
    }

    /// <summary>
    /// Ends the rotation process, sets the checkpoint and pushes player in direction.
    /// </summary>
    public void EndOfRotation(Vector2 pos, Vector2 addDir)
    {
        player.AddForceInDirection(newDirection);
        player.SetCheckpoint(pos, newDirection, addDir, newSpeed);
    }
}
