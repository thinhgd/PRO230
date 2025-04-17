using Game.Tutorials;
using UnityEngine;

public class EnemyAttackTester : MonoBehaviour
{
    public PlayerStatsDebugger player;
    public EnemyStats testEnemyStats;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.ReceiveAttackFromEnemy(testEnemyStats);
        }
    }
}
