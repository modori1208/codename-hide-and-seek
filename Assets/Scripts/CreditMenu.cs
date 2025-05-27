using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 크레딧 메뉴 캔버스 스크립트
/// </summary>
public class CreditMenu : MonoBehaviour
{

    public void OnClickBack()
    {
        SceneManager.LoadScene("Main");
    }
}
