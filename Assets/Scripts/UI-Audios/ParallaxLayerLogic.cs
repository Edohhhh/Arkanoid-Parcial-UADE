using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayerLogic : ICustomUpdate
{
    private Transform layer;
    private Transform paddle;
    private float scrollSpeed;
    private float initialX;

    public ParallaxLayerLogic(Transform layer, Transform paddle, float scrollSpeed)
    {
        this.layer = layer;
        this.paddle = paddle;
        this.scrollSpeed = scrollSpeed;
        this.initialX = layer.position.x;
    }

    public void CustomUpdate()
    {
        if (layer == null || paddle == null) return;

        float offset = paddle.position.x * scrollSpeed;
        Vector3 pos = layer.position;
        pos.x = initialX + offset;
        layer.position = pos;
    }
}