using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    public static EffectSpawner instance;
    [SerializeField] private ParticleSystem EnemyDeathParticleSystemPreFab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Tried to instantiate more than one EffectSpawner!");
            return;
        }
    }


    private static List<ParticleSystem> particleSystemsInUse = new List<ParticleSystem>();
    public static void SpawnEnemyDeathEffect(Vector3 placeOfDeath)
    {

        ParticleSystem particleSystem =
            Instantiate(instance.EnemyDeathParticleSystemPreFab, placeOfDeath, Quaternion.identity);
        particleSystem.Play();
        particleSystemsInUse.Add(particleSystem);
    }

    
    private void ManageParticleSystems()
    {
        for (int i = 0; i < particleSystemsInUse.Count; i++)
        {

        }
    }

}
