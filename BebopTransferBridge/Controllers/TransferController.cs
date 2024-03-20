namespace BebopTransferBridge.Controllers;

using BebopTransferBridge.Domain.User;
using Domain.Transfer;
using Infrastructure.MessageBroker;
using Infrastructure.MessageBroker.Configuration;
using Infrastructure.MessageBroker.Publisher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[ApiController]
[Route("[controller]")]
public class TransferController(IMessageSender messageSender, IOptions<BebopTransferQueues> exchangeOptions) : ControllerBase
{
    private readonly BebopTransferQueues _exchange = exchangeOptions.Value;

    [HttpPost()]
    public ActionResult Post([FromBody] UserTransferDto user, [FromHeader(Name = "X-Apigateway-Api-Userinfo")] string userInfoHeader)
    {
        if (string.IsNullOrEmpty(userInfoHeader))
        {
            return BadRequest("User Id not found");
        }
        var userInfo = UserInfoAuthDto.DecodeUserInfoAuth(userInfoHeader);
        if (userInfo == null || userInfo.UserId == Guid.Empty)
        {
            return BadRequest("Invalid user info");
        }
        var userTransfer = new UserTransfer
        {
            OperatorId = user.OperatorId,
            UserEmail = userInfo.Email,
            UserId = userInfo.UserId,
        };
        var headers = new Headers(EventTypes.TransferUser.ToString(), userInfo.UserId);
        messageSender.SendMessage(userTransfer, _exchange.TransferUserQueue, headers.GetAttributesAsDictionary());
        return Ok(user);
    }
}
