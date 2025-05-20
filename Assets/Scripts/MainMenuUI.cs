using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("GameScene");  // 씬 이름을 모르겠다..
    }

    public void OnClickCredit()
    {
        SceneManager.LoadScene("CreditScene");
    }

    public void OnClickQuit()
    {
        Debug.Log("게임 종료 시도됨");
        Application.Quit();
    }
}
