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
        Vector3 bulletFirePosition,
        Quaternion bulletFireRotation,
        Vector3 bulletPosition,
        Quaternion bulletRotation,
        Vector3 bulletDirection,
        LayerMask damageableLayer,
        UnityAction setShootingDone)
    {
        StartCoroutine(FireLaser(
            bulletFirePosition,
            bulletFireRotation,
            bulletPosition,
            bulletRotation,
            bulletDirection,
            damageableLayer,
            setShootingDone));
    }

    private IEnumerator FireLaser(
        Vector3 bulletFirePosition,
        Quaternion bulletFireRotation,
        Vector3 bulletPosition,
        Quaternion bulletRotation,
        Vector3 bulletDirection,
        LayerMask damageableLayer,
        UnityAction setShootingDone)
    {
        m_weaponAmmo--;
        Transform bulletFire = Instantiate(AssetManager.BulletFire(), bulletFirePosition, bulletFireRotation);
        Bullet bullet = Instantiate(AssetManager.SniperBullet(), bulletPosition, bulletRotation);
        bullet.Setup(true, bulletDirection, m_bulletSpeed, m_damages, damageableLayer, false);
        yield return new WaitForSeconds(0.5f);
        Destroy(bulletFire.gameObject);
        Destroy(bullet.gameObject);
        setShootingDone.Invoke();
    }
}
