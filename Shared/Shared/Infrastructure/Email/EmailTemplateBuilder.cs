using System.Text;

using LanguageExt.Sys.Traits;

using Microsoft.Extensions.Logging;

using Shared.Application.Contracts.Order.Dtos;

namespace Shared.Infrastructure.Email;

public interface IEmailTemplateBuilder
{
    IO<string> BuildOrderShippedEmailAsync(
        string userName,
        Guid orderId,
        string trackingCode,
        IEnumerable<OrderItemDto> orderItems,
        decimal subtotal,
        decimal tax,
        decimal total,
        DateTime orderDate,
        FileIO fileIO);

    IO<string> BuildEmailVerificationAsync(
        string userName,
        string email,
        string verificationCode,
        string verificationLink,
        FileIO fileIO);
}

public class EmailTemplateBuilder : IEmailTemplateBuilder
{
    private readonly ILogger<EmailTemplateBuilder> _logger;

    public EmailTemplateBuilder(ILogger<EmailTemplateBuilder> logger)
    {
        _logger = logger;
    }

    public IO<string> BuildOrderShippedEmailAsync(
        string userName,
        Guid orderId,
        string trackingCode,
        IEnumerable<OrderItemDto> orderItems,
        decimal subtotal,
        decimal tax,
        decimal total,
        DateTime orderDate,
        FileIO fileIO)
    {
        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Templates",
            "OrderShipped.html"
        );

        return from template in fileIO.Exists(path)
                .Bind(exists =>
                    exists
                        ? fileIO.ReadAllText(path, Encoding.UTF8)
                        : IO.fail<string>(Error.New("Order shipped template file does not exist")))
               let orderItemsHtml = BuildOrderItemsHtml(orderItems)
               let emailBody = template
                   .Replace("{{UserName}}", userName)
                   .Replace("{{OrderId}}", orderId.ToString()[..8])
                   .Replace("{{TrackingCode}}", trackingCode)
                   .Replace("{{OrderDate}}", orderDate.ToString("MMMM dd, yyyy"))
                   .Replace("{{OrderItems}}", orderItemsHtml)
                   .Replace("{{Subtotal}}", subtotal.ToString("F2"))
                   .Replace("{{Tax}}", tax.ToString("F2"))
                   .Replace("{{Total}}", total.ToString("F2"))
                   .Replace("{{TrackingLink}}", $"https://yourstore.com/track/{trackingCode}")
                   .Replace("{{Year}}", DateTime.UtcNow.Year.ToString())
               select emailBody;
    }

    public IO<string> BuildEmailVerificationAsync(
        string userName,
        string email,
        string verificationCode,
        string verificationLink,
        FileIO fileIO)
    {
        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Templates",
            "EmailVerification.html"
        );

        return from template in fileIO.Exists(path)
                .Bind(exists =>
                    exists
                        ? fileIO.ReadAllText(path, Encoding.UTF8)
                        : IO.fail<string>(Error.New("Email verification template file does not exist")))
               let emailBody = template
                   .Replace("{{VerificationLink}}", verificationLink)
                   .Replace("{{UserEmail}}", email)
                   .Replace("{{code}}", verificationCode)
                   .Replace("{{name}}", userName)
                   .Replace("{{Year}}", DateTime.UtcNow.Year.ToString())
               select emailBody;
    }

    private static string BuildOrderItemsHtml(IEnumerable<OrderItemDto> orderItems)
    {
        var sb = new StringBuilder();
        var result = orderItems.Aggregate(sb, (builder, dto) => builder.AppendLine($@"
                <div class=""order-item"">
                    <img src=""{dto.ImageUrl}"" alt=""{dto.Sku}"" class=""item-image"">
                    <div class=""item-details"">
                        <div class=""item-sku"">{dto.Sku}</div>
                        <div class=""item-variant"">Size: {dto.Size} | Quantity: {dto.Quantity}</div>
                    </div>
                    <div class=""item-price"">${dto.LineTotal:F2}</div>
                </div>"));

        return result.ToString();

    }


}
