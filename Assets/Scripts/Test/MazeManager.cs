using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    public Transform rotation;
    public Transform cam;

    private void FixedUpdate()
    {
        cam.rotation = rotation.rotation;
        Physics2D.gravity = -rotation.up.normalized * 9.81f;
    }
}
