using UnityEngine;
using DG.Tweening;

public class MovingDoor : MonoBehaviour
{
    [SerializeField] private GameObject doorObject;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float moveSpeed = 2f;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = doorObject.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OpenDoor(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OpenDoor(false);
        }
    }

    private void OpenDoor(bool open)
    {
        if (open)
        {
            doorObject.transform.DOMove(targetPosition.position, moveSpeed).SetEase(Ease.OutQuad);
        }
        else
        {
            doorObject.transform.DOMove(initialPosition, moveSpeed).SetEase(Ease.OutQuad);
        }
    }



}
