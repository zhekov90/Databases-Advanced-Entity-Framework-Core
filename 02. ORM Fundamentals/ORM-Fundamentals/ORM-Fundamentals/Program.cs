using System;

namespace ORM_Fundamentals
{
    using Microsoft.EntityFrameworkCore.SqlServer;

    using Microsoft.EntityFrameworkCore.Design;

    using ORM_Fundamentals.Models;

    using System.Linq;

    // dotnet tool install --global dotnet-ef
    // dotnet ef dbcontext scaffold "Server=.;Integrated Security=true;Database=SoftUni" Microsoft.EntityFrameworkCore.SqlServer -o Models
    class Program
    {
        static void Main(string[] args)
        {
            var db = new SoftUniContext();
            var employees = db.Employees.Where(x => x.FirstName.StartsWith("G")).OrderByDescending(s => s.Salary).Select(x => new { x.FirstName, x.LastName, x.Salary }).ToList();

            foreach (var e in employees)
            {
                Console.WriteLine($"{e.FirstName} {e.LastName} => {e.Salary}");
            }
        }
    }
}
