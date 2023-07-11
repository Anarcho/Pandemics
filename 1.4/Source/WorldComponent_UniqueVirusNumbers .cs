using RimWorld.Planet;
using Verse;

public class WorldComponent_UniqueVirusNumbers : WorldComponent
{
    private int nextVirusNumber = 0;

    public WorldComponent_UniqueVirusNumbers(World world) : base(world) { }

    public int GetNextVirusNumber()
    {
        int virusNumber = nextVirusNumber;
        virusNumber++;
        this.nextVirusNumber = virusNumber;
        return virusNumber;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref nextVirusNumber, "nextVirusNumber");
    }
}
