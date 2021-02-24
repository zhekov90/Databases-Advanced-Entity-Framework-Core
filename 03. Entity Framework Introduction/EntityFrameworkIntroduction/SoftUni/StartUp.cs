using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var softUniContext = new SoftUniContext();

            var result = GetEmployeesWithSalaryOver50000(softUniContext);
            Console.WriteLine(result);

        }


        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                          .Where(x => x.Salary > 50000)
                          .Select(x => new
                          {
                              x.FirstName,
                              x.Salary,
                          })
                          .OrderBy(x=>x.FirstName)
                          .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
               sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }

            var result = sb.ToString().TrimEnd();

            return result;
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                          .OrderBy(x => x.EmployeeId)
                          .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }

            var result = sb.ToString().TrimEnd();

            return result;
        }
    }

}
