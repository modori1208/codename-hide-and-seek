using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMessage : MonoBehaviour
{
    public static SceneMessage Instance;

    public string messageToShow = "";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
