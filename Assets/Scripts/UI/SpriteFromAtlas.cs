using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SpriteFromAtlas : MonoBehaviour
{
    [SerializeField] private SpriteAtlas atlas; // atlas with sprites
    [SerializeField] private string spriteName; // name of image

    void Start()
    {
        GetComponent<Image>().sprite = atlas.GetSprite(spriteName);
    }
}
