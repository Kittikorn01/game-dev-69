// Carries player stats across level scenes; static so it survives SceneManager.LoadScene
public static class PlayerRunData
{
    public static bool HasData { get; private set; }
    public static int Hp { get; private set; }
    public static float Battery { get; private set; }
    public static int Ammo { get; private set; }

    public static void Save(int hp, float battery, int ammo)
    {
        Hp = hp;
        Battery = battery;
        Ammo = ammo;
        HasData = true;
    }

    // Call when a new run starts so the next level begins with full stats
    public static void Clear()
    {
        HasData = false;
    }
}
