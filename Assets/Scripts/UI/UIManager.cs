using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager m_instance;
    [SerializeField]
    private Camera m_sceneCamera;
    [SerializeField]
    private Texture2D m_mouseCursorTexture;
    [SerializeField]
    private PickUpPopup m_pickUpPopup;
    [SerializeField]
    private Transform m_weaponInventoryContainer;
    [SerializeField]
    private WeaponSlot m_weaponSlot;
    [SerializeField]
    private WeaponData m_selectedWeaponData;
    [SerializeField]
    private PlayerController m_playerController;

    private void Awake()
    {
        if (m_instance == null) m_instance = this;
    }

    private void Start()
    {
        Cursor.SetCursor(m_instance.m_mouseCursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        SetupWeaponInventory();
        m_instance.m_playerController.m_onWeaponInventoryChanged += UpdateWeaponInventory;
        m_instance.m_playerController.m_onSelectedWeaponDataChanged += UpdateSelectedWeaponData;
    }

    /**
     * Display the pick up popup
     */
    public static void DisplayPickUpPopup(Vector3 position, string objectName)
    {
        m_instance.m_pickUpPopup.gameObject.SetActive(true);
        m_instance.m_pickUpPopup.Setup(objectName);
    }

    /**
     * Hide the pick up popup
     */
    public static void HidePickUpPopup()
    {
        m_instance.m_pickUpPopup.gameObject.SetActive(false);
    }

    /**
     * Setup the weapon inventory
     */
    private void SetupWeaponInventory()
    {
        for (int x = 0; x < m_instance.m_playerController.GetMaxWeaponsCount() - m_instance.m_weaponInventoryContainer.GetComponentsInChildren<WeaponSlot>().Length; x++)
            Instantiate(m_instance.m_weaponSlot, Vector3.zero, Quaternion.identity, m_instance.m_weaponInventoryContainer);
    }

    /**
     * Update the weapon inventory
     */
    private void UpdateWeaponInventory(object sender, PlayerController.OnWeaponInventoryChangedArgs args)
    {
        if (args.m_weaponSlotCount > m_instance.m_weaponInventoryContainer.GetComponentsInChildren<WeaponSlot>().Length)
            SetupWeaponInventory();       

        WeaponSlot[] weaponSlots = m_instance.m_weaponInventoryContainer.GetComponentsInChildren<WeaponSlot>(); 
        List<Rifle> weaponInventory = args.m_weapons;
        int weaponSlotIndex = 0;
        
        foreach (Rifle r in weaponInventory)
        {
            weaponSlots[weaponSlotIndex].Setup(false, m_instance.m_playerController, weaponSlotIndex, r.GetIcon(), r.GetName(), r.IsSelected());
            weaponSlotIndex++;
        }
    }

    /**
     * Update the currently carried weapon data
     */
    private void UpdateSelectedWeaponData(object sender, PlayerController.OnSelectedWeaponDataChanged args)
    {
        m_instance.m_weaponInventoryContainer.GetComponentsInChildren<WeaponSlot>()[args.m_weaponIndex].SetSelected(args.m_weapon != null);
        m_instance.m_selectedWeaponData.Setup(args.m_weapon);
    }
}
