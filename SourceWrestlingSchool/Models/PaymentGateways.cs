using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class PaymentGateways
    {
        public static BraintreeGateway Gateway = new BraintreeGateway
        {
            Environment = Braintree.Environment.SANDBOX,
            MerchantId = "f62xpb9hkshcjzqj",
            PublicKey = "2dw68vch4m84fjj7",
            PrivateKey = "cc5e19e5162e76dd739df1025952eebe",            
        };
    }
}
