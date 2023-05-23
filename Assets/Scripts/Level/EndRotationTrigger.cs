using UnityEngine;

public class EndRotationTrigger : MonoBehaviour
{
    private Rotation Parent; // main rotation element

    private void Start()
    {
        Parent = GetComponentInParent<Rotation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Parent.EndOfRotation(transform.position);
    }
}
