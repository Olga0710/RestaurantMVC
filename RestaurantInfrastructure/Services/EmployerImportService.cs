using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using RestDomain.Models;

namespace RestaurantInfrastructure.Services
{
    public class EmployerImportService : IImportService<Employer>
    {
        private readonly DbRestaurantContext _context;

        public EmployerImportService(DbRestaurantContext context)
        {
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            using var workbook = new XLWorkbook(stream);
            foreach (var worksheet in workbook.Worksheets)
            {
                var roleName = worksheet.Name; // Назва вкладки = Роль

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    var lastName = row.Cell(1).Value.ToString();
                    var firstName = row.Cell(2).Value.ToString();
                    var phone = row.Cell(3).Value.ToString();

                    // Шукаємо за телефоном (унікальний ідентифікатор)
                    var employer = await _context.Employers
                        .FirstOrDefaultAsync(e => e.PhoneNumber == phone, cancellationToken);

                    if (employer == null)
                    {
                        employer = new Employer
                        {
                            LastName = lastName,
                            FirstName = firstName,
                            PhoneNumber = phone,
                            Role = roleName,
                            SalaryPerHour = decimal.TryParse(row.Cell(4).Value.ToString(), out var s) ? s : 0,
                            CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                        };
                        _context.Employers.Add(employer);
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}