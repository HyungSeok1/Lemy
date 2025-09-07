using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

/// <summary>
/// 메시지 존에 들어오면 특정시간동안 메시지를 표시하는 클라스.
/// 이 스크립트가 적용된 오브젝트는 Collider2D가 있어야 함.(RequireComponent)
/// </summary>
/// 

public enum ZoneType
{
    Time,
    Zone
};

public enum OutputType
{
    Message, Image
};

[RequireComponent(typeof(Collider2D))]
public class MessageZone : MonoBehaviour
{
    [SerializeField] private OutputType outputType;
    [SerializeField] private ZoneType zoneType;

    [Tooltip("자동메시지로 뜰 문자열")]
    [SerializeField] private string message;
    [Tooltip("자동메시지로 뜰 이미지")]
    [SerializeField] private Sprite sprite;

    [SerializeField] private Collider2D col2D;

    private AutoMessagePanel AutoMessagePanel => DialoguePanel.Instance.autoMessagePanel;



    // temp
    private bool flag = true;
    private void Start()
    {
        if (flag is true)
            AutoMessagePanel.ShowMessageByTime(message, sprite, outputType);
        flag = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (zoneType == ZoneType.Time)
            AutoMessagePanel.ShowMessageByTime(message, sprite, outputType);
        else if (zoneType == ZoneType.Zone)
            AutoMessagePanel.ShowMessageByZone(message, sprite, outputType);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneType == ZoneType.Zone)
            AutoMessagePanel.HideMessageByZone();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (col2D is BoxCollider2D box)
        {
            Gizmos.matrix = box.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.offset, box.size);
        }
    }
}
