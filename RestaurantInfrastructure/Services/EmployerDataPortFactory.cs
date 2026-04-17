using RestaurantInfrastructure.Services;
using RestDomain.Models;

public class EmployerDataPortFactory : IDataPortServiceFactory<Employer>
{
    private readonly DbRestaurantContext _context;
    public EmployerDataPortFactory(DbRestaurantContext context) => _context = context;

    public IImportService<Employer> GetImportService(string contentType) => new EmployerImportService(_context);
    public IExportService<Employer> GetExportService(string contentType) => new EmployerExportService(_context);
}