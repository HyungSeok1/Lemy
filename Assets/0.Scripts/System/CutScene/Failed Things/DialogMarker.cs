using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine;

public class MyCustomMarker : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public ExposedReference<GameObject> targetObject;
}
