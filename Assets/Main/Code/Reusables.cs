using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reusables : MonoBehaviour
{
    public static Reusables instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            Debug.LogError("Tried to create more than one singleton!");
        }

        hexPathHighLights = new List<GameObject>();
        for (int i = 0; i < 128; i++)
        {
            hexPathHighLights.Add(Instantiate(hexPathHighLightPreFab));
            hexPathHighLights[i].SetActive(false);
        }
    }

    private void Start()
    {
        
    }
    [SerializeField] private GameObject hexPathHighLightPreFab;
    private static List<GameObject> hexPathHighLights;

    public static GameObject LoanHexPathHighLight()
    {
        GameObject hexHighLight = hexPathHighLights[hexPathHighLights.Count];
        hexHighLight.SetActive(true);
        hexPathHighLights.RemoveAt(hexPathHighLights.Count);
        return hexHighLight;
    }
    public static List<GameObject> LoanHexPathHighLights(int amount)
    {
        if(amount> hexPathHighLights.Count)
        {
            Debug.LogWarning(amount + " is more than I have: " + hexPathHighLights.Count);
            return null;
        }
        List< GameObject> lentHexHighLights = (hexPathHighLights.GetRange(hexPathHighLights.Count- amount - 1, amount));
        for (int i = 0; i < lentHexHighLights.Count; i++)
        {
            lentHexHighLights[i].SetActive(true);
        }
        hexPathHighLights.RemoveRange(hexPathHighLights.Count - amount - 1, amount);
        return lentHexHighLights;
    }

    public static void ReturnHexPathHighLight(GameObject hexHighLight)
    {
        hexPathHighLights.Add(hexHighLight);
    }
    public static void ReturnHexPathHighLights(List<GameObject>  hexHighLights)
    {
        hexPathHighLights.AddRange(hexHighLights);
        foreach (GameObject gameObject in hexHighLights)
        {
            gameObject.SetActive(false);
        }
    }
}
