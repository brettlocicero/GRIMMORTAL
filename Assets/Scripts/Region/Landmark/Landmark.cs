using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Landmark
{
    [SerializeField] GameObject prefab;
    [SerializeField] Vector2Int count = new(1, 3);
    [SerializeField] Vector2 distance = new(20f, 60f);
    [SerializeField] Vector2 scale = Vector2.one;

    [Header("Spacing")]
    [SerializeField] float radius = 5f;
    [SerializeField] int maxAttempts = 10;

    public void Spawn(Vector3 center, Transform parent, System.Random rng, List<(Vector3 position, float radius)> placed)
    {
        int amount = rng.Next(count.x, count.y + 1);

        for (int i = 0; i < amount; i++)
            TrySpawn(center, parent, rng, placed);
    }

    void TrySpawn(Vector3 center, Transform parent, System.Random rng, List<(Vector3 position, float radius)> placed)
    {
        float size = RandomRange(rng, scale.x, scale.y);
        float footprint = radius * size;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 direction = Quaternion.Euler(0f, RandomRange(rng, 0f, 360f), 0f) * Vector3.forward;
            Vector3 position = center + direction * RandomRange(rng, distance.x, distance.y);

            if (placed.TrueForAll(other => Vector3.Distance(position, other.position) >= footprint + other.radius))
            {
                Quaternion rotation = Quaternion.Euler(0f, RandomRange(rng, 0f, 360f), 0f);

                GameObject instance = Object.Instantiate(prefab, position, rotation, parent);
                instance.transform.localScale *= size;

                placed.Add((position, footprint));
                break;
            }
        }
    }

    float RandomRange(System.Random rng, float min, float max)
    {
        return Mathf.Lerp(min, max, (float)rng.NextDouble());
    }
}