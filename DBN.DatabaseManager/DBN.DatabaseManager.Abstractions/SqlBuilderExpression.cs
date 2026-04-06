using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace DBN.DatabaseManager.Abstractions
{
    internal class SqlBuilderExpression
    {
        public static string ExecuteWhere<T>(Expression<Func<T, bool>> expr)
        {
            // Get table name from type T
            string tableName = typeof(T).Name.ToUpper(); // optional: adjust casing

            // Support [Table("PATIENT")]
            var tableAttr = typeof(T).GetCustomAttribute<TableAttribute>();
            if (tableAttr != null)
                tableName = tableAttr.Name;

            // Build WHERE clause
            string whereClause = BuildWhere(expr);

            // Return full SQL
            return $"SELECT * FROM {tableName} WHERE {whereClause}";
        }

        public static string ExecuteWhereJoin<T1, T2>(Expression<Func<T1, bool>> expr1, Expression<Func<T2, bool>> expr2)
        {
            string table1 = GetTableName(typeof(T1));
            string table2 = GetTableName(typeof(T2));

            // Optionally infer join
            string joinClause = InferJoin<T1, T2>(table1, table2);

            string where1 = BuildWhere(expr1);
            string where2 = BuildWhere(expr2);

            return $"SELECT * FROM {table1} INNER JOIN {table2} ON {joinClause} WHERE {where1} AND {where2}";
        }

        private static string BuildWhere<T>(Expression<Func<T, bool>> expr)
        {
            return VisitExpression(expr.Body);
        }

        private static string GetTableName(Type t)
        {
            var attr = t.GetCustomAttribute<TableAttribute>();
            return attr?.Name ?? t.Name.ToUpper();
        }

        // Try to infer join based on naming conventions or [ForeignKey] attributes
        private static string InferJoin<T1, T2>(string table1, string table2)
        {
            // e.g., if PatientModel has GpPracticeId and GpPractice has GpPracticeId
            var fk = typeof(T1).GetProperties()
                .FirstOrDefault(p => p.Name.Equals($"{typeof(T2).Name}Id", StringComparison.OrdinalIgnoreCase));

            if (fk != null)
                return $"{table1}.{fk.Name.ToUpper()} = {table2}.{fk.Name.ToUpper()}";

            // fallback: manual join
            return "1=1"; // (acts as dummy join if no relationship found)
        }

        private static string VisitExpression(Expression expr)
        {
            return expr switch
            {
                BinaryExpression b => VisitBinary(b),
                MemberExpression m => VisitMember(m),
                ConstantExpression c => VisitConstant(c),
                MethodCallExpression mc => VisitMethodCall(mc),
                UnaryExpression u when u.NodeType == ExpressionType.Convert => VisitExpression(u.Operand),
                _ => throw new NotSupportedException($"Expression type {expr.NodeType} not supported"),
            };
        }

        private static string VisitBinary(BinaryExpression b)
        {
            string left = VisitExpression(b.Left);
            string right = VisitExpression(b.Right);
            string op = b.NodeType switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.AndAlso => "AND",
                ExpressionType.OrElse => "OR",
                _ => throw new NotSupportedException($"Binary operator {b.NodeType} not supported")
            };
            return $"({left} {op} {right})";
        }

        private static string VisitMember(MemberExpression m)
        {
            if (m.Expression is ParameterExpression)
            {
                var attr = m.Member.GetCustomAttribute<MapsToAttribute>();
                return attr?.ColumnName ?? m.Member.Name.ToUpper(); // map to DB column convention
            }

            // Support captured constants (closures)
            object? value = Expression.Lambda(m).Compile().DynamicInvoke();
            return FormatConstant(value);
        }

        private static string VisitConstant(ConstantExpression c)
        {
            return FormatConstant(c.Value);
        }

        private static string VisitMethodCall(MethodCallExpression mc)
        {
            if (mc.Method.Name == "Contains" && mc.Object != null)
            {
                string col = VisitExpression(mc.Object);
                string val = VisitExpression(mc.Arguments[0]);
                return $"({col} LIKE '%' + {val} + '%')";
            }
            throw new NotSupportedException($"Method {mc.Method.Name} is not supported");
        }

        private static string FormatConstant(object? value)
        {
            if (value == null) return "NULL";
            if (value is string or DateTime) return $"'{value}'";
            if (value is bool b) return b ? "1" : "0";
            return value.ToString()!;
        }
    }
}
