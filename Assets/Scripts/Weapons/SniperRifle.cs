using System.Collections;
using UnityEngine; 

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
        LayerMask damageableLayer)
    {
        StartCoroutine(FireLaser(
            bulletFirePosition,
            bulletFireRotation,
            bulletPosition,
            bulletRotation,
            bulletDirection,
            damageableLayer));
    }

    private IEnumerator FireLaser(
        Vector3 bulletFirePosition,
        Quaternion bulletFireRotation,
        Vector3 bulletPosition,
        Quaternion bulletRotation,
        Vector3 bulletDirection,
        LayerMask damageableLayer)
    {
        m_weaponAmmo--;
        Transform bulletFire = Instantiate(AssetManager.BulletFire(), bulletFirePosition, bulletFireRotation);

        for (int x = 0; x < 20; x++)
        {
            Bullet bullet = Instantiate(AssetManager.SniperBullet(), bulletPosition, bulletRotation);
            bullet.Setup(bulletDirection, m_bulletSpeed, damageableLayer, false);
            yield return new WaitForSeconds(0f);
        }

        Destroy(bulletFire.gameObject);
    }
}
