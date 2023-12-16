using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string PaymentStatusPending = "PaymentStatusPending";
		public const string PaymentStatusDelayedPayment = "PaymentStatusDelayedPayment";
        public const string PaymentStatusApproved = "PaymentStatusApproved";

		public const string StatusPending = "StatusPending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "StatusInProcess";
        public const string StatusShipped = "StatusShipped";

        public const string StatusRefunded = "StatusRefunded";
        public const string StatusCancelled = "StatusCancelled";

        public const string SessionCart = "SessionCart";


    }
}
