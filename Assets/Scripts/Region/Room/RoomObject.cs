using System;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] EnemyAI enemyPrefab;
    [SerializeField] int totalEnemies = 100;
    [SerializeField] int maxAlive = 30;
    [SerializeField] float spawnRadius = 25f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float rayHeight = 50f;

    [Header("Ramp")]
    [SerializeField] float startInterval = 5f;
    [SerializeField] float minInterval = 0.4f;

    public event Action OnCleared;

    Transform player;
    bool spawning;
    float spawnTimer;
    int spawned;
    int killed;

    int Alive => spawned - killed;

    void Start()
    {
        player = FindAnyObjectByType<PlayerState>().transform;
    }

    void Update()
    {
        if (spawning && spawned < totalEnemies)
            HandleSpawning();
    }

    public void StartEnemySpawning()
    {
        spawnTimer = 0f;
        spawned = 0;
        killed = 0;
        spawning = true;
    }

    void HandleSpawning()
    {
        spawnTimer += Time.deltaTime;

        float interval = Mathf.Lerp(startInterval, minInterval, (float)spawned / totalEnemies);

        if (spawnTimer >= interval && Alive < maxAlive)
        {
            spawnTimer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 origin = player.position + new Vector3(circle.x, rayHeight, circle.y);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayHeight * 2f, groundMask))
        {
            EnemyAI enemy = Instantiate(enemyPrefab, hit.point, Quaternion.identity, transform);
            enemy.OnEnemyKilled += HandleEnemyKilled;
            spawned++;
        }
    }

    void HandleEnemyKilled(EnemyAI enemy)
    {
        killed++;

        if (killed >= totalEnemies)
            OnCleared?.Invoke();
    }
}