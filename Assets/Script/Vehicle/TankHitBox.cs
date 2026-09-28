using UnityEngine;

public class TankHitBox : HitBox
{
    private Vehicle tank;
    [SerializeField] private GameObject tankDamaged;
    private void Awake()
    {
        tank = GetComponent<Vehicle>();
    }
    public override void GetDame(int damageAmount)
    {
        base.GetDame(damageAmount);
        Debug.Log($"Tank took {damageAmount} damage. Remaining health: {maxHealth}");
        if (CanNotLifeAnymore())
        {
            tank.ExitVehicle();
            foreach (Collider2D box in GetComponentsInChildren<Collider2D>())
            {
                box.enabled = false;
            }
            tankDamaged.SetActive(true);
            tank.DisableInteract();
        }
    }
}
