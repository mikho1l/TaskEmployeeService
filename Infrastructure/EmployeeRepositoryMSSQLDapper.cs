using Application.Interfaces;
using Core.Models;
using Dapper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class EmployeeRepositoryMSSQLDapper : IEmployeeRepository
    {
        const string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB; AttachDbFilename=C:\\Users\\mikhoil\\source\\repos\\TestTaskEmployeeService\\Infrastructure\\db\\EmployeeDB.mdf; Integrated Security=True; Connect Timeout=30";
        const string employeeTableName = "Employee";
        const string propertyNameId = "id";
        #region CreateEmployeeQueries
        const string createEmployeeQuery = "INSERT INTO [dbo].Employee VALUES (@Name, @Surname, @Phone, @CompanyId, @Passport, " +
            "@Department); SELECT CAST(SCOPE_IDENTITY() as int)";
        const string createPassportQuery = "INSERT INTO [dbo].Passport VALUES (@Type, @Number); SELECT CAST(SCOPE_IDENTITY() as int)";
        const string checkDepartmentExistanceById = "SELECT COUNT(*) FROM [dbo].Department WHERE Id = @Id";
        const string checkPassportUnique = "SELECT COUNT(*) FROM [dbo].Passport WHERE Type = @Type AND Number = @Number";
        #endregion
        #region DeleteEmployeeQueries
        const string checkEmployeeExistance = "SELECT COUNT(*) FROM [dbo].Employee WHERE Id = @Id";
        const string deleteEmployeeById = "DELETE FROM [dbo].Employee WHERE Id = @Id";
        #endregion
        #region GetEmployeesByCompany
        const string getEmployeesByCompany = "SELECT e.Id, e.Name, e.Surname, e.Phone, e.Company as CompanyId, p.Id, p.Type, " +
            "p.Number, d.Id, d.Name, d.Phone" +
            " FROM [dbo].Employee e LEFT JOIN [dbo].Passport p ON e.Passport = p.Id " +
            "LEFT JOIN [dbo].Department d ON e.Department = d.Id WHERE e.Company = @Id";
        #endregion
        #region GetEmployeesByDepartmentCompany
        const string getEmployeesByDepartmentCompany = "SELECT e.Id, e.Name, e.Surname, e.Phone, e.Company as CompanyId, p.Id, p.Type, " +
            "p.Number, d.Id, d.Name, d.Phone" +
            " FROM [dbo].Employee e LEFT JOIN [dbo].Passport p ON e.Passport = p.Id " +
            "LEFT JOIN [dbo].Department d ON e.Department = d.Id WHERE e.Company = @CompanyId and d.Name = @DepartmentName";
        #endregion
        public int DepartmentCountById(int id)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            return db.Query<int>(checkDepartmentExistanceById, new {Id = id}).Single();
        }

        public int EmployeeCountById(int id)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            return db.Query<int>(checkEmployeeExistance, new { Id = id }).Single();
        }

        public int PassportCountByTypeNumber(string type, string number)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            return db.Query<int>(checkPassportUnique, new { Type = type, Number = number}).Single();
        }

        public int CreateEmployee(Employee employee, int passportId)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            return db.Query<int>(createEmployeeQuery, new
            {
                employee.Name,
                employee.Surname,
                employee.Phone,
                employee.CompanyId,
                Passport = passportId,
                Department = employee.Department.Id
            }).Single();
        }

        public int CreatePassport(string type, string number)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            return db.Query<int>(createPassportQuery, new {Type = type, Number = number}).Single();
        }

        public void DeleteEmployeeById(int id)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            db.Query(deleteEmployeeById, new { Id = id });
        }

        public IEnumerable<Employee> GetEmployeesByCompany(int companyId)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            return db.Query<Employee, Passport, Department, Employee>(getEmployeesByCompany, (employee, passport, department) =>
            {
                employee.Passport = passport;
                employee.Department = department;
                return employee;
            }
            , new { Id = companyId });
        }

        public IEnumerable<Employee> GetEmployeesByDepartmentCompany(int companyId, string departmentName)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            return db.Query<Employee, Passport, Department, Employee>(getEmployeesByDepartmentCompany, (employee, passport, department) =>
            {
                employee.Passport = passport;
                employee.Department = department;
                return employee;
            }
               , new { CompanyId = companyId, DepartmentName = departmentName });
        }

        public void UpdateEmployee(string json)
        {
            var builder = new UpdateSqlBuilder(employeeTableName);
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Input JSON was empty");
            }
            var data = JObject.Parse(json);
            if (data.TryGetValue(propertyNameId, StringComparison.CurrentCultureIgnoreCase, out JToken value))
            {
                var valueString = value.ToString();
                builder.AddWhereEqualsCondition(propertyNameId, valueString);
            }
            else
            {
                throw new Exception("Property 'Id' not found");
            }
            var properties = new string[] { "Name", "Surname", "Phone", "Company" };
            foreach (var property in properties)
            {
                if (data.TryGetValue(property, StringComparison.CurrentCultureIgnoreCase, out JToken propertyValue))
                {
                    var valueString = propertyValue.ToString();
                    builder.SetPropertyToUpdate(property, valueString);
                }
            }
            if (builder.TryGetQuery(out string sqlQuery))
            {
                using IDbConnection db = new SqlConnection(connectionString);
                db.Query(sqlQuery);
            }
            else
            {
                throw new Exception("Не удалось сгенерировать запрос.");
            }
        }
    }
}
