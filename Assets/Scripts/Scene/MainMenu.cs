using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 메인 메뉴 캔버스 스크립트
/// </summary>
public class MainMenu : MonoBehaviour
{

    public PhotonLauncher photonLauncher;

    public void OnClickStart()
    {
        Debug.Log("[Main Menu] Start 버튼 눌림 - 서버 연결 시도");
        this.photonLauncher.StartGameConnection();
    }

    public void OnClickCredit()
    {
        Debug.Log("[Main Menu] 크레딧 화면 이동");
        SceneManager.LoadScene("Credit");
    }

    public void OnClickQuit()
    {
        Debug.Log("[Main Menu] 게임 종료 시도");
        Application.Quit();
    }
}
