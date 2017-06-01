namespace SourceWrestlingSchool.Models
{
    /// <summary>Represents a ViewModel to link a membership plan with a user.</summary>
    public class CreateCustomerViewModel
    {
        /// <summary>Gets or sets the id of the membership plan to be started.</summary>
        public int PlanId { get; set; }
        /// <summary>Gets or sets the authenticated user attempting to start a subscription.</summary>
        public ApplicationUser User { get; set; }

    }
}
