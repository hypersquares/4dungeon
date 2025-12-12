using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        print("LevelExit");
        print(other.tag);
        if (other.tag == "LevelExit")
        {
            SceneManager.LoadScene(1);
        }
    }
}
