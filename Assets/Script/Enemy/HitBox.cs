using UnityEngine;
using System.Collections;


public class HitBox : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] protected int maxHealth = 3;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float hitFlashDuration = 0.15f;
    [SerializeField] private Color hitColor = Color.red;

    private Coroutine hitFlashCoroutine;
    protected bool isDead = false;
    //private MouseAI mouseAI;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //mouseAI = GetComponent<MouseAI>();
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("AttackBox"))
    //    {
    //        //GetDame();
    //    }
    //}
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public virtual void GetDame(int damageAmount)
    {
        if (maxHealth <= 0)
        {
            maxHealth = 0;
            return;
        }
        // Flash đỏ
        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine = StartCoroutine(HitFlash());
        maxHealth -= damageAmount;
        //gameObject.SetActive(false);
    }
    public bool CanNotLifeAnymore()
    {
        return maxHealth <= 0;
    }
    private IEnumerator HitFlash()
    {
        Color originalColor = Color.white;

        // Đổi sang đỏ ngay lập tức
        spriteRenderer.color = hitColor;

        float timer = 0f;

        while (timer < hitFlashDuration)
        {
            timer += Time.deltaTime;

            float t = timer / hitFlashDuration;

            // Đỏ -> màu gốc
            spriteRenderer.color = Color.Lerp(
                hitColor,
                originalColor,
                t
            );

            yield return null;
        }

        // Đảm bảo trở về màu gốc
        spriteRenderer.color = originalColor;

        hitFlashCoroutine = null;
    }
    public virtual void KnockBack(Vector2 comingDirection, float force)
    {
        //mouseAI.SetTimeCanNotMove(1f);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(comingDirection * force, ForceMode2D.Impulse);
    }
}
