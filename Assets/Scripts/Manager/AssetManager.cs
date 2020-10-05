using UnityEngine;

public class AssetManager : MonoBehaviour
{
    private static AssetManager m_instance;

    [SerializeField]
    private Transform m_bulletFire;
    [SerializeField]
    private Transform m_sidekickBulletFire;
    [SerializeField]
    private Bullet m_regularBullet;
    [SerializeField]
    private Bullet m_sniperBullet;
    [SerializeField]
    private Bullet m_sidekickBullet;
    [SerializeField]
    private Transform m_bloodVFXContainer;
    [SerializeField]
    private Transform m_blood;

    private void Awake()
    {
        if (m_instance == null) m_instance = this;
    }

    public static Transform BulletFire()
    {
        return m_instance.m_bulletFire;
    }

    public static Transform SidekickBulletFire()
    {
        return m_instance.m_sidekickBulletFire;
    }

    public static Bullet RegularBullet()
    {
        return m_instance.m_regularBullet;
    }

    public static Bullet SniperBullet()
    {
        return m_instance.m_sniperBullet;
    }

    public static Bullet SidekickBullet()
    {
        return m_instance.m_sidekickBullet;
    }

    public static Transform BloodVFXContainer()
    {
        return m_instance.m_bloodVFXContainer;
    }

    public static Transform Blood()
    {
        return m_instance.m_blood;
    }
}
