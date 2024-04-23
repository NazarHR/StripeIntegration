namespace StripeItegration.Models
{
    public class PaymentSessionExternalParametersModel
    {
        public required string Product_Id { get; set; }
        public required string SuccessReturnUrl { get; set; }
    }
}
