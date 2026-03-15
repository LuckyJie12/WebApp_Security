using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebApp_Security.Pages
{
    [Authorize("HRManagerOnly")]
    
    public class HRManagerModel : PageModel
    {
        // Simple in-memory store for demonstration. In real app use a database.
        private static readonly List<Employee> _store = new List<Employee>
        {
            new Employee { Id = 1, Name = "Alice", Department = "HR", Role = "Manager" },
            new Employee { Id = 2, Name = "Bob", Department = "HR", Role = "Recruiter" }
        };

        public List<Employee> Employees { get; set; } = new List<Employee>();

        [BindProperty]
        public EmployeeInput NewEmployee { get; set; } = new EmployeeInput();

        public string? StatusMessage { get; set; }

        public void OnGet()
        {
            Employees = _store.ToList();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                Employees = _store.ToList();
                return Page();
            }

            var nextId = _store.Count == 0 ? 1 : _store.Max(e => e.Id) + 1;
            var emp = new Employee { Id = nextId, Name = NewEmployee.Name, Department = NewEmployee.Department, Role = NewEmployee.Role };
            _store.Add(emp);

            StatusMessage = "员工已添加。";
            Employees = _store.ToList();
            // Clear the form
            NewEmployee = new EmployeeInput();
            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            var emp = _store.FirstOrDefault(e => e.Id == id);
            if (emp != null)
            {
                _store.Remove(emp);
                StatusMessage = "员工已删除。";
            }

            Employees = _store.ToList();
            return Page();
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class EmployeeInput
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
