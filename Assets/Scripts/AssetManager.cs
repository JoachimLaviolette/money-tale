using UnityEngine;

public class AssetManager : MonoBehaviour
{
    private static AssetManager m_instance;

    [SerializeField]
    private Transform m_bulletFire;
    [SerializeField]
    private Bullet m_regularBullet;
    [SerializeField]
    private Bullet m_sniperBullet;

    private void Awake()
    {
        if (m_instance == null) m_instance = this;
    }

    public static Transform BulletFire()
    {
        return m_instance.m_bulletFire;
    }

    public static Bullet RegularBullet()
    {
        return m_instance.m_regularBullet;
    }

    public static Bullet SniperBullet()
    {
        return m_instance.m_sniperBullet;
    }
}
