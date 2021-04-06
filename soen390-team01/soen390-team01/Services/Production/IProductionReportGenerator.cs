using soen390_team01.Data.Entities;

namespace soen390_team01.Services
{
    public interface IProductionReportGenerator
    {
        public string Name { get; }

        /// <summary>
        /// Generates a production report
        /// </summary>
        /// <param name="prod"></param>
        /// <param name="quality"></param>
        public void Generate(Production prod, string quality);
    }
}
