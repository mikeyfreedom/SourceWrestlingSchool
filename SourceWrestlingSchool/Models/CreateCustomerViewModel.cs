﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class CreateCustomerViewModel
    {
        public int PlanID { get; set; }
        public ApplicationUser User { get; set; }

    }
}
