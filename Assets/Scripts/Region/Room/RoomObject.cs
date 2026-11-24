using System.Linq;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] EnemyAI[] enemyPrefabs;
    [SerializeField] int maxAlive = 30;

    [Header("Spawning")]
    [SerializeField] float spawnRadius = 25f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float rayHeight = 50f;

    [Header("Ramp")]
    [SerializeField] float startInterval = 3f;
    [SerializeField] float minInterval = 0.4f;
    [SerializeField] float rampDuration = 300f;
    [SerializeField] float threatStepTime = 60f;

    Transform player;
    bool spawning;
    float elapsed;
    float spawnTimer;
    int alive;

    void Start()
    {
        player = FindAnyObjectByType<PlayerState>().transform;
        StartEnemySpawning();
    }

    void Update()
    {
        if (spawning)
            HandleSpawning();
    }

    public void StartEnemySpawning()
    {
        elapsed = 0f;
        spawnTimer = 0f;
        alive = 0;
        spawning = true;
    }

    void HandleSpawning()
    {
        elapsed += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        float interval = Mathf.Lerp(startInterval, minInterval, elapsed / rampDuration);

        if (spawnTimer >= interval && alive < maxAlive)
        {
            spawnTimer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        EnemyAI prefab = PickEnemy();

        Vector2 circle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 origin = player.position + new Vector3(circle.x, rayHeight, circle.y);

        if (prefab && Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayHeight * 2f, groundMask))
        {
            EnemyAI enemy = Instantiate(prefab, hit.point, Quaternion.identity, transform);
            enemy.OnEnemyKilled += HandleEnemyKilled;
            alive++;
        }
    }

    // Stronger enemies unlock as time passes
    EnemyAI PickEnemy()
    {
        int maxThreat = 1 + (int)(elapsed / threatStepTime);
        EnemyAI[] options = enemyPrefabs.Where(enemy => enemy.Threat <= maxThreat).ToArray();

        return options.Length > 0 ? options[Random.Range(0, options.Length)] : null;
    }

    void HandleEnemyKilled(EnemyAI enemy)
    {
        alive--;
    }
}