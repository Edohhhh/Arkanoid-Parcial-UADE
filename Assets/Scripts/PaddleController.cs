using UnityEngine;

public class PaddleController : MonoBehaviour, ICustomUpdate
{
    public float speed = 10f;

    private void OnEnable()
    {
        CustomUpdateManager.Register(this);
    }

    private void OnDisable()
    {
        CustomUpdateManager.Unregister(this);
    }

    public void CustomUpdate()
    {
        float input = Input.GetAxisRaw("Horizontal");
        Vector3 movement = new Vector3(input, 0, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }
}
