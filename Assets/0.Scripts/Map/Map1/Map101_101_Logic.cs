using UnityEngine;

// 나중에 상속을 통한 맵 정리 필요할지도...
public class Map101_101_Logic : MonoBehaviour
{
    [SerializeField] GameObject doorOpened1;
    [SerializeField] GameObject doorOpened2;
    [SerializeField] GameObject doorOpened3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!Key.HasKey(101)) 
        { 
            doorOpened1.GetComponent<SpriteRenderer>().color = new Color(0,0,0); 
        } 
        else 
        {
            doorOpened1.GetComponent<SpriteRenderer>().color = new Color(1,1,1);
        }

        if (!Key.HasKey(102))
        {
            doorOpened2.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        }
        else
        {
            doorOpened2.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        if (!Key.HasKey(103))
        {
            doorOpened3.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        }
        else
        {
            doorOpened3.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
