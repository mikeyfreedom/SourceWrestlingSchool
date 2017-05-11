using Braintree;

namespace SourceWrestlingSchool.Models
{
    public class PaymentGateways
    {
        public static BraintreeGateway Gateway = new BraintreeGateway
        {
            Environment = Environment.SANDBOX,
            MerchantId = "f62xpb9hkshcjzqj",
            PublicKey = "2dw68vch4m84fjj7",
            PrivateKey = "cc5e19e5162e76dd739df1025952eebe",            
        };
    }
}
