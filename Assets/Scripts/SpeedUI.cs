using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUI : MonoBehaviour
{
    public GameObject HorizontalSpeed; // horizontal speed effect
    public GameObject VerticalSpeed; // vertical speed effect

    private void Start()
    {
        HorizontalSpeed.SetActive(false);
        VerticalSpeed.SetActive(false);
    }

    /// <summary>
    /// Sets the player's speed to new value.
    /// </summary>
    /// <param name="additionalSpeed"></param>
    public void AddSpeed(float additionalSpeed)
    {
        MovementManager Player = MovementManager.instance;
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

    /// <summary>
    /// Disables the effect after it is used.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    IEnumerator fading(GameObject obj)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        obj.SetActive(false);
    }
}
