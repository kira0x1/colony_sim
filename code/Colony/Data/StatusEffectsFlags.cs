using System;

namespace Kira
{
    [Flags]
    public enum StatusEffectsFlags
    {
        None = 0,
        Bleed = 1,
        Poison = 2,
    }

    public enum VillagerCondition
    {
        Normal = 0,
        Healthy = 1,
        UnHealthy = 2,
        Dead = 4,
    }

    [Flags]
    public enum Emotions
    {
        None = 0,
        Depressed = 1,
        Angry = 2,
        Sad = 3,
        Enraged = 4,
        Jealous = 5,
        Grief = 6,
        Nostalgic = 7
    }

    public enum MoodLevels
    {
        Neutral = 0,
        Content = 1,
        Happy = 2,
        Overjoyed = 3,
        UnHappy = 4,
        Sad = 5,
        Depressed = 6,
    }
}