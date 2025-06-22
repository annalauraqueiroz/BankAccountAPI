using BankingSystem.DTOs.Requests;
using BankingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest depositRequest)
        {
            if (depositRequest.Amount <= 0)
            {
                return BadRequest(new { message = "Deposit amount must be greater than zero." });
            }

            try
            {
                var result = await _accountService.DepositAsync(depositRequest);

                if (!result)
                    return BadRequest(new { message = "Deposit failed. Account not found." });

                return Ok(new { message = "Deposit successful." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Deposit failed: {e.Message}" });
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest withdrawRequest)
        {
            if (withdrawRequest.Amount <= 0)
            {
                return BadRequest(new { message = "Withdraw amount must be greater than zero." });
            }

            try
            {
                var success = await _accountService.WithdrawAsync(withdrawRequest);

                if (!success)
                    return BadRequest(new { message = "Withdraw failed. Check account or balance." });

                return Ok(new { message = "Withdraw successful." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Withdraw failed: {e.Message}" });
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest transferRequest)
        {
            if (transferRequest.Amount <= 0)
            {
                return BadRequest(new { message = "Transfer amount must be greater than zero." });
            }
            if (transferRequest.SourceAccountId == transferRequest.DestinationAccountId)
            {
                return BadRequest(new { message = "Unable to transfer amounts to a destination account that is the same as source account." });
            }
            try
            {
                var success = await _accountService.TransferAsync(transferRequest);

                if (!success)
                    return BadRequest(new { message = "Transfer failed. Check accounts or balance." });

                return Ok(new { message = "Transfer successful." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Transfer failed: {e.Message}" });
            }
        }
    }

}
