using DG.Tweening;
using UnityEngine;

public class SoulManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Vector3 soulTarget;
    [SerializeField] GameObject soulPrefab;

    [Header("Flight")]
    [SerializeField] Vector2 flightTime = new(0.8f, 1.2f);
    [SerializeField] float startHeight = 1f;
    [SerializeField] Ease ease = Ease.InQuad;

    [Header("Noise")]
    [SerializeField] float noiseAmount = 1.5f;
    [SerializeField] float noiseFrequency = 3f;

    void OnEnable()
    {
        EnemyAI.OnAnyEnemyKilled += SpawnSoul;
    }

    void OnDisable()
    {
        EnemyAI.OnAnyEnemyKilled -= SpawnSoul;
    }

    void SpawnSoul(EnemyAI enemy)
    {
        Vector3 start = enemy.transform.position + Vector3.up * startHeight;
        Transform soul = Instantiate(soulPrefab, start, Quaternion.identity).transform;

        Vector3 seed = Random.insideUnitSphere * 100f;

        DOVirtual.Float(0f, 1f, Random.Range(flightTime.x, flightTime.y), t =>
        {
            Vector3 end = soulTarget;
            Vector3 direction = (end - start).normalized;

            Vector3 side = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 up = Vector3.Cross(side, direction);

            float fade = 1f - t;
            float time = t * noiseFrequency;

            Vector3 noise = side * (Mathf.PerlinNoise(seed.x, time + seed.y) - 0.5f) + up * (Mathf.PerlinNoise(seed.z, time + seed.y) - 0.5f);

            soul.position = Vector3.Lerp(start, end, t) + noise * noiseAmount * fade;
        })
        .SetEase(ease)
        .OnComplete(() => Destroy(soul.gameObject, 2f));
    }
}