﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class DownloadTicketViewModel
    {
        public ApplicationUser User { get; set; }
        public TicketViewModel EventDetails { get; set; }
    }
}
