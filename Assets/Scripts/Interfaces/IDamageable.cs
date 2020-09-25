using UnityEngine;

public interface IDamageable
{
    void Damage(float amount, Transform damagerTransform);
}