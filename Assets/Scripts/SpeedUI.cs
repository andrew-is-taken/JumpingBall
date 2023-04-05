using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUI : MonoBehaviour
{
    private Movement Player;

    public GameObject HorizontalSpeed;
    public GameObject VerticalSpeed;

    private void Start()
    {
        Player = FindObjectOfType<Movement>();
        HorizontalSpeed.SetActive(false);
        VerticalSpeed.SetActive(false);
    }

    public void AddSpeed(float additionalSpeed)
    {
        Player.SetSpeed(Player.GetSpeed() + additionalSpeed);
        if (Player.movingHorizontally)
        {
            StartCoroutine(fading(HorizontalSpeed));
        }
        else
        {
            StartCoroutine(fading(VerticalSpeed));
        }
    }

    IEnumerator fading(GameObject obj)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        obj.SetActive(false);
    }
}
