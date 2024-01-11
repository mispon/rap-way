using Spine.Unity;

namespace Utils.Extensions
{
    public static class SpineExtensions
    {
        /// <summary>
        /// Устанавливает очередь анимаций из массива имен анимаций 
        /// </summary>
        public static void SetUpStatesOrder(this SkeletonGraphic graphic, string[] states, bool lastLoop = true, int trackIndex = 0, float delay = 0)
        {
            var index = 0;
            var maxIndex = states.Length - 1;
            bool PlayLoop(int currentIndex) => lastLoop && currentIndex == maxIndex;
            
            graphic.AnimationState.SetAnimation(trackIndex, states[index], PlayLoop(index));
            for (++index; index <= maxIndex; index++)
                graphic.AnimationState.AddAnimation(trackIndex, states[index], PlayLoop(index), delay);
        }
    }
}