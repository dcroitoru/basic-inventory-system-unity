using UnityEditor;

using GDS.Core.Events;

namespace GDS.Core.Editor {

    [InitializeOnLoadAttribute]
    public static class PlayModeStateChanged {

        static PlayModeStateChanged() {
            EditorApplication.playModeStateChanged += onPlayModeChange;
        }

        private static void onPlayModeChange(PlayModeStateChange state) {
            if (state == PlayModeStateChange.ExitingPlayMode) {
                EventBus.GlobalBus.Publish(new ResetEvent());
            }

        }
    }
}