using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UpdateSqlBuilder
    {
        string tableName;
        string sql;
        string whereCondition;
        string sqlPattern = "UPDATE dbo.{0} SET {1} = '{2}'";
        bool addWhereConditionMethodWasCompleted = false;
        bool setPropertyToUpdateWasCompleted = false;
        public UpdateSqlBuilder(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName argument was null or empty");
            }
            sql = sqlPattern.Replace("{0}", tableName);
        }
        public void AddWhereEqualsCondition(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key parameter was null");
            }
            if (!addWhereConditionMethodWasCompleted)
            {
                whereCondition += " WHERE";
            }
            else
            {
                whereCondition += " AND";
            }
            if (value == null)
            {
                whereCondition += $" {key} is null";
            }
            else
            {
                whereCondition += $" {key} = '{value}'";
            }
            addWhereConditionMethodWasCompleted = true;
        }
        public void SetPropertyToUpdate(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key parameter was null");
            }
            value ??= "null";
            if (setPropertyToUpdateWasCompleted)
            {
                sql += " , {1} = '{2}'";
            }
            sql = sql.Replace("{1}", key);
            sql = sql.Replace("{2}", value);
            setPropertyToUpdateWasCompleted = true;
        }
        public bool TryGetQuery(out string query)
        {
            if (!setPropertyToUpdateWasCompleted)
            {
                query = null;
                return false;
            }
            query = sql + whereCondition;
            return true;
        }
    }
}
