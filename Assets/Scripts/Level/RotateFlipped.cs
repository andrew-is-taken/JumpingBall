using UnityEngine;

public class CheckpointText : MonoBehaviour
{
    void Start()
    {
        if (transform.up == -Vector3.up)
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        }
    }  
}
