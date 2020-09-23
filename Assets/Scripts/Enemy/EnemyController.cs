using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour, IShooter
{
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

    private enum Anim
    {
        Damaged, 
        DeadFallBack,
        DeadFlyBack,
    }

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_enemy = GetComponent<Enemy>();
        m_isWalkingForwardHash = Animator.StringToHash("is_walking_forward");
        m_isWalkingBackwardHash = Animator.StringToHash("is_walking_backward");
        m_isDeadFallBackHash = Animator.StringToHash("is_dead_fall_back");
        m_isDeadFlyBackHash = Animator.StringToHash("is_dead_fly_back");
        m_isRunningHash = Animator.StringToHash("is_running");
        m_isDamagedHash = Animator.StringToHash("is_damaged");
        m_isTurningLeft45Hash = Animator.StringToHash("is_turning_left_45");
        m_isTurningRight45Hash = Animator.StringToHash("is_turning_right_45");
        m_isTurningRight90Hash = Animator.StringToHash("is_turning_right_90");
        m_isShooting = false;
        m_waypoints = m_waypointsTransform.GetComponentsInChildren<Transform>().Skip(1).ToArray();
        m_currentTarget = null;
        m_currentWaypointIndex = 0;
        m_enemyScan.m_onPlayerDetected += OnPlayerDetectedCallback;
        m_enemy.m_onEnemyDamaged += OnEnemyDamagedCallback;
    }

    private void Update()
    {
        CheckNextTarget();
        Move();
    }

    /**
     * Check the enemy's next target
     */
    private void CheckNextTarget()
    {
        // Check if the player has reached its current target if any
        if (m_currentTarget != null)
        {
            if (transform.position.Equals(m_currentTarget.position))
            {
                m_currentTarget = null;
                m_currentWaypointIndex++;
            }
        }

        if (m_currentTarget == null)
        {
            m_currentTarget = m_waypoints[m_currentWaypointIndex];
        }

        // Reverse the waypoints array if we reached the last one
        if (m_currentWaypointIndex == m_waypoints.Length)
        {
            m_currentWaypointIndex = 0;
            Array.Reverse(m_waypoints, 0, m_waypoints.Length);
        }
    }

    /**
     * Move the enemy towards its current target
     */
    private void Move()
    {
        // Agent.MoveTo(m_currentTarget.position);
    }

    /**
     * Play theprovided animation
     */
    private IEnumerator Animate(Anim animation)
    {
        switch (animation)
        {
            case Anim.Damaged:
                m_animator.SetBool(m_isDamagedHash, true);
                yield return new WaitForSeconds(0.5f);
                m_animator.SetBool(m_isDamagedHash, false);
                break;
            case Anim.DeadFallBack:
                m_animator.SetBool(m_isDeadFallBackHash, true);
                break;
        }

        yield return null;
    }

    /**
     * Rotate the enemy
     */
    private void Rotate(Vector3 target)
    {

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
            StartCoroutine(Animate(Anim.Damaged));
        }
        else
        {
            // Play dead anim
            StartCoroutine(Animate(Anim.DeadFallBack));
            gameObject.layer = (int) Mathf.Log(m_deadLayerMask.value, 2);
            enabled = false;
            m_enemy.enabled = false;
            m_enemyScan.enabled = false;
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
