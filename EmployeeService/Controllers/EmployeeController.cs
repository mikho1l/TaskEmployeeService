using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json.Linq;
using Core.Models;
using Application.Interfaces;

namespace EmployeeService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController(IEmployeeService employeeService) : Controller
    {

        [HttpPost(Name = "CreateEmployee")]
        public JsonResult CreateEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new WebException("Employee was null");
            }
           var employeeId = employeeService.CreateEmployee(employee);
            return Json(employeeId);
        }
        [HttpDelete(Name = "DeleteEmployee")]
        public JsonResult DeleteEmployee(int employeeId)
        {
            employeeService.DeleteEmployee(employeeId);
            return Json("Success");
        }
        [HttpGet("CompanyId")]
        public JsonResult GetEmployeeByCompany(int companyId)
        {
            var employees = employeeService.GetEmployeesByCompany(companyId);
            return Json(employees);
        }
        [HttpGet("CompanyId, DepartmentName")]
        public JsonResult GetEmployeeByDepartmentCompany(int companyId, string departmentName)
        {
            var employees = employeeService.GetEmployeesByDepartmentCompany(companyId, departmentName);
            return Json(employees);
        }
        [HttpPut]
        public JsonResult UpdateEmployee(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("json parameter was null");
            }
            employeeService.UpdateEmployee(json);
            return Json("Success");
        }
    }
}
