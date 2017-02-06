namespace RPH.Utilities.AI
{
    // RPH
    using Rage;

    public class AdvancedPed : AdvancedEntity
    {
        public Ped Ped { get; }

        public AdvancedPed(Ped ped) : base(ped)
        {
            Ped = ped;
            Ped.BlockPermanentEvents = true;
        }

        public AdvancedPed(Model model, Vector3 position, float heading) : this(new Ped(model, position, heading))
        {
        }

        public AdvancedPed(Vector3 position) : this(new Ped(position))
        {
        }

        protected override void OnSubUpdate()
        {
        }
    }
}
