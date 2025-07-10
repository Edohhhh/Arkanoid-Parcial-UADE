using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICustomUpdate
{
    void CustomUpdate();
}

public class CustomUpdateManager : MonoBehaviour
{
    public static CustomUpdateManager Instance { get; private set; }

    private static readonly List<ICustomUpdate> updatables = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void LateUpdate()
    {
        for (int i = 0; i < updatables.Count; i++)
        {
            updatables[i].CustomUpdate();
        }
    }

    public static void Register(ICustomUpdate obj)
    {
        if (!updatables.Contains(obj))
            updatables.Add(obj);
    }

    public static void Unregister(ICustomUpdate obj)
    {
        if (updatables.Contains(obj))
            updatables.Remove(obj);
    }

    public static void ClearAll()
    {
        updatables.Clear();
    }
}