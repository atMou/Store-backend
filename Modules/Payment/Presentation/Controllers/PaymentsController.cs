using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Payment.Application.Features.CancelStripePaymentIntent;
using Payment.Application.Features.ConfirmStripePaymentIntent;
using Payment.Application.Features.CreateStripePaymentIntent;
using Payment.Application.Features.GetPaymentByCartIdDetails;
using Payment.Application.Features.ProcessStripeRefund;
using Payment.Application.Features.StripeWebhook;
using Payment.Presentation.Requests;

using Shared.Application.Contracts.Payment.Results;
using Shared.Presentation.Extensions;

namespace Payment.Presentation.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("{cartId}/create-payment-intent")]
    [Authorize]
    public async Task<ActionResult<CreateStripePaymentIntentResult>> CreatePaymentIntent(
        [FromRoute] Guid cartId)
    {
        var command = new CreateStripePaymentIntentCommand
        {
            CartId = CartId.From(cartId)
        };

        var result = await _sender.Send(command);
        return result.ToActionResult(r => Ok(r), HttpContext.Request.Path);
    }

    [HttpPost("{paymentId}/confirm-payment-intent")]
    [Authorize]
    public async Task<ActionResult<ConfirmStripePaymentIntentResult>> ConfirmPaymentIntent(
        [FromRoute] Guid paymentId)
    {
        var command = new ConfirmStripePaymentIntentCommand
        {
            PaymentId = paymentId
        };

        var result = await _sender.Send(command);
        return result.ToActionResult(r => Ok(r), HttpContext.Request.Path);
    }

    [Authorize]
    [HttpPost("{paymentId}/cancel-payment-intent")]
    public async Task<ActionResult<CancelStripePaymentIntentResult>> CancelPaymentIntent(
        [FromRoute] Guid paymentId)
    {
        var command = new CancelStripePaymentIntentCommand
        {
            PaymentId = paymentId
        };

        var result = await _sender.Send(command);
        return result.ToActionResult(r => Ok(r), HttpContext.Request.Path);
    }

    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult<PaymentResult>> GetByCartId(
        [FromRoute] Guid paymentId)
    {
        var command = new GetPaymentByCartIdDetailsCommand()
        {
            CartId = CartId.From(paymentId)
        };

        var result = await _sender.Send(command);
        return result.ToActionResult(r => Ok(r), HttpContext.Request.Path);
    }


    [Authorize]
    [HttpPost("{paymentId}/stripe/refund")]
    public async Task<ActionResult<ProcessStripeRefundResult>> ProcessRefund(
        [FromRoute] Guid paymentId,
        [FromBody] ProcessStripeRefundRequest request)
    {
        var command = new ProcessStripeRefundCommand
        {
            PaymentId = PaymentId.From(paymentId),
            RefundAmount = request.RefundAmount
        };

        var result = await _sender.Send(command);
        return result.ToActionResult(r => Ok(r), HttpContext.Request.Path);
    }

    /// <summary>
    /// Stripe webhook endpoint for payment events
    /// </summary>
    [HttpPost("stripe/webhook")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].ToString();

        var command = new HandleStripeWebhookCommand
        {
            Payload = json,
            Signature = signature
        };

        var result = await _sender.Send(command);
        return result.Match(
            Succ: _ => Ok(),
            Fail: error => error.Code == 400
                ? BadRequest(new ProblemDetails { Detail = error.Message }) as IActionResult
                : StatusCode(500, new ProblemDetails { Detail = error.Message })
        );
    }
}
