using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IEmployeeService
    {
        public int CreateEmployee(Employee employee);
        public void DeleteEmployee(int employeeId);
        public IEnumerable<Employee> GetEmployeesByCompany(int companyId);
        public IEnumerable<Employee> GetEmployeesByDepartmentCompany(int companyId, string departmentName);
        public void UpdateEmployee(string json);
    }
}
