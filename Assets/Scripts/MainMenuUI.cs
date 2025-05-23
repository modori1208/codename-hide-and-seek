using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public PhotonLauncher photonLauncher;

    public void OnClickStart()
    {
        Debug.Log("Start��ư ���� - Photon ���� �õ�");
        photonLauncher.StartGameConnection(); // Photon ���� ����
    }

    public void OnClickCredit()
    {
        SceneManager.LoadScene("Credit");
    }

    public void OnClickQuit()
    {
        Debug.Log("���� ���� �õ���");
        Application.Quit();
    }
}
