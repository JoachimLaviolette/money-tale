using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager m_instance;
    [SerializeField]
    private Texture2D m_mouseCursorTexture;
    [SerializeField]
    private ActionPopup m_pickUpPopup;
    [SerializeField]
    private ActionPopup m_releasePopup;
    [SerializeField]
    private Transform m_weaponInventoryContainer;
    [SerializeField]
    private WeaponSlot m_weaponSlot;
    [SerializeField]
    private WeaponData m_selectedWeaponData;
    [SerializeField]
    private PlayerController m_playerController;

    private static string m_pickupSampleText = "Pick up {0} [E]";
    private static string m_releaseSampleText = "Release {0} [R]";

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
    public static void DisplayPickUpPopup(string objectName)
    {
        m_instance.m_pickUpPopup.gameObject.SetActive(true);
        m_instance.m_pickUpPopup.Setup(m_pickupSampleText, objectName);
    }

    /**
     * Hide the pick up popup
     */
    public static void HidePickUpPopup()
    {
        m_instance.m_pickUpPopup.gameObject.SetActive(false);
    }

    /**
     * Display the release popup
     */
    public static void DisplayReleasePopup(string objectName)
    {
        m_instance.m_releasePopup.gameObject.SetActive(true);
        m_instance.m_releasePopup.Setup(m_releaseSampleText, objectName);
    }

    /**
     * Hide the release popup
     */
    public static void HideReleasePopup()
    {
        m_instance.m_releasePopup.gameObject.SetActive(false);
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

        if (args.m_weaponSlotCount > weaponInventory.Count)
        {
            int currentEmptySlotIndex = args.m_weaponSlotCount - (args.m_weaponSlotCount - weaponInventory.Count);
            for (; currentEmptySlotIndex < args.m_weaponSlotCount; currentEmptySlotIndex++)
                weaponSlots[currentEmptySlotIndex].Setup(true);
        }
    }

    /**
     * Update the currently carried weapon data
     */
    private void UpdateSelectedWeaponData(object sender, PlayerController.OnSelectedWeaponDataChangedArgs args)
    {
        m_instance.m_weaponInventoryContainer.GetComponentsInChildren<WeaponSlot>()[args.m_weaponIndex].SetSelected(args.m_weapon != null);
        m_instance.m_selectedWeaponData.Setup(args.m_weapon);
    }
}
