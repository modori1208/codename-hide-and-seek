using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// 알림창 스크립트
/// </summary>
public class NoticeAlert : MonoBehaviour
{

    public static string messageToShow = "";

    [Header("SubNotice")]
    public GameObject subbox;
    public TMP_Text subintext;
    public Animator subani;

    private readonly WaitForSeconds _UIDelay1 = new(2.0f);
    private readonly WaitForSeconds _UIDelay2 = new(0.3f);

    void Start()
    {
        subbox.SetActive(false);

        // 씬 메시지가 있는 경우 알림창 띄움
        if (!string.IsNullOrEmpty(NoticeAlert.messageToShow))
        {
            SUB(NoticeAlert.messageToShow);

            // 메시지를 한 번만 보여주고 비움
            NoticeAlert.messageToShow = "";
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
