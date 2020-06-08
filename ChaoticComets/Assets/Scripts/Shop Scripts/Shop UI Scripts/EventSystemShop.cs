using UnityEngine.EventSystems;

// Attribution: PastelStudios & updated version by eron82
// http://blog.pastelstudios.com/2015/09/07/unity-tips-tricks-multiple-event-systems-single-scene-unity-5-1/
// https://forum.unity.com/threads/solved-multiple-eventsystem.512695/

public class EventSystemShop : EventSystem {

    protected override void OnEnable() {
        base.OnEnable();
    }

    protected override void Update() {
        EventSystem originalCurrent = EventSystem.current;
        current = this;
        base.Update();
        current = originalCurrent;
    }
}

