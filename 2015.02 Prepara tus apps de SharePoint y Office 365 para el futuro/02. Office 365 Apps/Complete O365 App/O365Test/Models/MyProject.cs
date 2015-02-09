using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace O365Test.Models
{
    public class MyProject
    {
        public string Title { get; set; }
        public string Owner { get; set; }

        [DisplayName("Statement Document")]
        public string DocumentName { get; set; }
        public string DocumentLink { get; set; }

    }
}