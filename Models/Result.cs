using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalPortal.Models
{
    public class Result
    {
        public int RowsEffected { get; set; }
        public string ReturnVal { get; set; }
        public int Sucessful { get; set; }
        public string ShortMsg { get; set; }
        public Exception exception { get; set; }
    }
}
