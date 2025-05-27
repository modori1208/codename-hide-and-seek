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
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
