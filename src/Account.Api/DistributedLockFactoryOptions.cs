namespace Account.Api
{
    public sealed class DistributedLockFactoryOptions
    {
        public double ExpiryTime { get; set; }
        public double WaitTime { get; set; }
        public double RetryTime { get; set; }
    }
}