using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour, ICustomUpdate
{
    public float speed = 10f;

    private void OnEnable() => CustomUpdateManager.Register(this);
    private void OnDisable() => CustomUpdateManager.Unregister(this);

    public void CustomUpdate()
    {
        float input = Input.GetAxisRaw("Horizontal");
        Vector3 movement = new Vector3(input, 0, 0) * speed * Time.deltaTime;

        
        Vector3 newPosition = transform.position + movement;

        
        Vector3 halfSize = GetComponent<Renderer>().bounds.extents;

        
        Bounds paddleBounds = new Bounds(newPosition, halfSize * 2);

       
        bool canMove = true;
        ColliderAABB[] colliders = FindObjectsByType<ColliderAABB>(FindObjectsSortMode.None);


        foreach (var col in colliders)
        {
            if (col.type == ColliderAABB.ColliderType.Wall)
            {
                Bounds wallBounds = col.GetComponent<Renderer>().bounds;

                if (paddleBounds.Intersects(wallBounds))
                {
                    canMove = false;
                    break;
                }
            }
        }

        if (canMove)
            transform.position = newPosition;
    }
}
