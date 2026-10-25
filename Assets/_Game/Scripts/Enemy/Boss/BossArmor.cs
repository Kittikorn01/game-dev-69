using System;
using UnityEngine;

// Armor breaks after the flashlight shines on it long enough, then regrows after a delay.
// While broken the flashlight does nothing extra.
public class BossArmor : MonoBehaviour, ILightReactive
{
    [SerializeField, Min(0.1f)] private float lightTimeToBreak = 5f;
    [SerializeField, Min(0.1f)] private float armorRegenTime = 10f;
    // seconds of light progress lost per second while unlit, so short gaps don't reset it
    [SerializeField, Min(0f)] private float lightDecayRate = 1f;

    private bool isArmored = true;
    private bool isLit;
    private float lightProgress;
    private float regenTimer;

    public event Action ArmorBroken;
    public event Action ArmorRestored;

    public bool IsArmored => isArmored;
    public bool IsLit => isLit;
    public float LightProgress01 => lightProgress / lightTimeToBreak;
    // 0 right after breaking, 1 when the armor is back
    public float RegenProgress01 => 1f - regenTimer / armorRegenTime;

    private void Update()
    {
        if (!isArmored)
        {
            regenTimer -= Time.deltaTime;
            if (regenTimer <= 0f) RestoreArmor();
            return;
        }

        if (isLit) lightProgress += Time.deltaTime;
        else lightProgress = Mathf.Max(lightProgress - lightDecayRate * Time.deltaTime, 0f);

        if (lightProgress >= lightTimeToBreak) BreakArmor();
    }

    public void OnLightEnter()
    {
        isLit = true;
    }

    public void OnLightExit()
    {
        isLit = false;
    }

    private void BreakArmor()
    {
        isArmored = false;
        regenTimer = armorRegenTime;
        Debug.Log($"[BossArmor] Armor broken for {armorRegenTime}s: {name}");
        ArmorBroken?.Invoke();
    }

    private void RestoreArmor()
    {
        isArmored = true;
        lightProgress = 0f;
        Debug.Log($"[BossArmor] Armor restored: {name}");
        ArmorRestored?.Invoke();
    }
}
