﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sys.Data.Entity
{
    class PropertyTranslator : ExpressionVisitor
    {
        private readonly List<string> properties = new List<string>();

        public PropertyTranslator()
        {
        }

        public List<string> Translate(Expression expression)
        {
            if (expression == null)
                return null;

            this.Visit(expression);
            return this.properties;
        }

        protected override Expression VisitMember(MemberExpression expr)
        {
            if (expr.Expression != null)
            {
                switch (expr.Expression.NodeType)
                {
                    case ExpressionType.Parameter:
                        properties.Add(expr.Member.Name);
                        return expr;
                }
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", expr.Member.Name));
        }

    }
}
