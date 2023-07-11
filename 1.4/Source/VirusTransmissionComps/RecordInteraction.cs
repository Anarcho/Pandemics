using RimWorld;
using Verse;

namespace Pandemics
{
    public class InteractionRecord
    {
        public Pawn Initiator { get; set; }
        public Pawn Recipient { get; set; }
        public int Timestamp { get; set; }
    }
}
