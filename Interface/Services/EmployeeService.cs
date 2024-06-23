using Application.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
    {
        public int CreateEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new Exception("Employee was null");
            }
            bool departmentExists = employeeRepository.DepartmentCountById(employee.Department.Id) != 0;
            if (!departmentExists)
            {
                throw new Exception($"Departnemt Id {employee.Department} not found.");
            }
            bool passportIsUnique = employeeRepository.PassportCountByTypeNumber(employee.Passport.Type, employee.Passport.Number) == 0;
            if (!passportIsUnique)
            {
                throw new Exception($"The passport with Type {employee.Passport.Type} and Number {employee.Passport.Number} already exists.");
            }
            int passportId = employeeRepository.CreatePassport(employee.Passport.Type, employee.Passport.Number);
            int employeeId = employeeRepository.CreateEmployee(employee, passportId);
            return employeeId;
        }

        public void DeleteEmployee(int employeeId)
        {
            bool employeeExists = employeeRepository.EmployeeCountById(employeeId) != 0;
            if (!employeeExists)
            {
                throw new WebException($"Employeer with Id {employeeId} not exists");
            }
            employeeRepository.DeleteEmployeeById(employeeId);
        }

        public IEnumerable<Employee> GetEmployeesByCompany(int companyId)
        {
            return employeeRepository.GetEmployeesByCompany(companyId);
        }

        public IEnumerable<Employee> GetEmployeesByDepartmentCompany(int companyId, string departmentName)
        {
            return employeeRepository.GetEmployeesByDepartmentCompany(companyId, departmentName);
        }

        public void UpdateEmployee(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Input JSON was empty");
            }
            employeeRepository.UpdateEmployee(json);
        }
    }
}
