using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static readonly Vector2 Bounds = new Vector2(50, 50);
    public static float Speed = 5;
    public static float SpawnDistanceFromPlayer = 25;
    public static float SpawnConstant = 100f; // higher value delays spawns and makes them more random
    public static float SpawnFrequency = 1f; // ensures spawns do not vary as framerate varies
    public static float SpawnRateIncrease = 0.005f; // increase per second (linear)
    public static float InitialSpawnRate = 1f;
    
    public float zombieSpawnFactor;
    public float archerSpawnFactor;
    public float machineGunnerSpawnFactor;
    public float drunkSpawnFactor;
    public float bomberSpawnFactor;
    public float rocketLauncherSpawnFactor;
    public float sentrySpawnFactor;
    
    public float spawnRate;
    public float timeSinceSpawn;
    public float timeElapsed;
    
    public GameObject zombiePrefab;
    public GameObject archerPrefab;
    public GameObject machineGunnerPrefab;
    public GameObject drunkPrefab;
    public GameObject bomberPrefab;
    public GameObject rocketLauncherPrefab;
    public GameObject sentryPrefab;
    
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        spawnRate = InitialSpawnRate;
        zombieSpawnFactor = 50;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        
        // Increase spawn factor by elapsed time
        timeElapsed += Time.deltaTime;
        timeSinceSpawn += Time.deltaTime;

        // Spawn enemies with probability proportional to the spawn factor
        if (timeSinceSpawn > SpawnFrequency) {
        
            zombieSpawnFactor += timeSinceSpawn;
            if (timeElapsed > 10) archerSpawnFactor += timeSinceSpawn;
            if (timeElapsed > 20) sentrySpawnFactor += timeSinceSpawn;
            if (timeElapsed > 40) machineGunnerSpawnFactor += timeSinceSpawn;
            if (timeElapsed > 60) drunkSpawnFactor += timeSinceSpawn;
            if (timeElapsed > 70) bomberSpawnFactor += timeSinceSpawn;
            if (timeElapsed > 80) rocketLauncherSpawnFactor += timeSinceSpawn;
            if (timeElapsed > 100) spawnRate += SpawnRateIncrease * timeElapsed;
            
            Debug.Log("Spawn Chances: " 
                + SpawnConstant / zombieSpawnFactor + ", " 
                + SpawnConstant / archerSpawnFactor + ", " 
                + SpawnConstant / drunkSpawnFactor + ", " 
                + SpawnConstant / bomberSpawnFactor + ", " 
                + SpawnConstant / machineGunnerSpawnFactor
            );
            
            timeSinceSpawn = 0;
            if (zombieSpawnFactor > 0) {
                // Debug.Log("Spawning with spawn rate " + spawnRate + " and spawn factor " + spawnFactor + ". Random value is from 0 to " + SpawnConstant / spawnRate / spawnFactor);
                if (Random.Range(0, SpawnConstant / zombieSpawnFactor) < 5) {
                    zombieSpawnFactor -= 8; // Note: Exempt from spawn rate increases
                    // TODO: scale down spawns a little bit when there are a lot of enemies
                    SpawnEnemy(zombiePrefab);
                }
            }
            if (archerSpawnFactor > 0) {
                if (Random.Range(0, SpawnConstant / archerSpawnFactor) < 3) {
                    if (timeElapsed < 40) {
                        archerSpawnFactor -= 12; // We want to demonstrate archers early on, but not have them overwhelming later
                    } else {
                        archerSpawnFactor -= 20; // Note: Exempt from spawn rate increases
                    }
                    SpawnEnemy(archerPrefab);
                }
            }
            if (bomberSpawnFactor > 0) {
                if (Random.Range(0, SpawnConstant / bomberSpawnFactor) < 1) {
                    bomberSpawnFactor -= 45;
                    SpawnEnemy(bomberPrefab);
                }
            }
            if (drunkSpawnFactor > 0) {
                if (Random.Range(0, SpawnConstant / drunkSpawnFactor) < 1) {
                    drunkSpawnFactor -= 30 / spawnRate;
                    SpawnEnemy(drunkPrefab);
                }
            }
            if (machineGunnerSpawnFactor > 0) {
                if (Random.Range(0, SpawnConstant / machineGunnerSpawnFactor) < 1) {
                    machineGunnerSpawnFactor -= 45 / spawnRate;
                    SpawnEnemy(machineGunnerPrefab);
                }
            }
            if (rocketLauncherSpawnFactor > 0) {
                if (Random.Range(0, SpawnConstant / rocketLauncherSpawnFactor) < 1) {
                    rocketLauncherSpawnFactor -= 60 / spawnRate;
                    SpawnEnemy(rocketLauncherPrefab);
                }
            }
            if (sentrySpawnFactor > 0) {
                if (Random.Range(0, SpawnConstant / sentrySpawnFactor) < 1) {
                    sentrySpawnFactor -= 30 / spawnRate;
                    SpawnEnemy(sentryPrefab);
                }
            }
            
        }
    }
    
    
    /**
     * Spawns a given enemy at a random position with appropriate stats.
     */
    private void SpawnEnemy(GameObject enemyPrefab) {
        Instantiate(enemyPrefab, GetEnemyPosition(), Quaternion.identity);
    }
    
    private Vector3 GetEnemyPosition() {
        Vector3 direction = Random.onUnitSphere;
        direction.z = 0;
        return player.transform.position + (direction * SpawnDistanceFromPlayer);
    }
}
