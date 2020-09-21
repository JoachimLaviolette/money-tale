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
        m_instance.m_playerController.m_onWeaponInventoryChanged += UpdateWeaponInventory;
        m_instance.m_playerController.m_onCurrentWeaponDataChanged += UpdateSelectedWeaponData;
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
     * Update the weapon inventory
     */
    private void UpdateWeaponInventory(object sender, PlayerController.OnWeaponInventoryChangedArgs args)
    {
        WeaponSlot[] weaponSlots = m_weaponInventoryContainer.GetComponentsInChildren<WeaponSlot>(); 
        List<Rifle> weaponInventory = args.m_weapons;
        int weaponSlotIndex = 0;

        foreach (Rifle r in weaponInventory)
        {
            weaponSlots[weaponSlotIndex].Setup(false, m_playerController, weaponSlotIndex, r.GetIcon(), r.GetName(), r.IsSelected()); ;
            weaponSlotIndex++;
        }
    }

    /**
     * Update the currently carried weapon data
     */
    private void UpdateSelectedWeaponData(object sender, PlayerController.OnSelectedWeaponDataChanged args)
    {
        m_selectedWeaponData.Setup(args.m_weapon);
    }
}
