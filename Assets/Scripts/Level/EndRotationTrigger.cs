using UnityEngine;

public class EndRotationTrigger : MonoBehaviour
{
    private Rotation Parent; // main rotation element
    private Vector2 RespawnDirection; // direction to respawn the player

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
