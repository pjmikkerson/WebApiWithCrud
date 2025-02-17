namespace WebApiWithCrud.Services
{
    public class DummyService(ILogger<DummyService> logger) : IDummyService
    {
        public void DoSomething() {
            logger.LogInformation("something is happening!");
            logger.LogCritical("oops");
            logger.LogDebug("nothing much");
        }
    }
}
