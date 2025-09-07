#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CustomEditor(typeof(ActorControlAsset))]
public class ActorControlAssetEditor : Editor
{
    void OnEnable() => EditorApplication.update += Repaint;
    void OnDisable() => EditorApplication.update -= Repaint;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var asset = (ActorControlAsset)target;
        var director = TimelineEditor.inspectedDirector;   // 현재 타임라인의 디렉터
        var timeline = TimelineEditor.inspectedAsset as TimelineAsset; // 현재 열려있는 타임라인

        double clipDuration = double.NaN;

        if (director != null && timeline != null)
        {
            // 이 타임라인의 모든 트랙/클립에서 이 자산을 쓰는 클립 찾기
            foreach (var track in timeline.GetOutputTracks()) // TrackAsset.GetClips() 사용
            {
                foreach (var clip in track.GetClips())
                {
                    if (clip != null && clip.asset == asset)
                    {
                        clipDuration = clip.duration; // 트림/스트레치 반영된 실제 길이
                        break;
                    }
                }
                if (!double.IsNaN(clipDuration)) break;
            }
        }

        // 숫자만 표시(입력 필드 없음)
        string text = double.IsNaN(clipDuration) ? "—" : clipDuration.ToString("0.######");
        GUILayout.Label("Anim Clip Time: " + text);
    }
}
#endif
