using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEmployeeRepository
    {
       public int CreateEmployee(Employee e, int passportId);
        public int CreatePassport(string type, string number);
        public int DepartmentCountById(int id);
        public int PassportCountByTypeNumber(string type, string number);
        public int EmployeeCountById(int id);
        public void DeleteEmployeeById(int id);
        public IEnumerable<Employee> GetEmployeesByCompany(int companyId);
        public IEnumerable<Employee> GetEmployeesByDepartmentCompany(int companyId, string departmentName);
        public void UpdateEmployee(string json);
    }
}
