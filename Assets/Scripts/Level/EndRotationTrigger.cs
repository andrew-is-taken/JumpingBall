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
        if (collision.transform.tag == "Player")
        {
            GetComponent<BoxCollider2D>().enabled = false;
            Parent.EndOfRotation(transform.position);
        }
        else
            collision.gameObject.SetActive(false);
    }
}
