using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;

// Attribution: PastelStudios & updated version by eron82
// http://blog.pastelstudios.com/2015/09/07/unity-tips-tricks-multiple-event-systems-single-scene-unity-5-1/
// https://forum.unity.com/threads/solved-multiple-eventsystem.512695/

public class MyEventSystemProvider : MonoBehaviour {
    public EventSystem eventSystem;
    public int playerAttachedTo;
    public EventSystem joystickEventSystem;
    public EventSystem keyboardEventSystem;

    public void DetermineControls() {
        if (playerAttachedTo == 1) {
            if (BetweenScenesScript.ControlTypeP1 == 0) {
                eventSystem = joystickEventSystem;
            }
            else {
                eventSystem = keyboardEventSystem;
            }
        }
        else { // (playerAttachedTo == 2)
            if (BetweenScenesScript.ControlTypeP2 == 0) {
                eventSystem = joystickEventSystem;
            }
            else {
                eventSystem = keyboardEventSystem;
            }
        }
    }
}
