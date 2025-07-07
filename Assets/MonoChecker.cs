using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoChecker : MonoBehaviour
{
    void Start()
    {
        MonoBehaviour[] allMB = FindObjectsOfType<MonoBehaviour>();
        Debug.Log("Total MonoBehaviours activos: " + allMB.Length);

        foreach (var mb in allMB)
        {
            Debug.Log(mb.GetType().Name + " en " + mb.gameObject.name);
        }
    }
}