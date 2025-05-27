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

        // �� �޽����� �ִ� ��� �˸�â ���
        if (SceneMessage.Instance != null && !string.IsNullOrEmpty(SceneMessage.Instance.messageToShow))
        {
            SUB(SceneMessage.Instance.messageToShow);

            // �޽����� �� ���� �����ְ� ���
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