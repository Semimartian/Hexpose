using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class WinScene : MonoBehaviour
{
    public static WinScene instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Tried to instantiate more than one map!");
            return;
        }
    }
    void Start()
    {
        
    }

    private void Update()
    {
        
            if (Input.GetKeyDown(KeyCode.W))
            {
            PlayScene();
            }
        
    }
    public static void PlayScene()
    {
        instance.StartCoroutine(instance.PlayEffects());
    }
    [SerializeField] private ParticleSystem[] confeties;
    [SerializeField] private float[] confetiesDelays;

    private IEnumerator PlayEffects()
    {
        for (int i = 0; i < confeties.Length; i++)
        {
            yield return new WaitForSeconds(confetiesDelays[i]);
            confeties[i].Play();
        }
    }
}
