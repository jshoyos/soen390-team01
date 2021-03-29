using System;
using System.Reflection;

namespace soen390_team01.Services
{
    public class ProductionReportGeneratorResolver
    {
        private readonly IServiceProvider _provider;

        public ProductionReportGeneratorResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public virtual IProductionReportGenerator Resolve(string name)
        {
            var type = Assembly.GetAssembly(typeof(ProductionReportGeneratorResolver))?.GetType($"{name}ProductionReportGenerator");
            var instance = _provider.GetService(type);

            return instance as IProductionReportGenerator;
        }
    }
}
