using UnityEngine;
using UnityEngine.UI;

public class DisplayLevelNumber : MonoBehaviour
{
    void Start()
    {
        GetComponentInChildren<Text>().text = name;
    }
}
