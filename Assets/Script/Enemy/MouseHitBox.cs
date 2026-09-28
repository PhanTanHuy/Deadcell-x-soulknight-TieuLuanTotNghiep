using System.Collections;
using TMPro;
using UnityEngine;

public class MouseHitBox : HitBox
{
    private MouseAI mouseAI;

    protected override void Start()
    {
        base.Start();
        mouseAI = GetComponent<MouseAI>();
    }

    public override void GetDame(int damageAmount)
    {
        base.GetDame(damageAmount);

        GameObject t = PoolObject.instance.SpawnObject(PoolObject.VFXType.DameText, transform, Vector2.one, 1f, true);
        if (t.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI dameText))
        {
            dameText.text = damageAmount.ToString();
        }

        // Trừ máu
        ShakeCam.Instance.Shake(0.2f, 1f);

        if (maxHealth <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                RoomManager.Instance.ClearMouse();
                gameObject.SetActive(false);
            }
        }
    }

  
    public override void KnockBack(Vector2 comingDirection, float force)
    {
        mouseAI.SetTimeCanNotMove(1f);
        base.KnockBack(comingDirection, force);
    }
}