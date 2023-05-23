using UnityEngine;

public class StartRotationTrigger : MonoBehaviour
{
    private Rotation Parent; // main rotation element

    private void Start()
    {
        Parent = GetComponentInParent<Rotation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Parent.StartOfRotation();
    }
}
