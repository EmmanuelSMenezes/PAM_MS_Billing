namespace Domain.Model
{
    public class MSBillingSettings
    {
        public string ConnectionString { get; set; }

        public string PrivateSecretKey { get; set; }

        public string TokenValidationMinutes { get; set; }
        public string OtpValidationMinutes { get; set; }
        public PagSeguro PagSeguro { get; set; }
    }
}