namespace BebopTransferBridge.Controllers;

using BebopTransferBridge.Domain.User;
using Domain.Transfer;
using Infrastructure.MessageBroker;
using Infrastructure.MessageBroker.Configuration;
using Infrastructure.MessageBroker.Publisher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("[controller]")]
public class TransferController(IMessageSender messageSender, IOptions<BebopTransferQueues> exchangeOptions) : ControllerBase
{
    private readonly BebopTransferQueues _exchange = exchangeOptions.Value;

    [HttpPost("transfer-user")]
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

    [HttpPost("transfer-user-external")]
    public ActionResult TransferFromExternalOperator([FromBody] TransferFromExternalOperatorDto user)
    {
        var headers = new Headers(EventTypes.NewUserFromTransfer.ToString(), GenerateGuidFromUserIdentificationNumber(user.Id));
        messageSender.SendMessage(user, _exchange.TransferFromExternalAsync, headers.GetAttributesAsDictionary());
        return Ok();
    }

    [HttpPost("transfer-complete")]
    public ActionResult TransferCompleteFromExternalOperator([FromQuery] Guid userId)
    {
        var headers = new Headers(EventTypes.NewUserFromTransfer.ToString(), userId);
        messageSender.SendMessage(new TransferRequestFromExternalOperator { UserId = userId}, _exchange.TransferFromExternalAsync, headers.GetAttributesAsDictionary());
        return Ok();
    }

    [HttpGet("transfer-complete")]
    public ActionResult GetTransferCompleteFromExternalOperator([FromQuery] Guid userId)
    {
        var headers = new Headers(EventTypes.TransferCompleteFromExternalOperator.ToString(), userId);
        messageSender.SendMessage(new TransferRequestFromExternalOperator { UserId = userId }, _exchange.TransferFromExternalAsync, headers.GetAttributesAsDictionary());
        return Ok();
    }

    private static Guid GenerateGuidFromUserIdentificationNumber(long identificationNumber)
    {
        byte[] userIdBytes = Encoding.UTF8.GetBytes(identificationNumber.ToString());
        byte[] hash = SHA1.HashData(userIdBytes);
        hash[6] = (byte)(hash[6] & 0x0F | 0x40);
        hash[8] = (byte)(hash[8] & 0x3F | 0x80);
        byte[] guidBytes = hash.Take(16).ToArray();

        return new Guid(guidBytes);
    }
}
