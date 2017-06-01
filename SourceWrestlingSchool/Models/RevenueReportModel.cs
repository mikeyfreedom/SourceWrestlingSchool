using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    /// <summary>Represents a data model of a monthly revenue report.</summary>
    public class RevenueReportModel
    {
        /// <summary>Gets or sets the date the report deals with.</summary>
        [DisplayFormat(DataFormatString = "{0:Y}")]
        public DateTime CurrentDate { get; set; }
        /// <summary>Gets or sets a list of live events that took place during the specified month.</summary>
        public List<LiveEvent> Events { get; set; }
        /// <summary>Gets or sets the number of bronze plan subscriptions pair for during the month.</summary>
        public int NoBronzeMemberships { get; set; }
        /// <summary>Gets or sets the number of silver plan subscriptions pair for during the month.</summary>
        public int NoSilverMemberships { get; set; }
        /// <summary>Gets or sets the number of gold plan subscriptions pair for during the month.</summary>
        public int NoGoldMemberships { get; set; }
        /// <summary>Gets or sets the total of bronze membership fees collected during the month.</summary>
        public float BronzeFeesCollected { get; set; }
        /// <summary>Gets or sets the total of silver membership fees collected during the month.</summary>
        public float SilverFeesCollected { get; set; }
        /// <summary>Gets or sets the total of gold membership fees collected during the month.</summary>
        public float GoldFeesCollected { get; set; }
        /// <summary>Gets or sets the number of private lessons that were authorised during the month.</summary>
        public int NoPrivateSessions { get; set; }
        /// <summary>Gets or sets the total of private lesson fees collected during the month.</summary>
        public double PrivateFeesCollected { get; set; }
        /// <summary>Gets or sets the number of fines levied due to late class cancellations.</summary>
        public int NoCancellationFines { get; set; }
        /// <summary>Gets or sets the total amount of fines collected during the report month.</summary>
        public double FinesCollected { get; set; }
        /// <summary>Gets or sets the total revenue from live events during the report month.</summary>
        public float TotalEventRevenue { get; set; }
        /// <summary>Gets or sets the total revenue from membership fees during the report month.</summary>
        public float TotalMembershipRevenue { get; set; }
        /// <summary>Gets or sets the total revenue from other sources(i.e. fines) during the report month.</summary>
        public double TotalOtherRevenue { get; set; }
        /// <summary>Gets or sets the total revenue for the month.</summary>
        public double TotalMonthRevenue { get; set; }
    }
}
