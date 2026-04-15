using System;

namespace ThePixelRealms
{
    internal sealed class StoryManager
    {
        public int CurrentStep { get; private set; } = 0;

        public void AdvanceStep()
        {
            CurrentStep++;
        }
    }
}