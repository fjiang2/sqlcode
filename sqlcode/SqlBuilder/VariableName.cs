using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Text
{
	public sealed class VariableName : IComparable, IComparable<string>, IEquatable<VariableName>
	{
		private readonly string id;

		public VariableName(string name)
		{
			this.id = name;
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return id.Equals(((VariableName)obj).id);
		}

		public bool Equals(VariableName obj)
		{
			return id.Equals(obj.id);
		}

		public override string ToString()
		{
			return this.id;
		}

		public static bool operator ==(VariableName vname1, VariableName vname2)
		{
			return vname1.id.Equals(vname2.id);
		}

		public static bool operator !=(VariableName vname1, VariableName vname2)
		{
			return !(vname1 == vname2);
		}

		public static implicit operator VariableName(string name)
		{
			return new VariableName(name);
		}

		public static explicit operator string(VariableName vname)
		{
			return vname.id;
		}

		public int CompareTo(object obj)
		{
			return this.id.CompareTo(obj);
		}

		public int CompareTo(string other)
		{
			return this.id.CompareTo(other);
		}

		public static explicit operator Expression(VariableName variableName)
		{
			return new Expression(variableName);
		}
	}
}

