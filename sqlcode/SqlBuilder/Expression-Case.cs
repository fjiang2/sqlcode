using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Data.Text
{
    public partial class Expression
    {
        public Expression CASE(Expression _case)
        {
            var expr = new Expression(this)
                .AppendSpace("CASE")
                .Append(_case);

            return expr;
        }


        public Expression WHEN(Expression condition)
        {
            return new Expression(this).WrapSpace("WHEN").Append(condition);
        }

        public Expression THEN(Expression then)
        {
            return new Expression(this).WrapSpace("THEN").Append(then);
        }

        public Expression ELSE(Expression _else)
        {
            return new Expression(this).WrapSpace("ELSE").Append(_else).AppendSpace();
        }

        public Expression END()
        {
            return new Expression(this).Append("END");
        }

        public static Expression WHEN(Expression condition, Expression then)
        {
            return new Expression("WHEN").AppendSpace().Append(condition).WrapSpace("THEN").Append(then);
        }

        public static Expression CASE(Expression _case, Expression[] whens, Expression _else)
        {
            var expr = new Expression("CASE")
                .AppendSpace()
                .Append(_case)
                .AppendSpace();

            foreach (var when in whens)
            {
                expr.Append(when).AppendSpace();
            }

            if (_else is object)
                expr.AppendSpace("ELSE").Append(_else).AppendSpace();

            expr.Append("END");

            return expr;
        }
        public static Expression CASE(Expression[] whens, Expression _else)
        {
            var expr = new Expression("CASE");

            foreach (var when in whens)
            {
                expr.Append(when).AppendSpace();
            }

            if (_else is object)
                expr.AppendSpace("ELSE").Append(_else).AppendSpace();

            expr.Append("END");

            return expr;
        }
    }
}
