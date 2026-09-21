using UnityEngine;
public enum BulletType
{
    Explosion,
    Electric,
    Fire,
    Ice,
}
public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.SetActive(false);
    }
}
