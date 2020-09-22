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
        Vector3 bulletFirePosition,
        Quaternion bulletFireRotation,
        Vector3 bulletPosition,
        Quaternion bulletRotation,
        Vector3 bulletDirection,
        LayerMask damageableLayer,
        UnityAction setShootingDone)
    {
        StartCoroutine(FireBullet(
            bulletFirePosition,
            bulletFireRotation,
            bulletPosition,
            bulletRotation,
            bulletDirection,
            damageableLayer,
            setShootingDone));
    }

    private IEnumerator FireBullet(
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
        Bullet bullet = Instantiate(AssetManager.RegularBullet(), bulletPosition, bulletRotation);
        bullet.Setup(false, bulletDirection, m_bulletSpeed, m_damages, damageableLayer);
        setShootingDone.Invoke();
        yield return new WaitForSeconds(0.15f);
        Destroy(bulletFire.gameObject);
    }
}
