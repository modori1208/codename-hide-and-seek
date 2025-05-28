using UnityEngine;

/// <summary>
/// 대상 Transform을 부드럽게 따라가는 카메라 스크립트
/// </summary>
public class CameraFollow : MonoBehaviour
{

    /// <summary>
    /// 카메라 이동의 부드러움 정도 (0~1)
    /// <para>- 0: 따라가지 않음</para>
    /// <para>- 1: 즉시 따라감</para>
    /// </summary>
    [Range(0f, 1f)]
    [SerializeField]
    private float smoothing = 0.09f;

    /// <summary>
    /// 카메라가 따라갈 대상 Transform입니다.
    /// </summary>
    public Transform target;

    void FixedUpdate()
    {
        if (target == null)
            return;

        // 대상 위치로 선형 보간
        Vector3 pos = Vector3.Lerp(transform.position, target.position, smoothing);
        pos.z = transform.position.z;
        transform.position = pos;
    }
}
