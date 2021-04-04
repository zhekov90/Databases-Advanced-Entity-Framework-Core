namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
                .Where(x => x.EmployeesTasks.Any(et => et.Task.OpenDate >= date))
                .ToArray()
                .Select(x => new
                {
                    Username = x.Username,
                    Tasks = x.EmployeesTasks
                    .Where(et => et.Task.OpenDate >= date)
                    .OrderByDescending(t => t.Task.DueDate)
                    .ThenBy(t => t.Task.Name)
                    .ToArray()
                    .Select(et => new
                    {
                        TaskName = et.Task.Name,
                        OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = et.Task.LabelType.ToString(),
                        ExecutionType = et.Task.ExecutionType.ToString()
                    }).ToList()
                })
                .OrderByDescending(x => x.Tasks.Count)
                .ThenBy(x => x.Username)
                .Take(10).ToList();

            var result = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return result;
        }

        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var sb = new StringBuilder();

            var projects = context
               .Projects
               .Where(x => x.Tasks.Count >= 1)
               .ToArray()
               .Select(x => new ProjectExportViewModel()
               {
                   TasksCount = x.Tasks.Count,
                   ProjectName = x.Name,
                   HasEndDate = x.DueDate == null ? "No" : "Yes",
                   Tasks = x.Tasks.ToArray().Select(t => new TaskExportViewModel()
                   {
                       Name = t.Name,
                       Label = t.LabelType.ToString()
                   })
                       .OrderBy(t => t.Name)
                       .ToArray()
               })
               .OrderByDescending(t => t.TasksCount)
               .ThenBy(t => t.ProjectName)
               .ToList();

            var result = XmlConverter.Serialize(projects, "Projects");

            return result;
        }

    }
}