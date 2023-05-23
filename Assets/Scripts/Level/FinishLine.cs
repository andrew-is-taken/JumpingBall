using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            MovementManager.instance.CrossedFinishLine();
            GetComponent<AudioSource>().Play();
        }
    }
}
