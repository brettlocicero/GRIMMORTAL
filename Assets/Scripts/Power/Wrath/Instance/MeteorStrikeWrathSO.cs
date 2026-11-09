using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WrathOfTheMeteor", menuName = "Scriptable Objects/Wraths/WrathOfTheMeteor")]
public class WrathOfTheMeteorSO : WrathSO
{
    [Header("Meteor")]
    [SerializeField] Meteor meteorPrefab;
    [SerializeField] float damage = 50f;
    [SerializeField] float stunTime = 1f;
    [SerializeField] float fallSpeed = 30f;
    [SerializeField, Range(10f, 90f)] float fallAngle = 55f;

    [Header("Summon")]
    [SerializeField] float duration = 10f;
    [SerializeField] Vector2 spawnInterval;
    [SerializeField] float radius = 12f;
    [SerializeField] float spawnHeight = 500f;

    public override void Activate(PlayerController player)
    {
        player.StartCoroutine(MeteorRoutine(player));
    }

    IEnumerator MeteorRoutine(PlayerController player)
    {
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            SpawnMeteor(player.transform.position);
            yield return new WaitForSeconds(Random.Range(spawnInterval.x, spawnInterval.y));
        }
    }

    void SpawnMeteor(Vector3 center)
    {
        Vector3 direction = GetRandomFallDirection();
        Vector2 offset = Random.insideUnitCircle * radius;

        Vector3 target = center + new Vector3(offset.x, 0f, offset.y);

        float distance = (spawnHeight - target.y) / -direction.y;
        Vector3 spawnPosition = target - direction * distance;

        Meteor meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.LookRotation(direction));
        meteor.Init(damage, stunTime, direction * fallSpeed);
    }

    Vector3 GetRandomFallDirection()
    {
        Vector2 horizontal = Random.insideUnitCircle.normalized;
        float angle = fallAngle * Mathf.Deg2Rad;

        return new Vector3(horizontal.x * Mathf.Cos(angle), -Mathf.Sin(angle), horizontal.y * Mathf.Cos(angle));
    }
}