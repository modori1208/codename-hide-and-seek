using UnityEngine;
using TMPro;
using System.Collections;
using System;

/// <summary>
/// 알림창 스크립트
/// </summary>
public class NoticeAlert : MonoBehaviour
{

    private GameObject alertPrefab;

    void Start()
    {
        this.alertPrefab = Resources.Load<GameObject>("Alert");
    }

    /// <summary>
    /// 알림창을 생성합니다.
    /// </summary>
    /// <param name="message">알림창 메시지</param>
    /// <returns>알림창 오브젝트</returns>
    public static GameObject Create(string message) => Create(message, 2.0f);

    /// <summary>
    /// 알림창을 생성합니다.
    /// </summary>
    /// <param name="message">알림창 메시지</param>
    /// <param name="duration">알림창 지속 시간 (음수의 경우 항상 켜짐)</param>
    /// <returns>알림창 오브젝트</returns>
    public static GameObject Create(string message, float duration)
    {
        GameObject obj = GameObject.Find("NoticeAlert");
        if (obj == null)
            throw new NullReferenceException("이 메서드를 사용하려면 해당 씬에 \"NoticeAlert\" 프리팹이 있어야합니다.");

        NoticeAlert alert = obj.GetComponent<NoticeAlert>();
        return alert.CreateAlert(message, duration);
    }

    /// <summary>
    /// 알림창을 생성합니다.
    /// </summary>
    /// <param name="message">알림창 메시지</param>
    /// <param name="duration">알림창 지속 시간 (음수의 경우 항상 켜짐)</param>
    /// <returns>알림창 오브젝트</returns>
    public GameObject CreateAlert(string message, float duration)
    {
        // 알림창 오브젝트 생성
        GameObject alert = Instantiate(this.alertPrefab, GameObject.Find("Canvas").transform);
        alert.SetActive(true);

        // 메시지 설정
        TMP_Text textObj = alert.transform.GetChild(0).GetComponent<TMP_Text>();
        textObj.text = message;

        // 애니메이션 재생
        StartCoroutine(AnimationDelay(alert, duration));
        return alert;
    }

    private IEnumerator AnimationDelay(GameObject alert, float duration)
    {
        Animator animator = alert.GetComponent<Animator>();

        // 알림창 생성 애니메이션
        alert.SetActive(true);
        animator.SetBool("isOn", true);

        // 알림창 제거 애니메이션
        if (duration >= 0)
        {
            yield return new WaitForSeconds(duration);

            animator.SetBool("isOn", false);
            yield return new WaitForSeconds(3.0f);
            alert.SetActive(false);
            Destroy(alert);
        }
    }
}
