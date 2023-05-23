using System;
using UnityEngine;

[Serializable]
public class PlayerSkin
{
    public Sprite sprite; // player's sprite
    public Gradient color; // gradient of trail
    public float time = 0.2f; // time of trail
    public Color dieColor; // color of death particles
}
