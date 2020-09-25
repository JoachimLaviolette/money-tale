using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour, IShooter
{
    [SerializeField]
    private Camera m_sceneCamera;
    private Animator m_animator;
    private int m_isAimingHash;
    private int m_isWalkingForwardHash;
    private int m_isWalkingBackwardHash; // probably not useful
    private int m_isDeadFallBackHash;
    private int m_isDeadFlyBackHash;
    private int m_isRunningHash;
    private int m_isDamagedHash;
    private int m_isTurningLeft45Hash;
    private int m_isTurningRight45Hash;
    private int m_isTurningRight90Hash;
    private bool m_lookForNextTarget;
    private bool m_isShooting;
    [SerializeField]
    private Transform m_waypointsTransform;
    private Transform[] m_waypoints;
    private int m_currentWaypointIndex;
    private Transform m_currentTarget;
    [SerializeField]
    private Transform m_bulletFirePosition;
    [SerializeField]
    private Transform m_bulletPosition;
    private Enemy m_enemy;
    [SerializeField]
    private EnemyScan m_enemyScan;
    [SerializeField]
    private LayerMask m_deadLayerMask;
    private NavMeshAgent m_agent;
    [SerializeField]
    private float m_walkSpeed = 2.5f;
    [SerializeField]
    private float m_runSpeed = 4.5f;
    private State m_currentState;

    private enum State
    {
        IdleRifle,
        IdleAim,
        Damaged, 
        DeadFallBack,
        DeadFlyBack, 
        Running,
        WalkingForward,
        WalkingBackward,
        TurningLeft45,
        TurningRight45, 
        TurningRight90,
    }

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_enemy = GetComponent<Enemy>();
        m_agent = GetComponent<NavMeshAgent>();
        m_isWalkingForwardHash = Animator.StringToHash("is_walking_forward");
        m_isWalkingBackwardHash = Animator.StringToHash("is_walking_backward");
        m_isDeadFallBackHash = Animator.StringToHash("is_dead_fall_back");
        m_isDeadFlyBackHash = Animator.StringToHash("is_dead_fly_back");
        m_isAimingHash = Animator.StringToHash("is_aiming");
        m_isRunningHash = Animator.StringToHash("is_running");
        m_isDamagedHash = Animator.StringToHash("is_damaged");
        m_isTurningLeft45Hash = Animator.StringToHash("is_turning_left_45");
        m_isTurningRight45Hash = Animator.StringToHash("is_turning_right_45");
        m_isTurningRight90Hash = Animator.StringToHash("is_turning_right_90");
        m_lookForNextTarget = true;
        m_isShooting = false;
        m_waypoints = m_waypointsTransform.GetComponentsInChildren<Transform>().Skip(1).ToArray();
        m_currentTarget = null;
        m_currentWaypointIndex = 0;
        m_currentState = State.IdleRifle;
        m_enemyScan.m_onPlayerDetected += OnPlayerDetectedCallback;
        m_enemy.m_onEnemyDamaged += OnEnemyDamagedCallback;
    }

    private void Update()
    {
        LookForNextTarget();
        Animate();
        Rotate();
        Move();
    }

    /**
     * Check the enemy's next target
     */
    private void LookForNextTarget()
    {
        if (!m_lookForNextTarget) return;

        // Check if the enemy has reached its current target if any
        if (m_currentTarget != null)
        {
            if (Vector3.Distance(transform.position, m_currentTarget.position) <= 0.2f)
            {
                m_currentTarget = null;
                m_currentWaypointIndex++;
                m_lookForNextTarget = false;
                State[] stateToExecute = { State.IdleAim, State.TurningLeft45, State.TurningRight45, };
                StartCoroutine(Animate(stateToExecute, 3f, () => { m_lookForNextTarget = true; }));

                return;
            }
        }

        if (m_currentTarget == null && m_waypoints.Length > 0)
        {
            m_currentTarget = m_waypoints[m_currentWaypointIndex];
            m_currentState = State.WalkingForward;
            m_agent.speed = m_walkSpeed;
        }

        // Reverse the waypoints array if we reached the last one
        if (m_currentWaypointIndex == m_waypoints.Length - 1)
        {
            m_currentWaypointIndex = 0;
            Array.Reverse(m_waypoints, 0, m_waypoints.Length);
        }
    }

    /**
     * Animate the enemy
     */
    private void Animate()
    {
        if (!m_lookForNextTarget) return;

        StartCoroutine(Animate(m_currentState));
    }

    /**
     * Rotate the enemy towards the given target
     */
    private void Rotate()
    {
        if (m_currentTarget == null) return;

        Vector3 newRotation = m_currentTarget.position;
        newRotation.y = transform.position.y;
        transform.LookAt(newRotation);
    }

    /**
     * Move the enemy towards its current target if any
     */
    private void Move()
    {
        if (m_currentTarget != null) 
            m_agent.SetDestination(m_currentTarget.position);
    }

    /**
     * Play the animations corresponding to the given states
     */
    private IEnumerator Animate(State[] states, float timeBetweenStateExecution = 0.5f, UnityAction callback = null)
    {
        foreach (State s in states)
        {
            StartCoroutine(Animate(s));
            yield return new WaitForSeconds(timeBetweenStateExecution);
        }

        callback?.Invoke();
    }
    
    private IEnumerator Animate(State state, UnityAction callback = null)
    {
        if (m_animator.GetBool(m_isAimingHash))
            m_animator.SetBool(m_isAimingHash, false);
        if (m_animator.GetBool(m_isDamagedHash))
            m_animator.SetBool(m_isDamagedHash, false);
        if (m_animator.GetBool(m_isRunningHash))
            m_animator.SetBool(m_isRunningHash, false);
        if (m_animator.GetBool(m_isTurningLeft45Hash))
            m_animator.SetBool(m_isTurningLeft45Hash, false);
        if (m_animator.GetBool(m_isTurningRight45Hash))
            m_animator.SetBool(m_isTurningRight45Hash, false);
        if (m_animator.GetBool(m_isTurningRight90Hash))
            m_animator.SetBool(m_isTurningRight90Hash, false);
        if (m_animator.GetBool(m_isWalkingBackwardHash))
            m_animator.SetBool(m_isWalkingBackwardHash, false);
        if (m_animator.GetBool(m_isWalkingForwardHash))
            m_animator.SetBool(m_isWalkingForwardHash, false);

        switch (state)
        {
            case State.Damaged:
                m_animator.SetBool(m_isDamagedHash, true);
                yield return new WaitForSeconds(0.5f);
                m_animator.SetBool(m_isDamagedHash, false);
                break;
            case State.DeadFallBack:
                m_animator.SetBool(m_isDeadFallBackHash, true);
                break;
            case State.DeadFlyBack:
                m_animator.SetBool(m_isDeadFlyBackHash, true);
                break;
            case State.Running:
                if (!m_animator.GetBool(m_isRunningHash)) 
                    m_animator.SetBool(m_isRunningHash, true);
                break;
            case State.WalkingForward:
                if (!m_animator.GetBool(m_isWalkingForwardHash))
                    m_animator.SetBool(m_isWalkingForwardHash, true);
                if (m_animator.GetBool(m_isWalkingBackwardHash))
                    m_animator.SetBool(m_isWalkingBackwardHash, false);
                if (m_animator.GetBool(m_isRunningHash))
                    m_animator.SetBool(m_isRunningHash, false);
                break;
            case State.WalkingBackward:
                if (!m_animator.GetBool(m_isWalkingBackwardHash))
                    m_animator.SetBool(m_isWalkingBackwardHash, true);
                if (m_animator.GetBool(m_isWalkingForwardHash))
                    m_animator.SetBool(m_isWalkingForwardHash, false);
                if (m_animator.GetBool(m_isRunningHash))
                    m_animator.SetBool(m_isRunningHash, false);
                break;
            case State.TurningLeft45:
                if (!m_animator.GetBool(m_isTurningLeft45Hash))
                    m_animator.SetBool(m_isTurningLeft45Hash, true);
                if (m_animator.GetBool(m_isTurningRight45Hash))
                    m_animator.SetBool(m_isTurningRight45Hash, false);
                if (m_animator.GetBool(m_isTurningRight90Hash))
                    m_animator.SetBool(m_isTurningRight90Hash, false);
                break;
            case State.TurningRight45:
                if (!m_animator.GetBool(m_isTurningRight45Hash))
                    m_animator.SetBool(m_isTurningRight45Hash, true);
                if (m_animator.GetBool(m_isTurningLeft45Hash))
                    m_animator.SetBool(m_isTurningLeft45Hash, false);
                if (m_animator.GetBool(m_isTurningRight90Hash))
                    m_animator.SetBool(m_isTurningRight90Hash, false);
                break;
            case State.TurningRight90:
                if (!m_animator.GetBool(m_isTurningRight90Hash))
                    m_animator.SetBool(m_isTurningRight90Hash, true);
                if (m_animator.GetBool(m_isTurningLeft45Hash))
                    m_animator.SetBool(m_isTurningLeft45Hash, false);
                if (m_animator.GetBool(m_isTurningRight45Hash))
                    m_animator.SetBool(m_isTurningRight45Hash, false);
                break;
            case State.IdleAim:
                if (!m_animator.GetBool(m_isAimingHash))
                    m_animator.SetBool(m_isAimingHash, true);
                break;
            default:
                break;
        }

        callback?.Invoke();
        yield return null;
    }

    /**
     * Shoot
     */
    public void Shoot()
    {

    }

    /**
     * Called when the player is detected by the enemy scan
     */
    private void OnPlayerDetectedCallback(object sender, EnemyScan.OnPlayerDetectedArgs args)
    {
        if (args.m_playerTransform != null)
        {
            m_currentState = State.Running;
            m_agent.speed = m_runSpeed;
        } 
        else
        {
            m_currentState = State.IdleAim;
            m_agent.speed = 0f;
        }

        m_currentTarget = args.m_playerTransform;
    }

    /**
     * Called when the enemy is somehow damaged
     */
    private void OnEnemyDamagedCallback(object sender, Enemy.OnEnemyDamagedArgs args)
    {
        if (args.m_enemyState == Enemy.State.Damaged)
        {
            // Play damaged anim
            m_currentState = State.Damaged;
            StartCoroutine(Animate(State.Damaged, () => m_currentState = State.IdleAim));
        }
        else
        {
            // Play dead anim
            m_currentState = State.DeadFallBack;
            StartCoroutine(Animate(State.DeadFallBack));
            gameObject.layer = (int) Mathf.Log(m_deadLayerMask.value, 2);
            enabled = false;
            m_enemy.enabled = false;
            m_enemyScan.enabled = false;
            m_agent.enabled = false;
        }

        ReleaseBlood(transform.position, args.m_enemyState == Enemy.State.Dead);
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
}
