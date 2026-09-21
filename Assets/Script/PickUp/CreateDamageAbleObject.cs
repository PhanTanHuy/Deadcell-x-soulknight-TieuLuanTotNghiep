using UnityEngine;

public class CreateDamageAbleObject : Item
{
    public PoolObject.VFXType vfxType;
    public override void Pick(GameObject player)
    {
        PoolObject.instance.SpawnDamageAbleObject(vfxType, player.transform);
    }
}
