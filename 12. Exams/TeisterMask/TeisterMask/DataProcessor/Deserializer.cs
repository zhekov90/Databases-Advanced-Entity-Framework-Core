namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var projects = new List<Project>();

            var projectDtos = XmlConverter
                .Deserializer<ProjectInputModel>(xmlString, "Projects");

            foreach (var projectInfo in projectDtos)
            {
                if (!IsValid(projectInfo))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime openDate;

                var isOpenDateValid = DateTime.TryParseExact(projectInfo.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out openDate);

                if (!isOpenDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? dueDate = null;

                if (!string.IsNullOrEmpty(projectInfo.DueDate))
                {
                    DateTime dueDateValue;

                    var isDueDateValid = DateTime.TryParseExact(projectInfo.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDateValue);

                    if (!isDueDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    dueDate = dueDateValue;
                }

                var project = new Project
                {
                    Name = projectInfo.Name,
                    OpenDate = openDate,
                    DueDate = dueDate
                };


                foreach (var taskInfo in projectInfo.Tasks)
                {
                    if (!IsValid(taskInfo))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime taskOpenDate;

                    var isTaskOpenDateValid = DateTime.TryParseExact(taskInfo.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out taskOpenDate);

                    if (taskOpenDate < project.OpenDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime taskDueDate;

                    var isTaskDueDateValid = DateTime.TryParseExact(taskInfo.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out taskDueDate);

                    if (!isTaskDueDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (dueDate.HasValue && (taskDueDate > project.DueDate))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var task = new Task
                    {
                        Name = taskInfo.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)taskInfo.ExecutionType,
                        LabelType = (LabelType)taskInfo.LabelType
                    };

                    project.Tasks.Add(task);
                }

                projects.Add(project);

                sb.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
            }

            context.Projects.AddRange(projects);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var employees = new List<Employee>();

            var employeeDtos = JsonConvert.DeserializeObject<EmployeeInputModel[]>(jsonString);

            foreach (var employeeInfo in employeeDtos)
            {
                if (!IsValid(employeeInfo) || employeeInfo.Username.Any(x=> !char.IsLetterOrDigit(x)))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var employee = new Employee
                {
                    Username = employeeInfo.Username,
                    Email = employeeInfo.Email,
                    Phone = employeeInfo.Phone
                };

                foreach (var taskInfo in employeeInfo.Tasks.Distinct())
                {
                    if (context.Tasks.All(x => x.Id != taskInfo))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var task = context.Tasks.FirstOrDefault(x => x.Id == taskInfo);

                    var empTask = new EmployeeTask
                    {
                        Employee = employee,
                        Task = task
                    };

                    employee.EmployeesTasks.Add(empTask);

                }

                employees.Add(employee);

                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));

            }

            context.Employees.AddRange(employees);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}