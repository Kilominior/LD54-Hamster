using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampController : MonoBehaviour
{
    public Sprite darkSprite;
    public Sprite lightSprite;

    public bool IsLighted()
    {
        if (GetComponent<SpriteRenderer>().sprite == lightSprite)
            return true;
        return false;
    }
}
