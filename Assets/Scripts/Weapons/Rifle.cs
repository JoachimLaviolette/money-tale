using UnityEngine;

public abstract class Rifle: MonoBehaviour, IPickable, IReleasable
{
    [SerializeField]
    protected Sprite m_icon;
    protected string m_name;
    [SerializeField]
    [Range(25f, 100f)]
    protected float m_damages = 25f;
    [SerializeField]
    [Range(0f, 1f)]
    protected float m_fireRate = 0.25f; // how much time you have to wait before shooting again
    [SerializeField]
    [Range(1, 50f)]
    protected float m_bulletSpeed; // how fast are the bullets
    [SerializeField]
    [Range(1, 20)] 
    protected int m_weaponMaxAmmo;
    protected int m_weaponAmmo;
    protected int m_bulletRatio = 1; // how many bullets are shot within one shot
    protected bool m_isSelected = false;
    private float m_yTranslation = 10f;

    virtual protected void Start()
    {
        m_weaponAmmo = m_weaponMaxAmmo;
    }

    abstract public void Fire(
        Vector3 bulletFirePosition,
        Quaternion bulletFireRotation,
        Vector3 bulletPosition,
        Quaternion bulletRotation,
        Vector3 bulletDirection,
        LayerMask damageableLayer);

    /**
     * Retun if the rifle can still fire a bullet
     */
    public bool CanFire()
    {
        return m_weaponAmmo > 0;
    }

    /**
     * Get rifle name
     */
    public string GetName()
    {
        return m_name;
    }

    /**
     * Pick up the rifle
     */
    public void PickUp()
    {
        gameObject.transform.Translate(0f, -m_yTranslation, 0f);
    }

    /**
     * Release the rifle at the given position
     */
    public void Release(Vector3 position)
    {
        transform.Translate(0f, m_yTranslation, 0f);
        transform.position = new Vector3(
            position.x,
            transform.position.y,
            position.z);
    }

    /**
     * Get the rifle icon
     */
    public Sprite GetIcon()
    {
        return m_icon;
    }

    /**
     * Get the rifle max ammo capacity
     */
    public int GetMaxAmmo()
    {
        return m_weaponMaxAmmo;
    }

    /**
     * Get the rifle current ammo capacity
     */
    public int GetAmmo()
    {
        return m_weaponAmmo;
    }

    /**
     * Mark the rifle as selected
     */
    public void SetSelected(bool isSelected)
    {
        m_isSelected = isSelected;
    }

    /**
     * Return if the rifle is currently carried by the player
     */
    public bool IsSelected()
    {
        return m_isSelected;
    }
}
