using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditUI : MonoBehaviour
{
    public void OnClickBack()
    {
        SceneManager.LoadScene("Main");
    }
}
