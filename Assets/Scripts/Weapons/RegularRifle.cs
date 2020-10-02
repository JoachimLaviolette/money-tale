using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RegularRifle: Rifle
{
    override protected void Start()
    {
        m_name = "Regular Rifle"; 
        base.Start();
    }

    override public void Fire(
        Transform shooterTransform,
        Vector3 bulletFirePosition,
        Quaternion bulletFireRotation,
        Vector3 bulletPosition,
        Quaternion bulletRotation,
        Vector3 bulletDirection,
        LayerMask damageableLayerMask,
        UnityAction setShootingDone = null)
    {
        StartCoroutine(FireBullet(
            shooterTransform,
            bulletFirePosition,
            bulletFireRotation,
            bulletPosition,
            bulletRotation,
            bulletDirection,
            damageableLayerMask,
            setShootingDone));
    }

    private IEnumerator FireBullet(
        Transform shooterTransform, 
        Vector3 bulletFirePosition,
        Quaternion bulletFireRotation,
        Vector3 bulletPosition,
        Quaternion bulletRotation,
        Vector3 bulletDirection,
        LayerMask damageableLayerMask,
        UnityAction setShootingDone = null)
    {
        m_weaponAmmo--;
        Transform bulletFire = Instantiate(AssetManager.BulletFire(), bulletFirePosition, bulletFireRotation);
        Bullet bullet = Instantiate(AssetManager.RegularBullet(), bulletPosition, bulletRotation);
        bullet.Setup(false, bulletDirection, m_bulletSpeed, m_damages, m_damageType, shooterTransform, damageableLayerMask);
        yield return new WaitForSeconds(m_fireRate);
        Destroy(bulletFire.gameObject);
        yield return new WaitForSeconds(m_forcedFireRate);
        setShootingDone?.Invoke();
    }
}
