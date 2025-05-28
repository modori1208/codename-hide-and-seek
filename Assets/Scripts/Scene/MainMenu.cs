using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 메인 메뉴 캔버스 스크립트
/// </summary>
public class MainMenu : MonoBehaviour
{

    public static bool disconnectByMasterClient = false;

    [SerializeField]
    private PhotonLauncher photonLauncher;

    void FixedUpdate()
    {
        // Start 메서드에서는 오브젝트가 모두 로드되지 않았기에 (...)
        if (disconnectByMasterClient)
        {
            NoticeAlert.Create("호스트 플레이어가 게임을 종료하였습니다.\n서버에 다시 접속해주세요.", 4.0f);
            disconnectByMasterClient = false;
        }
    }

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
