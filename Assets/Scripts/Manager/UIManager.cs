using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    private static UIManager m_instance;
    [SerializeField]
    private Transform m_canvasTransform;
    [SerializeField]
    private Texture2D m_mouseCursorTexture;
    [SerializeField]
    private InfoMessage m_infoMessage;
    [SerializeField]
    private ActionPopup m_actionPopup;
    [SerializeField]
    private ActionPopup m_releasePopup;
    [SerializeField]
    private Transform m_weaponInventoryContainer;
    [SerializeField]
    private WeaponSlot m_weaponSlot;
    [SerializeField]
    private WeaponData m_selectedWeaponData;
    [SerializeField]
    private OutZone m_outZone;
    [SerializeField]
    private PlayerController m_playerController;
    [SerializeField]
    private Color
        DEFAULT_ACTION_TEXT_COLOR,
        DEFAULT_ACTION_BACKGROUND_COLOR,
        OUT_ZONE_DISABLED_TEXT_COLOR,
        OUT_ZONE_DISABLED_BACKGROUND_COLOR,
        OUT_ZONE_ENABLED_TEXT_COLOR,
        OUT_ZONE_ENABLED_BACKGROUND_COLOR;

    private static string m_pickupSampleText = "Pick up {0} [E]";
    private static string m_releaseSampleText = "Release {0} [R]";
    private static string m_nextFloorSampleText = "Go to the next floor [A]";
    private static string m_killAllEnemiesSampleText = "Kill all enemies before";

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
        m_instance.m_playerController.m_onPickableDetected += OnPickableDetectedCallback;
        m_instance.m_playerController.m_onObjectCarried += DisplayReleasePopup;
        m_instance.m_playerController.m_onObjectReleased += HideReleasePopup;
        m_instance.m_playerController.m_onOutZoneDetected += OnOutZoneDetectedCallback;
        m_instance.m_playerController.m_onOutZoneDetected += OnOutZoneDetectedCallback;
        GameManager.m_onPlayerDead += OnPlayerDeadCallback;
        GameManager.m_onAllEnemiesDead += EnableOutZone;
    }

    /**
     * Trigger the appropriate callback when a pickable object is detected
     */
    private void OnPickableDetectedCallback(object sender, PlayerController.OnPickableDetectedArgs args)
    {
        if (args.m_pickableName == null) HideActionPopup();
        else DisplayActionPopup(m_pickupSampleText, args.m_pickableName);
    }

    /**
     * Trigger the appropriate callback when the out zone is reached
     */
    private void OnOutZoneDetectedCallback(object sender, PlayerController.OnOutZoneDetectedArgs args)
    {
        if (!args.m_isInOutZone) HideActionPopup();
        else 
            DisplayActionPopup(
                args.m_isOutZoneEnabled ? m_nextFloorSampleText : m_killAllEnemiesSampleText, 
                null,
                () => m_instance.m_actionPopup.Setup(
                    args.m_isOutZoneEnabled ? OUT_ZONE_ENABLED_TEXT_COLOR : OUT_ZONE_DISABLED_TEXT_COLOR,
                    args.m_isOutZoneEnabled ? OUT_ZONE_ENABLED_BACKGROUND_COLOR : OUT_ZONE_DISABLED_BACKGROUND_COLOR));
    }

    /**
     * Triggered when the player is dead
     */
    private void OnPlayerDeadCallback(object sender, GameManager.OnPlayerDeadArgs args)
    {
        InfoMessage infoMessage = Instantiate(m_infoMessage, m_canvasTransform);
        infoMessage.Setup(
            args.m_message,
            args.m_option1, 
            args.m_option2);
    }

    /**
     * Display the action popup
     */
    private void DisplayActionPopup(string sampleText, string textFormatArgs, UnityAction customSetupActionCallback = null)
    {
        m_instance.m_actionPopup.gameObject.SetActive(true);
        m_instance.m_actionPopup.Setup(sampleText, textFormatArgs);

        if (customSetupActionCallback != null) customSetupActionCallback.Invoke();
        else m_instance.m_actionPopup.Setup(DEFAULT_ACTION_TEXT_COLOR, DEFAULT_ACTION_BACKGROUND_COLOR);
    }

    /**
     * Hide the action popup
     */
    private void HideActionPopup()
    {
        m_instance.m_actionPopup.gameObject.SetActive(false);
    }

    /**
     * Display the release popup
     */
    private void DisplayReleasePopup(object sender, PlayerController.OnObjectCarriedArgs args)
    {
        m_instance.m_releasePopup.gameObject.SetActive(true);
        m_instance.m_releasePopup.Setup(m_releaseSampleText, args.m_objectName);
    }

    /**
     * Hide the release popup
     */
    private void HideReleasePopup(object sender, EventArgs args)
    {
        m_instance.m_releasePopup.gameObject.SetActive(false);
    }

    /**
     * Setup the weapon inventory
     */
    private void SetupWeaponInventory()
    {
        for (int x = 0; x < m_instance.m_playerController.GetMaxWeaponsAllowed() - m_instance.m_weaponInventoryContainer.GetComponentsInChildren<WeaponSlot>().Length; x++)
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

    /**
     * Enable out zone
     */
    private void EnableOutZone(object sender, EventArgs args)
    {
        m_instance.m_outZone.SetEnabled(true);
    }
}
