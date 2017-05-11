using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    public class RevenueReportModel
    {
        [DisplayFormat(DataFormatString = "{0:Y}")]
        public DateTime CurrentDate { get; set; }
        public List<LiveEvent> Events { get; set; }
        public int NoBronzeMemberships { get; set; }
        public int NoSilverMemberships { get; set; }
        public int NoGoldMemberships { get; set; }
        public float BronzeFeesCollected { get; set; }
        public float SilverFeesCollected { get; set; }
        public float GoldFeesCollected { get; set; }
        public int NoPrivateSessions { get; set; }
        public double PrivateFeesCollected { get; set; }
        public int NoCancellationFines { get; set; }
        public double FinesCollected { get; set; }
        public float TotalEventRevenue { get; set; }
        public float TotalMembershipRevenue { get; set; }
        public double TotalOtherRevenue { get; set; }
        public double TotalMonthRevenue { get; set; }
    }
}
