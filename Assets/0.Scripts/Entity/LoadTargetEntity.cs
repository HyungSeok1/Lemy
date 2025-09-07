using UnityEngine;

public interface LoadTargetEntity
{
    public EntityGuide myEntityGuide { get; set; } // 죽을때 flag 바꿔주는용도 참조
    /// <summary>
    /// Die, 키 습득 등 메서드에서 임의로 넣어서 실행.
    /// </summary>
    public void SetSpawnFlagFalse(); 
}
