using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 수집 열쇠 개수 HUD 스크립트
/// </summary>
public class KeyCountHud : MonoBehaviour
{

    /// <summary>
    /// 열쇠(미획득) 스프라이트
    /// </summary>
    [SerializeField]
    private Sprite keyOffSprite;

    /// <summary>
    /// 열쇠(획득) 스프라이트
    /// </summary>
    [SerializeField]
    private Sprite keyOnSprite;

    public void UpdateKeyCount(int count)
    {
        if (count <= 0)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            int current = 0;
            foreach (Transform child in transform)
            {
                Image image = child.gameObject.GetComponent<Image>();
                image.sprite = (current++ < count) ? this.keyOnSprite : this.keyOffSprite;
            }

            this.gameObject.SetActive(true);
        }
    }
}
