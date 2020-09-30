using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IShooter
{
    private Animator m_animator;
    private int m_isRunningForwardHash;
    private int m_isRunningBackwardHash;
    private int m_isRunningLeftHash;
    private int m_isRunningRightHash;
    private int m_isArmedHash;
    private int m_isDeadHash;
    private float m_prevH = 0f;
    private float m_prevV = 0f;
    [SerializeField]
    private float m_moveSpeed = 0.1f;
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
    private LayerMask m_damageableLayerMask; 
    [SerializeField]
    private LayerMask m_deadLayerMask; 
     [SerializeField]
    private Camera m_sceneCamera;
    [SerializeField]
    private Transform m_bulletFirePosition;
    [SerializeField]
    private Transform m_bulletPosition;
    private Player m_player;
    private List<Rifle> m_weaponInventory;
    private int m_maxWeaponsAllowed = 0;
    private int m_currentWeaponIndex;
    private IPickable m_focusedPickableObject;

    public EventHandler<OnWeaponInventoryChangedArgs> m_onWeaponInventoryChanged;
    public EventHandler<OnSelectedWeaponDataChangedArgs> m_onSelectedWeaponDataChanged;
    public EventHandler<OnPickableDetectedArgs> m_onPickableDetected;
    public EventHandler<OnObjectCarriedArgs> m_onObjectCarried;
    public EventHandler<EventArgs> m_onObjectReleased;
    public EventHandler<OnOutZoneDetectedArgs> m_onOutZoneDetected;
    public class OnWeaponInventoryChangedArgs : EventArgs
    {
        public int m_weaponSlotCount;
        public List<Rifle> m_weapons;
    }
    public class OnSelectedWeaponDataChangedArgs : EventArgs
    {
        public int m_weaponIndex;
        public Rifle m_weapon;
    }
    public class OnPickableDetectedArgs : EventArgs
    {
        public string m_pickableName;
    }
    public class OnObjectCarriedArgs : EventArgs
    {
        public string m_objectName;
    }
    public class OnOutZoneDetectedArgs : EventArgs
    {
        public bool m_isInOutZone;
        public bool m_isOutZoneEnabled;
    }

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_player = GetComponent<Player>();
        m_isRunningForwardHash = Animator.StringToHash("is_running_forward");
        m_isRunningBackwardHash = Animator.StringToHash("is_running_backward");
        m_isRunningLeftHash = Animator.StringToHash("is_running_left");
        m_isRunningRightHash = Animator.StringToHash("is_running_right");
        m_isArmedHash = Animator.StringToHash("is_armed");
        m_isDeadHash = Animator.StringToHash("is_dead");
        m_isArmed = false;
        m_isShooting = false;
        m_weaponInventory = new List<Rifle>();
        m_currentWeaponIndex = -1;
        m_maxWeaponsAllowed = 0;
        m_focusedPickableObject = null;
        m_player.m_onPlayerDamaged += OnPlayerDamagedCallback;
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
        if (!m_isArmed) return;
        if (m_currentWeaponIndex == -1) return;
        if (m_isShooting) return;
        if (!m_weaponInventory[m_currentWeaponIndex].CanFire()) return;

        m_isShooting = true;
        m_weaponInventory[m_currentWeaponIndex].Fire(
                transform,
                m_bulletFirePosition.position,
                Quaternion.Euler(90f, 0f, -transform.rotation.eulerAngles.y),
                m_bulletPosition.position,
                Quaternion.Euler(90f, 0f, 90f - transform.rotation.eulerAngles.y),
                (m_sceneCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position),
                m_damageableLayerMask,
                () => m_isShooting = false
            );

        m_onSelectedWeaponDataChanged?.Invoke(this, new OnSelectedWeaponDataChangedArgs { m_weaponIndex = m_currentWeaponIndex, m_weapon = m_weaponInventory[m_currentWeaponIndex] });
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

        transform.Translate(new Vector3(h, 0f, v) * m_moveSpeed * Time.deltaTime, Space.Self);
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

        Vector3 newRotation = m_sceneCamera.ScreenToWorldPoint(Input.mousePosition);
        newRotation.y = transform.position.y;
        transform.LookAt(newRotation);  
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
        m_onSelectedWeaponDataChanged?.Invoke(this, new OnSelectedWeaponDataChangedArgs { m_weaponIndex = m_currentWeaponIndex, m_weapon = m_weaponInventory[m_currentWeaponIndex] });

        if (m_weaponInventory[weaponIndex] is IReleasable && m_weaponInventory[weaponIndex] is IPickable)
            m_onObjectCarried?.Invoke(this, new OnObjectCarriedArgs { m_objectName = ((IPickable) m_weaponInventory[weaponIndex]).GetName() });
    }

    /**
     * Uncarry the carried weapon
     */
    public void UnEquip(int weaponIndex)
    {
        m_isArmed = false;
        m_weaponInventory[weaponIndex].SetSelected(false);
        m_animator.SetBool(m_isArmedHash, false);
        m_onSelectedWeaponDataChanged?.Invoke(this, new OnSelectedWeaponDataChangedArgs { m_weaponIndex = m_currentWeaponIndex, m_weapon = null });
        m_onObjectReleased?.Invoke(this, EventArgs.Empty);
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
            m_onPickableDetected?.Invoke(this, new OnPickableDetectedArgs { m_pickableName = pickableObject.GetName() });

            return;
        }

        OutZone outZone = collider.GetComponent<OutZone>();

        if (outZone != null)
        {
            m_onOutZoneDetected?.Invoke(this, new OnOutZoneDetectedArgs { 
                m_isInOutZone = true,
                m_isOutZoneEnabled = outZone.isEnabled() });

            return;
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
            m_onPickableDetected?.Invoke(this, new OnPickableDetectedArgs { m_pickableName = null });
        }

        OutZone outZone = collider.GetComponent<OutZone>();

        if (outZone != null)
        {
            m_onOutZoneDetected?.Invoke(this, new OnOutZoneDetectedArgs { m_isInOutZone = false });

            return;
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
        else if (m_focusedPickableObject is Ammo a)
        {
            if (m_isArmed && m_currentWeaponIndex != -1)
            {
                Rifle currentWeapon = m_weaponInventory[m_currentWeaponIndex];

                if (!currentWeapon.IsFull()) currentWeapon.AddAmmo(a.GetAmount());
                else return;
            }
            else return;

            m_onSelectedWeaponDataChanged?.Invoke(this, new OnSelectedWeaponDataChangedArgs { m_weaponIndex = m_currentWeaponIndex, m_weapon = m_weaponInventory[m_currentWeaponIndex] });
        }

        m_onPickableDetected?.Invoke(this, new OnPickableDetectedArgs { m_pickableName = null });
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
        releasableObject.Release(transform.position);
    }

    /**
     * Return the maximum of weapons allowed
     */
    public int GetMaxWeaponsAllowed()
    {
        return m_maxWeaponsAllowed;
    }

    /**
     * Called when the player is somehow damaged
     */
    private void OnPlayerDamagedCallback(object sender, Player.OnPlayerDamagedArgs args)
    {
        if (args.m_playerState == Player.State.Dead)
        {
            // Play dead anim
            m_animator.SetBool(m_isDeadHash, true);
            gameObject.layer = (int) Mathf.Log(m_deadLayerMask.value, 2);
            enabled = false;
            m_player.enabled = false;
            m_onPickableDetected?.Invoke(this, new OnPickableDetectedArgs { m_pickableName = null });
            m_onObjectReleased?.Invoke(this, EventArgs.Empty);
        }

        ReleaseBlood(transform.position, args.m_playerState == Player.State.Dead);
    }

    /**
     * Release some blood around the given position, more if for death
     */
    private void ReleaseBlood(Vector3 position, bool forDeath)
    {
        for (int x = 0; x < UnityEngine.Random.Range(1, 4); x++)
        {
            Transform blood = Instantiate(
                AssetManager.Blood(),
                position + new Vector3(UnityEngine.Random.Range(-0.35f, 0.35f), 0f,
                UnityEngine.Random.Range(-0.35f, 0.35f)),
                Quaternion.Euler(90f, UnityEngine.Random.Range(0f, 360f), 0f),
                AssetManager.BloodVFXContainer());

            float randomScaleFactor = forDeath ? UnityEngine.Random.Range(0.6f, 1f) : UnityEngine.Random.Range(0.1f, 0.6f);
            blood.localScale = new Vector3(
                randomScaleFactor,
                randomScaleFactor,
                1f);
        }
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
