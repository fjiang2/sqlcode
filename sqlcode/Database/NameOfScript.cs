using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data
{
    class NameOfScript
    {
        public const string empty = "";
        private readonly string name;

        public NameOfScript(string name)
        {
            this.name = name;
        }

        protected virtual string LeftBracket => "[";
        protected virtual string RightBracket => "]";

        public virtual string DefaultSchemaName => "dbo";

        public bool IsWrapped => name.StartsWith(LeftBracket) && name.EndsWith(RightBracket);


        private static bool Validate(string name)
        {

            int i = 0;
            char ch = name[i++];

            if (!char.IsLetter(ch) && ch != '_')
                return false;

            while (i < name.Length)
            {
                ch = name[i++];

                if (ch != '_' && !char.IsLetterOrDigit(ch))
                    return false;
            }

            return true;
        }

        private string PeelName(string name)
        {
            int a = LeftBracket.Length;
            int b = RightBracket.Length;

            if (IsWrapped)
                return name.Substring(a, name.Length - a - b);
            else
                return name;
        }

        public string ShortName()
        {
            string _name = PeelName(this.name);
            if (Validate(_name))
                return _name;
            else
                return FormalName();
        }

        public string FormalName()
        {
            if (IsWrapped)
                return name;

            return $"{LeftBracket}{name}{RightBracket}";
        }

        public string SchemaName()
        {
            if (name == DefaultSchemaName)
                return empty;

            if (name != empty)
                return ShortName();

            return empty;
        }

        public virtual string ParameterName()
        {
            return "@" + name;
        }

        public override string ToString()
        {
            return name;
        }

    }
}
