using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public static PopupUI Instance { get; private set; }

    public GameObject popupPrefab;
    public int maxPopups = 5;
    public float popupDuration;

    private readonly Queue<GameObject> activePopups = new();

    [SerializeField] private Transform ItemUIParent;
    [SerializeField] private Transform QuestUIParent;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowItemPopup(string itemName, Sprite itemIcon)
    {
        GameObject newPopup = Instantiate(popupPrefab, ItemUIParent);
        TMP_Text text = newPopup.GetComponentInChildren<TMP_Text>();
        int length = itemName.Length;
        if (length > 7)
            text.fontSize = 36f - (length - 7) * 1.5f;
        text.text = $"{itemName} 획득";

        Image iconImage = newPopup.transform.Find("ItemIcon").GetComponent<Image>();
        iconImage.gameObject.SetActive(true);

        iconImage.sprite = itemIcon;

        activePopups.Enqueue(newPopup);
        if(activePopups.Count > maxPopups)
        {
            Destroy(activePopups.Dequeue());
        }

        StartCoroutine(FadeOutAndDestroy(newPopup));
    }

    public void ShowQuestPopup(string questName)
    {
        GameObject newPopup = Instantiate(popupPrefab, QuestUIParent);
        TMP_Text text = newPopup.GetComponentInChildren<TMP_Text>();
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.localPosition = new Vector3(0, 0, 0);
        int length = questName.Length;
        if (length > 7)
            text.fontSize = 36f - (length - 7) * 1.5f;
        text.text = $"{questName} 수락";

        StartCoroutine(FadeOutAndDestroy(newPopup));
    }

    private IEnumerator FadeOutAndDestroy(GameObject popup)
    {
        yield return new WaitForSeconds(popupDuration);
        if (popup == null) yield break;

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        float timer = 0f;
        for(timer = 0f; timer < 1f; timer += Time.deltaTime)
        {
            if (popup == null) yield break;
            canvasGroup.alpha = 1f - timer;
            yield return null;
        }

        Destroy(popup);
    }


}
