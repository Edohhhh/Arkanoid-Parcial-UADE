using UnityEngine;

public interface IPool
{
    GameObject GetFromPool(Vector3 position);
    void ReturnToPool(GameObject obj);
}