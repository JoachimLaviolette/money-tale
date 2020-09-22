using System.Collections;
using UnityEngine;

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
        LayerMask damageableLayer)
    {
        StartCoroutine(FireBullet(
            bulletFirePosition,
            bulletFireRotation,
            bulletPosition,
            bulletRotation,
            bulletDirection,
            damageableLayer));
    }

    private IEnumerator FireBullet(
        Vector3 bulletFirePosition,
        Quaternion bulletFireRotation,
        Vector3 bulletPosition,
        Quaternion bulletRotation,
        Vector3 bulletDirection,
        LayerMask damageableLayer)
    {
        m_weaponAmmo--;
        Transform bulletFire = Instantiate(AssetManager.BulletFire(), bulletFirePosition, bulletFireRotation);
        Bullet bullet = Instantiate(AssetManager.RegularBullet(), bulletPosition, bulletRotation);
        bullet.Setup(bulletDirection, m_bulletSpeed, m_damages, damageableLayer);
        yield return new WaitForSeconds(0.15f);
        Destroy(bulletFire.gameObject);
    }
}
