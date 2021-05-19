using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameManager gm;

    [Header("Enemy")]

    public List<Enemy> enemyPrefabs;
    public List<Enemy> enemies;
    [HideInInspector] public List<Enemy> enemiesToClear;

    [Space]

    public Transform spawn;

    [Header("Properties")]
    public float enemySpawnProbability = .3f;
    [SerializeField] float enemyDestructionDelay = .3f;

    [Header("Properties")]
    public List<string> enemySpawnPatternData;
    List<Enemy[]> enemySpawnPatterns;

    // Start is called before the first frame update
    void Start()
    {
        if (enemies.Count > 0)
            foreach (var enemy in enemies)
                enemy.gm = this.gm;

        TranslateEnemyPatterns();

        enemiesToClear = new List<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Beat()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy enemy = enemies[i];
            enemy.Beat();
        }

        ClearFlaggedEnemies();

        SpawnNewEnemies();
    }

    void TranslateEnemyPatterns()
    {
        enemySpawnPatterns = new List<Enemy[]>();
        // enemySpawnPatterns.Initialize();

        // Debug.Log($"Waves: {enemySpawnPatternData.Count}");

        for (int i = 0; i < enemySpawnPatternData.Count; i++)
            enemySpawnPatterns.Add(TranslateEnemyPattern(i));
    }

    Enemy[] TranslateEnemyPattern(int index)
    {
        Enemy[] ret = new Enemy[3];

        // Debug.Log($"Getting Spawn Wave {index}");

        string enemyString = enemySpawnPatternData[index];

        enemyString.Trim();
        for (int i = 0; i < 3; i++)
        {
            // Debug.Log($"Parsing {enemyString[i]} as c.");
            int enemyType = int.Parse(enemyString[i].ToString());

            if (enemyType < 1)
            {
                ret[i] = null;
                continue;
            }

            ret[i] = enemyPrefabs[enemyType - 1];
        }

        return ret;
    }

    void SpawnEnemyWave(int index)
    {
        Enemy[] enemyWave = TranslateEnemyPattern(index);

        for (int i = 0; i < 3; i++)
        {
            if (!enemyWave[i])
                continue;

            Enemy e = Instantiate(enemyWave[i]);
            e.gm = gm;
            e.lane = (Player.Lane)i;
            e.transform.parent = spawn;
            e.Setup();
            enemies.Add(e);
        }

    }

    void SpawnNewEnemies()
    {
        if (Random.value > enemySpawnProbability)
            return;

        // @TODO: Queue random enemy pattern
        SpawnEnemyWave(Random.Range(0, enemySpawnPatterns.Count));
    }

    public List<Enemy> GetEnemiesOnRow(int row)
    {
        List<Enemy> ret = new List<Enemy>();

        foreach (var enemy in enemies)
            if ((int)enemy.beatCountdown == row)
                ret.Add(enemy);

        return ret;
    }

    public List<Enemy> GetEnemiesOnLane(Player.Lane lane)
    {
        List<Enemy> ret = new List<Enemy>();

        foreach (var enemy in enemies)
            if (enemy.lane == lane)
                ret.Add(enemy);

        return ret;
    }

    public bool FlagEnemy(Enemy enemy)
    {
        if (!enemy)
            return false;

        enemiesToClear.Add(enemy);
        return true;
    }

    public void ClearFlaggedEnemies()
    {
        int size = enemiesToClear.Count;

        if (size == 0)
            return;

        foreach (var enemy in enemiesToClear)
        {
            enemies.Remove(enemy);
            Destroy(enemy.gameObject, enemyDestructionDelay);
        }

        enemiesToClear.Clear();
        
        /*
        if (size > 0)
            Debug.Log($"Cleared {size} enemies.");
        */
    }
}