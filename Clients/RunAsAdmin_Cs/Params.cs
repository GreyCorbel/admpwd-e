using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunAsAdmin
{
    public class Params
    {
        public string User { get; set; }
        public string ProgramPath { get; set; }
        public bool NoLocalProfile { get; set; }
    }
}
