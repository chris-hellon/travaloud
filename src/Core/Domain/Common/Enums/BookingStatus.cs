namespace Travaloud.Domain.Common.Enums;

public enum BookingStatus
{
    [Display(Name = "Enquiry")]
    Enquiry,
    [Display(Name = "Confirmed - No Deposit")]
    ConfirmedNoDeposit,
    [Display(Name = "Confirmed - Deposit Taken")]
    ConfirmedDepositTaken,
    [Display(Name = "Deposit Pending")]
    DepositPending,
    [Display(Name = "Refund")]
    Refund,
    [Display(Name = "Partial Refund")]
    PartialRefund,
    [Display(Name = "No Show")]
    NoShow,
    [Display(Name = "Cancelled")]
    Cancelled
}