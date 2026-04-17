using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using RestDomain.Models;

namespace RestaurantInfrastructure.Services
{
    public class EmployerExportService : IExportService<Employer>
    {
        private readonly DbRestaurantContext _context;
        public EmployerExportService(DbRestaurantContext context) => _context = context;

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            var employers = await _context.Employers.ToListAsync(cancellationToken);
            using var workbook = new XLWorkbook();

            var roles = employers.Select(e => e.Role).Distinct();

            foreach (var role in roles)
            {
                var worksheet = workbook.Worksheets.Add(role ?? "Other");
                worksheet.Cell(1, 1).Value = "Прізвище";
                worksheet.Cell(1, 2).Value = "Ім'я";
                worksheet.Cell(1, 3).Value = "Телефон";
                worksheet.Cell(1, 4).Value = "Ставка";
                worksheet.Row(1).Style.Font.Bold = true;

                var roleEmployers = employers.Where(e => e.Role == role).ToList();
                for (int i = 0; i < roleEmployers.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = roleEmployers[i].LastName;
                    worksheet.Cell(i + 2, 2).Value = roleEmployers[i].FirstName;
                    worksheet.Cell(i + 2, 3).Value = roleEmployers[i].PhoneNumber;
                    worksheet.Cell(i + 2, 4).Value = roleEmployers[i].SalaryPerHour;
                }
            }
            workbook.SaveAs(stream);
        }
    }
}