using UnityEngine;
using System.Collections;

public class BossMonster : Monster
{
    [Header("Boss Settings")]
    [SerializeField] private string bossName = "던전 보스";
    [SerializeField] private int bossReward = 500;

    protected override void Start()
    {
        base.Start();

        // 보스 HP UI 
        if (BossHealthUI.Instance != null)
        {
            BossHealthUI.Instance.ShowBossHealth(this, bossName, maxHealth);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        // HP UI
        if (BossHealthUI.Instance != null)
        {
            BossHealthUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    protected override void Die()
    {
        if (BossHealthUI.Instance != null)
        {
            BossHealthUI.Instance.HideBossHealth();
        }

        DropExperience();
        ShowBossClearUI();
        Destroy(gameObject);
    }

    protected override void DropExperience()
    {
        if (ExperienceSystem.Instance != null)
        {
            ExperienceSystem.Instance.GainExperience(bossReward);
        }
    }

    void ShowBossClearUI()
    {
        if (BossClearUI.Instance != null)
        {
            BossClearUI.Instance.ShowClearUI();
        }
    }
}