using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;

    [SerializeField] int maxHealth = 100;

    [Header("UI References")]
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Transform healthRect;
    [SerializeField] Image damageScreenRect;

    [Header("Damage Flash")]
    [SerializeField, Range(0f, 1f)] float flashAlpha = 0.4f;
    [SerializeField] float flashDuration = 0.3f;

    PlayerCurrency playerCurrency;

    int health;
    bool isDead;

    public int Health => health;

    void Awake()
    {
        GetReferences();
        
        health = maxHealth;
        UpdateHealthUI();

        Instance = this;
    }

    void GetReferences()
    {
        playerCurrency = GetComponentInChildren<PlayerCurrency>();
    }

    public void Damage(int amount)
    {
        if (!isDead)
        {
            health = Mathf.Max(0, health - amount);
            UpdateHealthUI();
            PlayDamageFX();

            if (health <= 0)
                Die();
        }
    }

    void Die()
    {
        isDead = true;
        // RegionGenerator.Instance.ReturnToHub();
    }

    public void Revive()
    {
        isDead = false;
        health = maxHealth;
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        healthText.SetText("{0} / {1}", (float)health, (float)maxHealth);
        healthRect.localScale = new Vector3((float)health / maxHealth, 1f, 1f);
    }

    void PlayDamageFX()
    {
        damageScreenRect.DOKill();
        damageScreenRect.DOFade(0f, flashDuration).From(flashAlpha);

        CinemachineShakeManager.Instance.ShakeCamera(2f, 0.25f, 0.2f, 95f);
    }

    public Transform GetPlayerTransform()
    {
        return transform;
    }

    public void AddCurrency(int amount)
    {
        playerCurrency.AddCurrency(amount);
    }
}