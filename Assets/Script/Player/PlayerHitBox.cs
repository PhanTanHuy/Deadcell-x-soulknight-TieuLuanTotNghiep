using UnityEngine;

public class PlayerHitBox : HitBox
{
    public override void GetDame(int damageAmount)
    {
        base.GetDame(damageAmount);
        Debug.Log($"Player took {damageAmount} damage. Remaining health: {maxHealth}");
        if (CanNotLifeAnymore())
        {
            // Xử lý khi nhân vật chết
            Debug.Log("Player is dead!");
            // Thêm các hành động khác khi nhân vật chết, ví dụ: hiển thị màn hình Game Over, reset level, v.v.
        }
    }
}
