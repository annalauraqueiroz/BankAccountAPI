using BankingSystem.DTOs.Requests;
using BankingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();

                return Ok(accounts);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Failed to get accounts: {e.Message}" });
            }
        }
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccount(int accountId)
        {
            try
            {
                var account = await _accountService.GetAccountAsync(accountId);

                if (account is null)
                    return NotFound(new { message = "Account not found." });

                return Ok(account);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Failed to get account: {e.Message}" });
            }
        }

        [HttpGet("{accountId}/statement")]
        public async Task<IActionResult> GetAccountStatement(int accountId)
        {
            try
            {
                var statement = await _accountService.GetStatementAsync(accountId);

                if (statement is null)
                    return NotFound(new { message = "Account not found." });

                return Ok(statement);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Failed to get statement: {e.Message}" });
            }
        }


        [HttpGet("{accountId}/balance")]
        public async Task<IActionResult> GetAccountBalance(int accountId)
        {
            try
            {
                var account = await _accountService.GetAccountAsync(accountId);

                if (account is null)
                    return NotFound(new { message = "Account not found." });

                return Ok(account.CurrentBalance);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Failed to get account balance: {e.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(AccountRequest accountRequest)
        {
            if (string.IsNullOrEmpty(accountRequest.AccountHolderName))
                return BadRequest(new { message = "Account holder name is required." });

            try
            {
                var account = await _accountService.CreateAccountAsync(accountRequest);

                return Created();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Failed to save new account: {e.Message}" });
            }
        }

        [HttpPut("{accountId}")]
        public async Task<IActionResult> PutAccount(int accountId, [FromBody] AccountRequest request)
        {
            if (string.IsNullOrEmpty(request.AccountHolderName))
                return BadRequest(new { message = "Account holder name is required." });

            try
            {
                var account = await _accountService.UpdateAccountAsync(accountId, request);

                if (account is null)
                    return NotFound(new { message = "Account not found." });

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Failed to update account: {e.Message}" });
            }
        }

        [HttpDelete("{accountId}")]
        public async Task<IActionResult> DeleteAccount(int accountId)
        {
            try
            {
                bool? deleteAccount = await _accountService.DeleteAccountAsync(accountId);

                if (deleteAccount is null)
                    return NotFound(new { message = "Account not found." });
                else if (deleteAccount is false)
                    return BadRequest(new { message = "Account balance must be zero before deleting the account." });

                return Ok(new { message = "Account deleted successfully." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Failed to delete account: {e.Message}" });
            }
        }
    }
}
