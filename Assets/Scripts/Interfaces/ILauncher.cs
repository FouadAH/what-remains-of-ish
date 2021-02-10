public interface ILauncher
{
    int MinRangeDamage { get; set; }
    int MaxRangeDamage { get; set; }
    float RangedAttackMod { get; set; }
    void Launch(ProjectileLauncher projectileLauncher); 
}
