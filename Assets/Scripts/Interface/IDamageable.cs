using UnityEngine;

public enum DamageType
{
    Regular, Hard,
}
public interface IDamageable
{
    void Damage(float amount, Transform damagerTransform, DamageType damageType);
}