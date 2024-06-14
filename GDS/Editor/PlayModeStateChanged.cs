using UnityEngine;
using UnityEditor;
using GDS;

namespace GDS {

    [InitializeOnLoadAttribute]
    public static class PlayModeStateChanged {

        static PlayModeStateChanged() {
            EditorApplication.playModeStateChanged += onPlayModeChange;
        }

        private static void onPlayModeChange(PlayModeStateChange state) {
            if (state == PlayModeStateChange.ExitingPlayMode) {
                Global.GlobalBus.Publish(new ResetEvent());
            }

        }
    }
}