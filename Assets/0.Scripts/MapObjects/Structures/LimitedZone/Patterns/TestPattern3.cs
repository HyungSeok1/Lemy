// Pattern2.cs
using System.Collections;
using UnityEngine;

public class TestPattern3 : IPattern
{
    public IEnumerator Execute()
    {
        Debug.Log("Pattern 3 시작");
        yield return new WaitForSeconds(5f);
        Debug.Log("Pattern 3 완료");
    }
}
