namespace SimplePlatformer
{
    internal sealed class NpcCombat
    {
        private readonly TestMap map;

        public NpcCombat(TestMap map)
        {
            this.map = map;
        }

        // jelenleg: NPC-k nem sebeznek, nem ütközéses sebzés
        public void CheckNpcCollisions()
        {
            // szándékosan üres
            // később: Guard kaphat contact damage-et, ha akarod
        }
    }
}
