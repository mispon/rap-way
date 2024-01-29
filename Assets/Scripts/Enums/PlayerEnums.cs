using System.ComponentModel;

namespace Enums
{
    public enum Gender
    {
        [Description("gender_male")]
        Male,
        [Description("gender_female")]
        Female
    }
    
    public enum Race
    {
        [Description("race_white")]
        White,
        [Description("race_asian")]
        Asian,
        [Description("race_african")]
        African,
        [Description("race_latino")]
        Latino,
        [Description("race_afroamerican")]
        AfricanAmerican,
        [Description("race_indian")]
        Indian
    }
    
    public enum Skills
    {
        [Description("skill_autotune")]
        Autotune,
        [Description("skill_edlib")]
        Edlib,
        [Description("skill_doubletime")]
        DoubleTime,
        [Description("skill_shoutout")]
        ShoutOut,
        [Description("skill_freestyle")]
        Freestyle,
        [Description("skill_punch")]
        Punch,
        [Description("skill_flip")]
        Flip,
        [Description("skill_dab")]
        Dab,
        [Description("skill_shootdance")]
        ShootDance
    }
}