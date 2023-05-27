using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float speed;

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
