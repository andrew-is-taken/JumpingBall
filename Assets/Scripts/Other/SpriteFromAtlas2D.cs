using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SpriteFromAtlas2D : MonoBehaviour
{
    [SerializeField] private SpriteAtlas atlas; // atlas with sprites
    [SerializeField] private string spriteName; // name of image

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = atlas.GetSprite(spriteName);
    }
}
