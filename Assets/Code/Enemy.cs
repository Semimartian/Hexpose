using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected Rigidbody rigidbody;
    protected bool isAlive = true;
    public void Die(Transform parent)
    {
        isAlive = false;
        transform.parent = parent;
        StartCoroutine(DeathSceneCoroutine());
        Debug.Log("Enemy down!");
    }

    private IEnumerator DeathSceneCoroutine()
    {
       
        yield return new WaitForSeconds(0.6f);
        gameObject.SetActive(false);
        EffectSpawner.SpawnEnemyDeathEffect(transform.position);

    }
}
