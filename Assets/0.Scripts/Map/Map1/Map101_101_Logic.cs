using UnityEngine;

// 나중에 상속을 통한 맵 정리 필요할지도...
public class Map101_101_Logic : MonoBehaviour
{
    [SerializeField] private ItemData key1_1_2Data;
    [SerializeField] private ItemData key1_1_3Data;
    [SerializeField] private ItemData key1_1_4Data;

    [SerializeField] private GameObject doorOpened1;
    [SerializeField] private GameObject doorOpened2;
    [SerializeField] private GameObject doorOpened3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!Player.Instance.inventory.HasItem(key1_1_2Data))
            doorOpened1.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        else doorOpened1.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);

        if (!Player.Instance.inventory.HasItem(key1_1_3Data))
            doorOpened2.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        else doorOpened2.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);

        if (!Player.Instance.inventory.HasItem(key1_1_4Data))
            doorOpened3.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        else doorOpened3.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
