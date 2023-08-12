using Microsoft.Extensions.Configuration;
using Cowbot.Server.Configuration;
using Stripe;

namespace Cowbot.Server.ThirdPartyServices.Stripe;

public class StripePaymentPageService
{
    private readonly IConfiguration _configuration;
    private readonly string _stripeApiKey;

    public StripePaymentPageService(IConfiguration configuration)
    {
        _configuration = configuration;
        _stripeApiKey = _configuration.StripePaymentLinksApiKey();
        StripeConfiguration.ApiKey = _stripeApiKey;
    }

    public Price CreatePrice(long amount, string currency, string? productId, string? productName)
    {
        var options = new PriceCreateOptions
        {
            UnitAmount = amount,
            Currency = currency,
            Product = productId ?? null,
            ProductData = productName != null ? new PriceProductDataOptions {
                Name = productName
            } : null
        };
        var service = new PriceService();
        return service.Create(options);
    }

    public Product CreateProduct(string name)
    {
        var options = new ProductCreateOptions
        {
            Name = name,
        };
        var service = new ProductService();
        return service.Create(options);
    }

    public PaymentLink CreateHostedPaymentPage()
    {
        var options = new PaymentLinkCreateOptions
        {
            LineItems = new List<PaymentLinkLineItemOptions>
            {
                new PaymentLinkLineItemOptions
                {
                    Price = "price_1LUBqV2eZvKYlo2CDhAiHku6",
                    Quantity = 1,
                },
            },
        };
        var service = new PaymentLinkService();
        return service.Create(options);
    }
}