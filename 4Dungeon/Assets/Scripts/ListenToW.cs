using UnityEngine;

public class ListenToW : MonoBehaviour
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

    private void HandleWChanged(int newW)
    {
        float inverseFactor = 1f / Mathf.Max(1f + newW * scaleMultiplier, 0.01f);

        transform.localScale = baseScale * inverseFactor;
    }
}
