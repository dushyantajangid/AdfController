namespace AdfController.Model
{
    public class AzureOptions
    {
        public string ResourceGroup { get; set; }
        public string DataFactoryName { get; set; }
        public string TenantId { get; set; }
        public string ApplicationId { get; set; }
        public string AuthenticationKey { get; set; }
        public string SubscriptionId { get; set; }
        public string ActiveDirectoryAuthority { get; set; }
        public string ResourceManagerUrl { get; set; }
    }
}