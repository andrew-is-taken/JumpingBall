using UnityEngine;

public class GameCanvasManager : MonoBehaviour
{
    private void Awake()
    {
        int amount = FindObjectsOfType<GameCanvasManager>().Length;

        if (amount == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
