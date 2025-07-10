using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PowerUps/DropTable")]
public class PowerUpDropTable : ScriptableObject
{
    [Range(0f, 1f)] public float dropChance = 0.3f;
    public List<PowerUpDropEntry> powerUps;

    public PowerUpEffectSO GetRandomPowerUp()
    {
        Debug.Log($"🔍 Probando drop: chance={dropChance}, entries={powerUps.Count}");

        if (Random.value > dropChance || powerUps.Count == 0)
            return null;

        float totalWeight = 0f;
        foreach (var p in powerUps)
            totalWeight += p.weight;

        float randomPoint = Random.value * totalWeight;

        foreach (var p in powerUps)
        {
            if (randomPoint < p.weight)
                return p.effect;
            randomPoint -= p.weight;
        }

        return null;
    }
}