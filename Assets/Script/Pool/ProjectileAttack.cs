using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{
    public List<GameObject> projectilePrefabs = new();
    //public Transform shootPos;
    public Transform attacker;
    public SpriteRenderer spriteWeapon;
    private string currnetNameProjectile;
    private GameObject projectileHold;
    private List<Queue<ProjectileMovement>> projectilePools = new();
    [SerializeField] private int damageAmount = 1;
    [Header("Multi Shot")]
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private float bulletAngleStep = 10f;
    public int DamageAmount => damageAmount;
    public bool IsProjectileActive => projectileHold != null;
    protected virtual void Start()
    {
        InitializePools();
    }

    protected virtual void InitializePools()
    {
        for (int i = 0; i < projectilePrefabs.Count; i++)
        {
            Queue<ProjectileMovement> pool = new();

            for (int j = 0; j < 30; j++)
            {
                GameObject obj = Instantiate(projectilePrefabs[i]);
                obj.SetActive(false);

                //obj.GetComponent<AttackNode>().attackerBasePower =
                //    attacker.GetComponent<AttackNode>().attackerBasePower;

                ProjectileMovement p = obj.GetComponent<ProjectileMovement>();
                p.projectileAttack = this;
                pool.Enqueue(p);
            }

            projectilePools.Add(pool);
        }
        currnetNameProjectile = projectilePools[0].Peek().gameObject.name;
    }

    public void ChangeProjectilePool(int poolIndex, GameObject newProjectilePrefab, int size, int bulletPerShott)
    {
        if (poolIndex < 0 || poolIndex >= projectilePools.Count)
            return;
        if (projectileHold != null)
        {
            projectileHold.SetActive(false);
            Destroy(projectileHold.gameObject);
            projectileHold = null;
        }
        Queue<ProjectileMovement> pool = projectilePools[poolIndex];

        while (pool.Count > 0)
        {
            ProjectileMovement proj = pool.Dequeue();

            if (proj != null)
                Destroy(proj.gameObject);
        }

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(newProjectilePrefab);
            obj.SetActive(false);

            //obj.GetComponent<AttackNode>().attackerBasePower =
            //    attacker.GetComponent<AttackNode>().attackerBasePower;
            ProjectileMovement p = obj.GetComponent<ProjectileMovement>();
            p.projectileAttack = this;
            pool.Enqueue(p);
        }
        bulletsPerShot = bulletPerShott;
        projectilePrefabs[poolIndex] = newProjectilePrefab;
        currnetNameProjectile = projectilePools[0].Peek().gameObject.name;
    }
    public void ActiveProjectHold(int projectileIndex, Vector2 direction, Transform shootPos)
    {
        if (projectileIndex < 0 || projectileIndex >= projectilePools.Count)
            return;

        Queue<ProjectileMovement> pool = projectilePools[projectileIndex];
        if (pool.Count == 0)
            return;

        ProjectileMovement proj = pool.Dequeue();
        proj.InitializeProjectile(
            ProjectileMovement.MoveMode.StraightDirection,
            shootPos.position,
            null,
            direction);
        proj.SetShootPos(shootPos);
        proj.gameObject.SetActive(true);
        projectileHold = proj.gameObject;
    }
    public void DeactiveProjectHold(int projectileIndex)
    {
        if (projectileIndex < 0 || projectileIndex >= projectilePools.Count)
            return;
        Queue<ProjectileMovement> pool = projectilePools[projectileIndex];
        if (projectileHold == null)
            return;
        projectileHold.SetActive(false);
        pool.Enqueue(projectileHold.GetComponent<ProjectileMovement>());
        projectileHold = null;
    }
    public bool ShootProjectile(int projectileIndex, Vector2 direction, Transform shootPos)
    {
        if (projectileIndex < 0 ||
            projectileIndex >= projectilePools.Count)
            return false;

        Queue<ProjectileMovement> pool =
            projectilePools[projectileIndex];

        if (pool.Count == 0)
            return false;

        int bulletCount = Mathf.Max(1, bulletsPerShot);

        for (int i = 0; i < bulletCount; i++)
        {
            if (pool.Count == 0)
                break;

            ProjectileMovement proj = pool.Dequeue();

            float angleOffset =
                (i - (bulletCount - 1) / 2f)
                * bulletAngleStep;

            Vector2 bulletDirection =
                RotateDirection(direction, angleOffset);

            proj.InitializeProjectile(
                ProjectileMovement.MoveMode.StraightDirection,
                shootPos.position,
                null,
                bulletDirection
            );

            StartCoroutine(
                ReturnToPoolAfterDelay(
                    proj,
                    pool,
                    2f
                )
            );

            proj.gameObject.SetActive(true);
        }
        return true;
    }
    private Vector2 RotateDirection(
    Vector2 direction,
    float angle)
    {
        return (
            Quaternion.Euler(
                0f,
                0f,
                angle
            ) * direction
        ).normalized;
    }
    protected IEnumerator ReturnToPoolAfterDelay(
        ProjectileMovement proj,
        Queue<ProjectileMovement> pool,
        float delay)
    {
        yield return new WaitForSeconds(delay);
        if (proj.gameObject.name != currnetNameProjectile)
        {
            Destroy(proj.gameObject);
            yield break;
        }
        proj.gameObject.SetActive(false);
        pool.Enqueue(proj);
    }
    public void ChangeProjectileScale(int poolIndex, float percent)
    {
        float multiplier = 1f + percent / 100f;

        Vector3 newScale = new Vector3(
            multiplier,
            multiplier,
            multiplier
        );
        if (poolIndex < 0 || poolIndex >= projectilePools.Count)
            return;

        Queue<ProjectileMovement> pool = projectilePools[poolIndex];

        foreach (ProjectileMovement projectile in pool)
        {
            if (projectile == null)
                continue;

            projectile.transform.localScale = newScale;
        }
    }
    
}