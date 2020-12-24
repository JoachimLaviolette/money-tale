using UnityEngine;

public class Ammo : MonoBehaviour, IPickable
{
    private string m_name;
    [SerializeField]
    [Range(1, 5)]
    private int m_amount = 1;

    private void Start()
    {
        m_name = "Ammo";
    }

    /**
     * Get the slot name
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
        gameObject.SetActive(false);
    }

    /**
     * Return the amount of the ammo
     */
    public int GetAmount()
    {
        return m_amount;
    }
}
