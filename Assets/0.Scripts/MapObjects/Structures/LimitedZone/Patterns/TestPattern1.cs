// Pattern1.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPattern1 : IPattern
{
    GameObject testItem;

    public TestPattern1(GameObject testItem)
    {
        this.testItem = testItem;
    }

    public IEnumerator Execute()
    {
        var spawned = new List<GameObject>();

        for (int i = 0; i < 4; i++)
        {
            Vector3 pos = new Vector3(-6 + i * 2, 0, 0);
            GameObject g = Object.Instantiate(testItem, pos, Quaternion.identity);
            spawned.Add(g);
        }

        Debug.Log("Pattern 1: 모두 파괴될 때까지 대기");

        yield return new WaitUntil(() => spawned.TrueForAll(o => o == null));

        Debug.Log("Pattern 1 완료");
    }
}
