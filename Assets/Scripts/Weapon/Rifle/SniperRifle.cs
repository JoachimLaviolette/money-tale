using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SniperRifle: Rifle
{
    override protected void Start()
    {
        m_name = "Sniper Rifle";
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
        StartCoroutine(FireLaser(
            shooterTransform,
            bulletFirePosition,
            bulletFireRotation,
            bulletPosition,
            bulletRotation,
            bulletDirection,
            damageableLayerMask,
            setShootingDone));
    }

    private IEnumerator FireLaser(
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
        Bullet bullet = Instantiate(AssetManager.SniperBullet(), bulletPosition, bulletRotation);
        bullet.Setup(true, bulletDirection, m_bulletSpeed, m_damages, m_damageType, shooterTransform, damageableLayerMask, false);
        yield return new WaitForSeconds(m_fireRate);
        Destroy(bulletFire.gameObject);
        Destroy(bullet.gameObject);
        yield return new WaitForSeconds(m_forcedFireRate);
        setShootingDone?.Invoke();
    }
}
