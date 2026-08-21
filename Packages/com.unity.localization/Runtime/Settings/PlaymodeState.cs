namespace UnityEngine.Localization
{
    /// <summary>
    /// Used during testing to simulate playmode without having to actually enter playmode.
    /// </summary>
    static class PlaymodeState
    {
        #if UNITY_EDITOR
        /// <summary>
        /// We use this for testing so we don't have to enter play mode.
        /// </summary>
        public static bool? IsPlayingOverride { get; set; }
        #endif

        public static bool IsChangingPlayMode => IsPlayingOrWillChangePlaymode && !IsPlaying;

        public static bool IsPlayingOrWillChangePlaymode
        {
            get
            {
                #if UNITY_EDITOR
                if (IsPlayingOverride.HasValue)
                    return IsPlayingOverride.Value;
                return UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || IsPlaying;
                #else
                return true;
                #endif
            }
        }

        public static bool IsPlaying
        {
            get
            {
                #if UNITY_EDITOR
                if (IsPlayingOverride.HasValue)
                    return IsPlayingOverride.Value;
                #endif
                return Application.isPlaying;
            }
        }
    }
}
