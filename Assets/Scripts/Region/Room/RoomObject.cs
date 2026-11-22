using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] EnemyAI[] enemyPrefabs;

    [Header("Waves")]
    [SerializeField] int startBudget = 10;
    [SerializeField] int budgetPerWave = 10;
    [SerializeField] int maxAlive = 30;
    [SerializeField] float breakTime = 10f;

    [Header("Spawning")]
    [SerializeField] float spawnRadius = 25f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float rayHeight = 50f;

    [Header("Ramp")]
    [SerializeField] float startInterval = 3f;
    [SerializeField] float minInterval = 0.4f;

    public event Action OnWaveCleared;

    Transform player;
    bool running;
    bool onBreak;
    float spawnTimer;
    float breakTimer;
    int wave;
    int budgetTotal;
    int budgetLeft;
    int alive;

    public int Wave => wave;

    void Start()
    {
        player = FindAnyObjectByType<PlayerState>().transform;
    }

    void Update()
    {
        if (running)
            Tick();
    }

    public void StartEnemySpawning()
    {
        wave = 0;
        alive = 0;
        running = true;

        StartWave();
    }

    void StartWave()
    {
        wave++;
        budgetTotal = startBudget + (wave - 1) * budgetPerWave;
        budgetLeft = budgetTotal;
        spawnTimer = 0f;
        onBreak = false;
    }

    void Tick()
    {
        if (onBreak)
            HandleBreak();

        else
            HandleWave();

        UpdateUI();
    }

    void HandleWave()
    {
        if (budgetLeft > 0)
            HandleSpawning();

        else if (alive == 0)
            EndWave();
    }

    void HandleBreak()
    {
        breakTimer -= Time.deltaTime;

        if (breakTimer <= 0f)
            StartWave();
    }

    void EndWave()
    {
        onBreak = true;
        breakTimer = breakTime;

        OnWaveCleared?.Invoke();
    }

    void HandleSpawning()
    {
        spawnTimer += Time.deltaTime;

        float spent = 1f - (float)budgetLeft / budgetTotal;
        float interval = Mathf.Lerp(startInterval, minInterval, spent);

        if (spawnTimer >= interval && alive < maxAlive)
        {
            spawnTimer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        EnemyAI prefab = PickEnemy();

        if (prefab == null)
            budgetLeft = 0;

        else
            SpawnEnemy(prefab);
    }

    EnemyAI PickEnemy()
    {
        EnemyAI[] options = enemyPrefabs.Where(enemy => enemy.Threat <= wave && enemy.Threat <= budgetLeft).ToArray();

        return options.Length > 0 ? options[UnityEngine.Random.Range(0, options.Length)] : null;
    }

    void SpawnEnemy(EnemyAI prefab)
    {
        Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 origin = player.position + new Vector3(circle.x, rayHeight, circle.y);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayHeight * 2f, groundMask))
        {
            EnemyAI enemy = Instantiate(prefab, hit.point, Quaternion.identity, transform);
            enemy.OnEnemyKilled += HandleEnemyKilled;

            budgetLeft -= prefab.Threat;
            alive++;
        }
    }

    void HandleEnemyKilled(EnemyAI enemy)
    {
        alive--;
    }

    void UpdateUI()
    {
        string textToUpdate = onBreak
            ? $"Wave {wave} cleared\nNext wave in {Mathf.CeilToInt(breakTimer)}"
            : $"Wave {wave}\nEnemies: {alive}";

        RegionGenerator.Instance.UpdateRegionInfoText(textToUpdate);
    }
}