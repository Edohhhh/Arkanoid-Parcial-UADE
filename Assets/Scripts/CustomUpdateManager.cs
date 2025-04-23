using System.Collections.Generic;
using UnityEngine;

public interface ICustomUpdate
{
    void CustomUpdate();
}

public class CustomUpdateManager : MonoBehaviour
{
    private static List<ICustomUpdate> updatables = new List<ICustomUpdate>();

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

    void LateUpdate()
    {
        foreach (var obj in updatables)
        {
            obj.CustomUpdate();
        }
    }
}
