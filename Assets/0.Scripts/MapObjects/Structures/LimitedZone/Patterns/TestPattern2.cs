// Pattern2.cs
using System.Collections;
using UnityEngine;

public class TestPattern2 : IPattern
{
    public IEnumerator Execute()
    {
        Debug.Log("Pattern 2 시작");
        yield return new WaitForSeconds(2f);
        Debug.Log("Pattern 2 완료");
    }
}
