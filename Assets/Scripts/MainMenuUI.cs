using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("GameScene");  // �� �̸��� �𸣰ڴ�..
    }

    public void OnClickCredit()
    {
        SceneManager.LoadScene("CreditScene");
    }

    public void OnClickQuit()
    {
        Debug.Log("���� ���� �õ���");
        Application.Quit();
    }
}
