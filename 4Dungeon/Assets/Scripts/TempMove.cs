using UnityEngine;

public class TempMove : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 5f;
    
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newX = startPosition.x + Mathf.Sin(Time.time * speed) * distance;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}
