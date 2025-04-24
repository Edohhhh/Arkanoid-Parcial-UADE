using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICustomUpdate
{
    void CustomUpdate();
}

public class CustomUpdateManager : MonoBehaviour
{
    private static readonly List<ICustomUpdate> updatables = new List<ICustomUpdate>();
    private static readonly List<ICustomUpdate> tempCopy = new List<ICustomUpdate>(); // Lista temporal reutilizable

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
        tempCopy.Clear();                 // Limpiar antes de reutilizar
        tempCopy.AddRange(updatables);   // Copiar referencias sin generar allocaciones

        foreach (var obj in tempCopy)
        {
            obj.CustomUpdate();          // Llamar a la función personalizada
        }
    }
}