using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Data.Text
{
    public sealed partial class Expression
    {

        #region implicit section

        //public static implicit operator Expression(SqlValue value)
        //{
        //    return new Expression(value); 
        //}

        public static implicit operator Expression(string value)
        {
            return new Expression(new SqlValue(value));    // s= 'string'
        }

        public static implicit operator Expression(bool value)
        {
            return new Expression(new SqlValue(value));    // b=1 or b=0
        }


        public static implicit operator Expression(char value)
        {
            return new Expression(new SqlValue(value));     // ch= 'c'
        }

        public static implicit operator Expression(byte[] value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(byte value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(sbyte value)
        {
            return new Expression(new SqlValue(value));
        }


        public static implicit operator Expression(int value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(short value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(ushort value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(uint value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(long value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(ulong value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(double value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(float value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(decimal value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(Guid value)
        {
            return new Expression(new SqlValue(value));
        }

        public static implicit operator Expression(DateTime value)
        {
            return new Expression(new SqlValue(value));   //dt = '10/20/2012'
        }

        public static implicit operator Expression(DateTimeOffset value)
        {
            return new Expression(new SqlValue(value));  
        }

        public static implicit operator Expression(DBNull value)
        {
            return new Expression(new SqlValue(value));   // NULL
        }

        public static implicit operator Expression(Enum value)
        {
            return new Expression(new SqlValue(Convert.ToInt32(value)));
        }

        #endregion


        /// <summary>
        /// string s = (string)expr;
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static explicit operator string(Expression expr)
        {
            return expr.ToString();
        }

        #region operator + - * / %, and, or, not

        public static Expression operator -(Expression expr)
        {
            return OPR("-", expr);
        }

        public static Expression operator +(Expression expr)
        {
            return OPR("+", expr);
        }

        public static Expression operator +(Expression expr1, Expression expr2)
        {
            return OPR(expr1, "+", expr2);
        }

        public static Expression operator -(Expression expr1, Expression expr2)
        {
            return OPR(expr1, "-", expr2);
        }

        public static Expression operator *(Expression expr1, Expression expr2)
        {
            return OPR(expr1, "*", expr2);
        }

        public static Expression operator /(Expression expr1, Expression expr2)
        {
            return OPR(expr1, "/", expr2);
        }

        public static Expression operator %(Expression expr1, Expression expr2)
        {
            return OPR(expr1, "%", expr2);
        }


        public static Expression operator ==(Expression expr1, Expression expr2)
        {
            if (expr2 is null || expr2.ToString() == "NULL")
            {
                return new Expression(expr1).Append(" IS NULL");
            }

            return OPR(expr1, "=", expr2);
        }


        public static Expression operator !=(Expression expr1, Expression expr2)
        {
            if (expr2 is null || expr2.ToString() == "NULL")
            {
                return new Expression(expr1).Append(" IS NOT NULL");
            }

            return OPR(expr1, "<>", expr2);
        }

        public static Expression operator >(Expression expr1, Expression expr2)
        {
            return OPR(expr1, ">", expr2);
        }

        public static Expression operator <(Expression expr1, Expression expr2)
        {
            return OPR(expr1, "<", expr2);
        }

        public static Expression operator >=(Expression expr1, Expression expr2)
        {
            return OPR(expr1, ">=", expr2);
        }

        public static Expression operator <=(Expression expr1, Expression expr2)
        {
            return OPR(expr1, "<=", expr2);
        }


        public static Expression operator &(Expression expr1, Expression expr2)
        {
            return OPR(expr1, "AND", expr2);
        }

        public static Expression operator |(Expression expr1, Expression expr2)
        {
            return OPR(expr1, "OR", expr2);
        }

        public static Expression operator ~(Expression expr)
        {
            return OPR("NOT", expr);
        }
        
        #endregion

    }
}
