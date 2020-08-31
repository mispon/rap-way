using System;

namespace Models.UI
{
    [Serializable]
    public struct CanvasGroupSettings
    {
        public float alpha;
        public bool interactable;
        public bool blocksRaycasts;
        public bool ignoreParentGroups;
    }
}