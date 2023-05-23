using UnityEngine;

public class FinishMultiplierText : MonoBehaviour
{
    [SerializeField] private float xOffset; // offset where the text will be placed after rotation

    void Start()
    {
        if(transform.up != Vector3.up)
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            if (transform.localRotation.eulerAngles.z == 180 || transform.localRotation.eulerAngles.z < 0.1f) // if finish line is horizontal
                transform.localPosition = new Vector3(xOffset, 0, 0);
        }
    }
}
