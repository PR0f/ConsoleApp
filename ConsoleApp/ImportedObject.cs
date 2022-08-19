using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class ImportedObject : ImportedObjectBaseClass
    {
        public string Name
        {
            get;
            set;
        }
        public string Schema;

        public string ParentName;
        public string ParentType
        {
            get; set;
        }

        public string DataType { get; set; }
        public string IsNullable { get; set; }

        public double NumberOfChildren;
    }
}
