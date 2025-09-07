using UnityEngine;
using UnityEngine.Playables;

public class CameraSwitchReceiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is CameraSwitchMarker marker)
        {
            MainCameraScript.Instance.FollowCameraByName(marker.switchTarget);
            print("작동");
        }
    }

}
