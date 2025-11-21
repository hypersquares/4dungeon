using UnityEngine;

public class ChangeW : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 0.1f;
    [SerializeField] private Vector3 baseScale = Vector3.one;

    private void OnEnable()
    {
        GameManager.OnWChanged += HandleWChanged;
    }

    private void OnDisable()
    {
        GameManager.OnWChanged -= HandleWChanged;
    }

    private void Start()
    {
        HandleWChanged(GameManager.Instance.W);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            GameManager.Instance.DecrementW(1);
        }

        if (Input.GetKey(KeyCode.E))
        {
            GameManager.Instance.IncrementW(1);
        }
    }

    private void HandleWChanged(int newW)
    {
        transform.localScale = baseScale + Vector3.one * (newW * scaleMultiplier);
    }
}
