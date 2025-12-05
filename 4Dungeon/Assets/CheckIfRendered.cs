using UnityEngine;
using UnityEditor;
[ExecuteInEditMode]
public class CheckIfRendered : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnBecameVisible()
    {
        Debug.Log("I am visible");
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
