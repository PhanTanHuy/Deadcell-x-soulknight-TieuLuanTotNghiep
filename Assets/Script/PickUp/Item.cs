using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public Sprite ItemSprite;
    public abstract void Pick(GameObject player);
}
