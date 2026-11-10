using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] EnemyAI enemyPrefab;
    [SerializeField] float spawnRadius = 25f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float rayHeight = 50f;

    [Header("Ramp")]
    [SerializeField] float startInterval = 5f;
    [SerializeField] float minInterval = 0.4f;
    [SerializeField] float rampDuration = 180f;

    Transform player;
    bool spawning;
    float elapsed;
    float spawnTimer;

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
        spawning = true;
    }

    void HandleSpawning()
    {
        elapsed += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        float interval = Mathf.Lerp(startInterval, minInterval, elapsed / rampDuration);

        if (spawnTimer >= interval)
        {
            spawnTimer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        Vector2 circle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 origin = player.position + new Vector3(circle.x, rayHeight, circle.y);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayHeight * 2f, groundMask))
            Instantiate(enemyPrefab, hit.point, Quaternion.identity, transform);
    }
}