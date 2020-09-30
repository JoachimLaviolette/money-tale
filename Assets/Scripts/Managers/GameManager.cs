using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    [SerializeField]
    private Transform m_enemyData;
    private List<Enemy> m_enemies;
    private int m_enemyInitialCount;
    private int m_enemyCurrentCount;
    public static EventHandler<EventArgs> m_onAllEnemiesDead;

    private void Awake()
    {
        if (m_instance == null) m_instance = this;
    }

    private void Start()
    {
        m_instance.m_enemies = m_instance.m_enemyData.GetComponentsInChildren<Enemy>().ToList();
        m_instance.m_enemyInitialCount = m_instance.m_enemies.Count;
        m_instance.m_enemyCurrentCount = m_instance.m_enemyInitialCount;
        ObserveEnemies();
    }

    /**
     * Observe all enemies
     */
    private void ObserveEnemies()
    {
        foreach (Enemy e in m_instance.m_enemies)
            e.m_onEnemyDamaged += OnEnemyDeadCallback;
    }

    /**
     * Tiggered whenever an enemy is damaged to check if it is dead
     */
    private void OnEnemyDeadCallback(object sender, Enemy.OnEnemyDamagedArgs args)
    {
        if (args.m_enemyState == Enemy.State.Dead)
        {
            m_instance.m_enemyCurrentCount--;
            // We stop observing the enemy
            args.m_enemy.m_onEnemyDamaged = null;
        }

        // Called to mark the out zone enabled so the player can access the next level
        if (m_instance.m_enemyCurrentCount == 0)
            m_onAllEnemiesDead?.Invoke(this, EventArgs.Empty);
    }
}
