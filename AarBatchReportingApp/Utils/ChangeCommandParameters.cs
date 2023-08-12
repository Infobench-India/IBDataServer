using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AarBatchReportingApp.Utils
{
    public class ChangeCommandParameters
    {
        public string Password { get; set; }
        public string ConfirmPassword  { get; set; }

        public string OldPassword { get; set; }
    }
}
