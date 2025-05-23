using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public PhotonLauncher photonLauncher;

    public void OnClickStart()
    {
        Debug.Log("Start버튼 눌림 - Photon 연결 시도");
        photonLauncher.StartGameConnection(); // Photon 연결 시작
    }

    public void OnClickCredit()
    {
        SceneManager.LoadScene("Credit");
    }

    public void OnClickQuit()
    {
        Debug.Log("게임 종료 시도됨");
        Application.Quit();
    }
}
