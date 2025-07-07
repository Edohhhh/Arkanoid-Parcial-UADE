using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleLogic : ICustomUpdate
{
    public Transform transform;
    public float speed = 10f;

    public void CustomUpdate()
    {
        float input = Input.GetAxisRaw("Horizontal");
        Vector3 movement = new Vector3(input, 0, 0) * speed * Time.deltaTime;

        Vector3 newPosition = transform.position + movement;
        Vector3 halfSize = GetRendererBoundsExtents(transform);
        Bounds paddleBounds = new Bounds(newPosition, halfSize * 2);

        bool canMove = true;
        ColliderAABB[] colliders = Object.FindObjectsByType<ColliderAABB>(FindObjectsSortMode.None);

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

    private Vector3 GetRendererBoundsExtents(Transform tf)
    {
        var renderer = tf.GetComponent<Renderer>();
        return renderer != null ? renderer.bounds.extents : Vector3.one * 0.5f;
    }
}