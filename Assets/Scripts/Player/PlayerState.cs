using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;

    [Header("UI References")]
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Transform healthRect;
    [SerializeField] Image damageScreenRect;

    [Header("Damage Flash")]
    [SerializeField, Range(0f, 1f)] float flashAlpha = 0.4f;
    [SerializeField] float flashDuration = 0.3f;

    int health;

    public int Health => health;

    void Awake()
    {
        health = maxHealth;
        UpdateHealthUI();
    }

    public void Damage(int amount)
    {
        health = Mathf.Max(0, health - amount);
        UpdateHealthUI();
        PlayDamageFX();
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
}