using UnityEngine;

public class Slot : MonoBehaviour, IPickable
{
    private string m_name;

    private void Start()
    {
        m_name = "Weapon Slot";    
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
}
