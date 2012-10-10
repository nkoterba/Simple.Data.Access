using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.Access.Test
{
    class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LongText { get; set; }
        public int Age { get; set; }
        public DateTime DateTime { get; set; }

        
        //public bool YesNo { get; set; }
        public decimal Currency { get; set; }

        // Should be array of bytes
        public string OleObject { get; set; }
        public string Hyperlink { get; set; }
        public byte Byte { get; set; }
        public Int16 Integer { get; set; }
        public int LongInteger { get; set; }
        public Single Single { get; set; }
        public double Double { get; set; }

        // Shoudl be a Guid
        public string ReplicationID { get; set; }
        public decimal Decimal { get; set; }
    }
}
