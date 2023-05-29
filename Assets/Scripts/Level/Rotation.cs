using System.Collections;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private MovementManager player; // player
    [SerializeField] private GameObject[] FollowingPart;

    [Header("New parameters")]
    public float newSpeed; // new speed for player's movement
    public Vector3 newDirection; // new direction for player's movement
    [HideInInspector] public float newMainMovementCoordinate; // new coordinate for player's movement

    [Header("Rotation parameters")]
    [SerializeField] private bool inverted; // if the rotation is clockwise or counter-clockwise
    [SerializeField] private bool hasCheckpoint;

    private GameObject CheckpointAnim; // animator when checkpoint is set

    private void Start()
    {
        player = MovementManager.instance;
        CheckpointAnim = GetComponentInChildren<Animator>().gameObject;
        CheckpointAnim.SetActive(hasCheckpoint);
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
    public void EndOfRotation(Vector3 pos)
    {
        TurnOnFollowingPart();
        player.AddForceInDirection(newDirection);
        if (hasCheckpoint)
        {
            StartCoroutine(setCheckpoint());
            player.SetCheckpoint(this, player.transform.position, newDirection, player.Movement.additionalDirection, newSpeed);
        }
    }

    /// <summary>
    /// Turns on tthe scripts on the following part of the level to sync with player's position.
    /// </summary>
    public void TurnOnFollowingPart()
    {
        foreach(var part in FollowingPart)
        {
            part.GetComponent<ILevelObject>().restartObject();
        }
    }

    /// <summary>
    /// Visual animation of placing a checkpoint.
    /// </summary>
    /// <returns></returns>
    IEnumerator setCheckpoint()
    {
        yield return new WaitForSeconds(.75f);
        CheckpointAnim.SetActive(false);
    }
}
