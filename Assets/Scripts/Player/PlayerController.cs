using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IShooter
{
    private Animator m_animator;
    private int m_isRunningForwardHash;
    private int m_isRunningBackwardHash;
    private int m_isRunningLeftHash;
    private int m_isRunningRightHash;
    private int m_isArmedHash;
    private float m_prevH = 0f;
    private float m_prevV = 0f;
    [SerializeField]
    private float m_acceleration = 0.1f;
    [SerializeField] 
    private float m_detectionDistanceForward = 0.5f,
        m_detectionDistanceBackward = 0.5f,
        m_detectionDistanceLeft = 0.5f,
        m_detectionDistanceRight = 0.5f;
    private bool m_isArmed;
    private bool m_isShooting;
    [SerializeField]
    private LayerMask environmentLayerMask;
    [SerializeField]
    private LayerMask m_damageableLayer;
    [SerializeField]
    private Camera m_sceneCamera;
    [SerializeField]
    private Transform m_bulletFirePosition;
    [SerializeField]
    private Transform m_bulletPosition;
    private List<Rifle> m_weaponInventory;
    private int m_maxWeaponsAllowed = 0;
    private int m_currentWeaponIndex;
    private IPickable m_focusedPickableObject;

    public EventHandler<OnWeaponInventoryChangedArgs> m_onWeaponInventoryChanged;
    public EventHandler<OnSelectedWeaponDataChanged> m_onSelectedWeaponDataChanged;
    public class OnWeaponInventoryChangedArgs: EventArgs
    {
        public int m_weaponSlotCount;
        public List<Rifle> m_weapons;
    }
    public class OnSelectedWeaponDataChanged : EventArgs
    {
        public int m_weaponIndex;
        public Rifle m_weapon;
    }

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_isRunningForwardHash = Animator.StringToHash("is_running_forward");
        m_isRunningBackwardHash = Animator.StringToHash("is_running_backward");
        m_isRunningLeftHash = Animator.StringToHash("is_running_left");
        m_isRunningRightHash = Animator.StringToHash("is_running_right");
        m_isArmedHash = Animator.StringToHash("is_armed");
        m_isArmed = false;
        m_isShooting = false;
        m_weaponInventory = new List<Rifle>();
        m_currentWeaponIndex = -1;
        m_maxWeaponsAllowed = 0;
        m_focusedPickableObject = null;
    }

    private void Update()
    {
        float h = m_prevH;
        float v = m_prevV;

        HandleInputs(ref h, ref v);
        CheckMovability(ref h, ref v);
        Move(h, v);
        Animate(h, v);

        m_prevH = h;
        m_prevV = v;
    }

    private void FixedUpdate()
    {
        Rotate();
    }

    /**
     * Handle user inputs 
     */
    private void HandleInputs(ref float h, ref float v)
    {
        HandleMoveInputs(ref h, ref v);
        HandleActionInputs();
    }

    /**
     * Handle move inputs
     */
    private void HandleMoveInputs(ref float h, ref float v)
    {
        if (m_isShooting) return;

        if (!(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)
                    || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)
                    || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)
                    || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)
                )
            ) { v = 0f; h = 0f; }
        else if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow))
            && !(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)
                    || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)
                    || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)
                )
            ) { v = 1f; h = 0f; }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            && !(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)
                    || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)
                    || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)
                )
            ) { v = -1f; h = 0f; }
        else if ((Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
            && !(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)
                    || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)
                    || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)
                )
            ) { v = 0f; h = -1f; }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            && !(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)
                    || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)
                    || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)
                )
            ) { v = 0f; h = 1f; }
    }

    /**
     * Handle action inputs
     */
    private void HandleActionInputs()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) Shoot();
        if (Input.GetKeyDown(KeyCode.E)) PickUp();
        if (Input.GetKeyDown(KeyCode.R)) Release();
    }

    /**
     * Shoot
     */
    public void Shoot()
    {
        if (m_isArmed && m_currentWeaponIndex != -1 && !m_isShooting)
        {
            if (m_weaponInventory[m_currentWeaponIndex].CanFire())
            {
                m_isShooting = true;

                m_setShootingDone += SetShootingDone;

                m_weaponInventory[m_currentWeaponIndex].Fire(
                        m_bulletFirePosition.position,
                        Quaternion.Euler(90f, 0f, -transform.rotation.eulerAngles.y),
                        m_bulletPosition.position,
                        Quaternion.Euler(90f, 0f, 90f - transform.rotation.eulerAngles.y),
                        (m_sceneCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position),
                        m_damageableLayer,
                        m_setShootingDone
                    );

                m_onSelectedWeaponDataChanged?.Invoke(this, new OnSelectedWeaponDataChanged { m_weaponIndex = m_currentWeaponIndex, m_weapon = m_weaponInventory[m_currentWeaponIndex] });
            }
        }
    }

    /**
     * Action called as delegate to control the shooting state of the player
     */
    private UnityAction m_setShootingDone;
    private void SetShootingDone()
    {
        m_isShooting = false;
    }

    /**
     * Check if the player can move in any direction
     */
    private void CheckMovability(ref float h, ref float v)
    {
        bool canMoveForward, canMoveBackward, canMoveLeft, canMoveRight;

        canMoveForward = !Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), m_detectionDistanceForward, environmentLayerMask);
        canMoveBackward = !Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), m_detectionDistanceBackward, environmentLayerMask);
        canMoveLeft = !Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), m_detectionDistanceLeft, environmentLayerMask);
        canMoveRight = !Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), m_detectionDistanceRight, environmentLayerMask);

        if (v > 0f && !canMoveForward) v = 0f;
        if (v < 0f && !canMoveBackward) v = 0f;
        if (h < 0f && !canMoveLeft) h = 0f;
        if (h > 0f && !canMoveRight) h = 0f;
    }

    /**
     * Move the player
     */
    private void Move(float h, float v)
    {
        if (m_isShooting) return;

        transform.Translate(new Vector3(h, 0f, v) * m_acceleration * Time.deltaTime, Space.Self);
    }

    /**
     * Animate the player
     */
    private void Animate(float h, float v)
    {
        if (m_isShooting)
        {
            m_animator.SetBool(m_isRunningForwardHash, false);
            m_animator.SetBool(m_isRunningBackwardHash, false);
            m_animator.SetBool(m_isRunningLeftHash, false);
            m_animator.SetBool(m_isRunningRightHash, false);
            m_animator.SetBool(m_isArmedHash, true);

            return;
        }

        m_animator.SetBool(m_isRunningForwardHash, v > 0f);
        m_animator.SetBool(m_isRunningBackwardHash, v < 0f);
        m_animator.SetBool(m_isRunningLeftHash, h < 0f);
        m_animator.SetBool(m_isRunningRightHash, h > 0f);
    }

    /**
     * Rotate the player
     */
    private void Rotate()
    {
        if (m_isShooting) return;

        Vector3 newPosition = m_sceneCamera.ScreenToWorldPoint(Input.mousePosition);
        newPosition.y = transform.position.y;
        m_sceneCamera.transform.LookAt(m_sceneCamera.transform);
        transform.LookAt(newPosition);  
    }

    /**
     * Carry the provided weapon
     */
    public void Equip(int weaponIndex)
    {
        if (weaponIndex > m_maxWeaponsAllowed - 1 || weaponIndex < 0) 
            throw new NullReferenceException("Invalid weapon index provided.");

        if (m_isArmed && m_currentWeaponIndex != -1)
            UnEquip(m_currentWeaponIndex);

        m_isArmed = true;
        m_currentWeaponIndex = weaponIndex;
        m_weaponInventory[weaponIndex].SetSelected(true);
        m_animator.SetBool(m_isArmedHash, true);
        m_onSelectedWeaponDataChanged?.Invoke(this, new OnSelectedWeaponDataChanged { m_weaponIndex = m_currentWeaponIndex, m_weapon = m_weaponInventory[m_currentWeaponIndex] });
        
        if (m_weaponInventory[weaponIndex] is IReleasable && m_weaponInventory[weaponIndex] is IPickable)
            UIManager.DisplayReleasePopup(((IPickable) m_weaponInventory[weaponIndex]).GetName());
    }

    /**
     * Uncarry the carried weapon
     */
    public void UnEquip(int weaponIndex)
    {
        m_isArmed = false;
        m_weaponInventory[weaponIndex].SetSelected(false);
        m_animator.SetBool(m_isArmedHash, false);
        m_onSelectedWeaponDataChanged?.Invoke(this, new OnSelectedWeaponDataChanged { m_weaponIndex = m_currentWeaponIndex, m_weapon = null });
        m_currentWeaponIndex = -1;
    }

    /**
     * Handle the collision between the player and another object
     */
    private void OnTriggerEnter(Collider collider)
    {
        IPickable pickableObject = collider.GetComponent<IPickable>();

        if (pickableObject != null)
        {
            m_focusedPickableObject = pickableObject;
            UIManager.DisplayPickUpPopup(pickableObject.GetName());
        }
    }

    /**
     * Handle the collision exit between the player and another object
     */
    private void OnTriggerExit(Collider collider)
    {
        IPickable pickableObject = collider.GetComponent<IPickable>();

        if (pickableObject != null)
        {
            m_focusedPickableObject = null;
            UIManager.HidePickUpPopup();
        }
    }

    /**
     * Pick up the currently focused pickable object if any
     */
    private void PickUp()
    {
        if (m_focusedPickableObject == null) return;

        if (m_focusedPickableObject is Rifle r)
        {
            if (m_maxWeaponsAllowed > 0 && m_weaponInventory.Count < m_maxWeaponsAllowed)
            {
                m_weaponInventory.Add(r);
                m_onWeaponInventoryChanged?.Invoke(this, new OnWeaponInventoryChangedArgs { m_weaponSlotCount = m_maxWeaponsAllowed, m_weapons = m_weaponInventory });
            }
            else return;
        }
        else if (m_focusedPickableObject is Slot)
        {
            m_maxWeaponsAllowed++;
            m_onWeaponInventoryChanged?.Invoke(this, new OnWeaponInventoryChangedArgs { m_weaponSlotCount = m_maxWeaponsAllowed, m_weapons = m_weaponInventory });
        }

        UIManager.HidePickUpPopup();
        m_focusedPickableObject.PickUp();
        m_focusedPickableObject = null;
    }

    /**
     * Release the currently selected pickable object if any
     */
    private void Release()
    {
        if (!m_isArmed) return;
        if (m_currentWeaponIndex == -1) return;

        IReleasable releasableObject = m_weaponInventory[m_currentWeaponIndex] is IReleasable ? (IReleasable) m_weaponInventory[m_currentWeaponIndex] : null;

        if (releasableObject == null) return;

        int currentWeaponIndex = m_currentWeaponIndex;
        UnEquip(currentWeaponIndex);
        m_weaponInventory.Remove(m_weaponInventory[currentWeaponIndex]);
        m_onWeaponInventoryChanged?.Invoke(this, new OnWeaponInventoryChangedArgs { m_weaponSlotCount = m_maxWeaponsAllowed, m_weapons = m_weaponInventory });

        UIManager.HideReleasePopup();
        releasableObject.Release(transform.position);
    }

    /**
     * Return the maximum of weapons allowed
     */
    public int GetMaxWeaponsCount()
    {
        return m_maxWeaponsAllowed;
    }

    /**
     * Draw Gizmos in scene view
     */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward) * m_detectionDistanceForward);
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.back) * m_detectionDistanceBackward);
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.left) * m_detectionDistanceLeft);
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.right) * m_detectionDistanceRight);
    }
}
