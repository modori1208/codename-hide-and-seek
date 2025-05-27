using UnityEngine;
using TMPro;
using System.Collections;

public class NoticeUI : MonoBehaviour
{
    [Header("SubNotice")]
    public GameObject subbox;
    public TMP_Text subintext;
    public Animator subani;

    private WaitForSeconds _UIDelay1 = new WaitForSeconds(2.0f);
    private WaitForSeconds _UIDelay2 = new WaitForSeconds(0.3f);

    void Start()
    {
        subbox.SetActive(false);

        // 씬 메시지가 있는 경우 알림창 띄움
        if (SceneMessage.Instance != null && !string.IsNullOrEmpty(SceneMessage.Instance.messageToShow))
        {
            SUB(SceneMessage.Instance.messageToShow);

            // 메시지를 한 번만 보여주고 비움
            SceneMessage.Instance.messageToShow = "";
        }
    }

    public void SUB(string message)
    {
        subintext.text = message;
        subbox.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(SUBDelay());
    }

    IEnumerator SUBDelay()
    {
        subbox.SetActive(true);
        subani.SetBool("isOn", true);
        yield return _UIDelay1;
        subani.SetBool("isOn", false);
        yield return _UIDelay2;
        subbox.SetActive(false);
    }
}