using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private GameObject[] PreviousPart;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            MovementManager.instance.CrossedFinishLine();
            GetComponent<AudioSource>().Play();
            TurnOffPreviousPart();
        }
    }

    /// <summary>
    /// Turns off the scripts on the previous part of the level to remove unnecessary behaviour.
    /// </summary>
    private void TurnOffPreviousPart()
    {
        foreach (var part in PreviousPart)
        {
            part.GetComponent<ILevelObject>().turnOffObject();
        }
    }
}
