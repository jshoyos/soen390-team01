using soen390_team01.Data.Entities;

namespace soen390_team01.Services
{
    public interface IProductionService
    {

        public int Interval { get; set; }

        /// <summary>
        /// Producing a bike
        /// </summary>
        /// <param name="bike"></param>
        /// <param name="quantity"></param>
        public void ProduceBike(Bike bike, int quantity);
        /// <summary>
        /// Fixing a stopped production
        /// </summary>
        /// <param name="prod"></param>
        public void FixStoppedProduction(Production prod);
    }
}