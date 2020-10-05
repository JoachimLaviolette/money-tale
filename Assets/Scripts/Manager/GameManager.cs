using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    [SerializeField]
    private Transform m_enemyData;
    private List<Enemy> m_enemies;
    [SerializeField]
    private PlayerController m_playerController;
    private int m_enemyInitialCount;
    private int m_enemyCurrentCount;
    public static EventHandler<EventArgs> m_onAllEnemiesDead;
    public static EventHandler<OnPlayerDeadArgs> m_onPlayerDead;
    public class OnPlayerDeadArgs : EventArgs
    {
        public string m_message;
        public InfoMessage.Option m_option1;
        public InfoMessage.Option m_option2;
    }

    private static string m_gameOverMessage = "BANKS AIN'T SAFE... YOU'RE DEAD!\nDO YOU WANT TO RETRY?";

    private void Awake()
    {
        if (m_instance == null) m_instance = this;
    }

    private void Start()
    {
        m_instance.m_enemies = m_instance.m_enemyData.GetComponentsInChildren<Enemy>().ToList();
        m_instance.m_enemyInitialCount = m_instance.m_enemies.Count;
        m_instance.m_enemyCurrentCount = m_instance.m_enemyInitialCount;
        ObserveAllEnemies();
        m_instance.m_playerController.m_onPlayerDead += OnPlayerDeadCallback;
    }

    /**
     * Observe all enemies
     */
    private void ObserveAllEnemies()
    {
        foreach (Enemy e in m_instance.m_enemies)
            e.m_onEnemyDamaged += OnEnemyDeadCallback;
    }

    /**
     * Stop all enemies
     */
    private void StopAllEnemies()
    {
        foreach (Enemy e in m_instance.m_enemies)
        {
            e.GetComponent<Animator>().enabled = false;
            e.GetComponent<EnemyController>().enabled = false;
            e.enabled = false;
        }
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


    /**
     * Tiggered when the player is dead
     */
    private void OnPlayerDeadCallback(object sender, EventArgs args)
    {
        StopAllEnemies();

        m_onPlayerDead?.Invoke(m_instance, new OnPlayerDeadArgs
        {
            m_message = m_gameOverMessage,
            m_option1 = new InfoMessage.Option 
            { 
                m_text = "NO [N]",
                m_cmd = KeyCode.N,
                m_callback = () => Application.Quit()
            },
            m_option2 = new InfoMessage.Option
            {
                m_text = "YES [Y]",
                m_cmd = KeyCode.Y,
                m_callback = () => SceneManager.LoadScene(0)
            },
        });
    }
}
