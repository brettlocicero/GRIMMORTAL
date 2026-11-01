using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] Camera cam;
    [SerializeField] DamageNumberUI damageNumberPrefab;
    [SerializeField] Transform instancedUIRoot;

    void Awake()
    {
        Instance = this;
    }

    public void DisplayDamageNumber(Transform target, Vector3 hitPoint, float damage)
    {
        bool inFrontOfCamera = cam.WorldToScreenPoint(hitPoint).z >= 0;

        if (inFrontOfCamera)
        {
            DamageNumberUI damageNumber = Instantiate(damageNumberPrefab, instancedUIRoot);
            damageNumber.Display(damage, target, cam, hitPoint);
        }
    }
}