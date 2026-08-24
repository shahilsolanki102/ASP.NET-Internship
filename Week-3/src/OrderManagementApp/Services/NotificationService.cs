namespace OrderManagementApp.Services
{
    public interface INotificationService
    {
        Task<bool> SendOrderConfirmationEmailAsync(string customerEmail, string orderNumber, decimal totalAmount);
    }

    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendOrderConfirmationEmailAsync(string customerEmail, string orderNumber, decimal totalAmount)
        {
            _logger.LogInformation("Order confirmation email sent to {Email} for Order #{OrderNumber}, Total: ${Total}", 
                customerEmail, orderNumber, totalAmount);
            return Task.FromResult(true);
        }
    }
}
