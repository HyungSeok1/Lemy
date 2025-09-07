using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CameraSwitchMarker : Marker, INotification
{
    public string switchTarget;
    public PropertyName id => new PropertyName(nameof(CameraSwitchMarker));
}
