using BankingSystem.Controllers;
using BankingSystem.DTOs.Requests;
using BankingSystem.DTOs.Responses;
using BankingSystem.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankingSystem.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly AccountsController _controller;

        public AccountControllerTests()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _controller = new AccountsController(_accountServiceMock.Object);
        }

        [Fact]
        public async Task GetAllAccounts_ShouldReturnOkResult_WithListOfAccountsAsync()
        {
            var accounts = new List<AccountResponse>
                {
                    new AccountResponse { AccountId = 1, AccountHolderName = "Anna Queiroz" },
                    new AccountResponse { AccountId = 2, AccountHolderName = "Laura Santos" }
                };

            _accountServiceMock.Setup(s => s.GetAllAccountsAsync()).ReturnsAsync(accounts);

            var result = await _controller.GetAllAccounts();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(accounts);
        }

        [Fact]
        public async Task GetAccountBalance_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            _accountServiceMock.Setup(s => s.GetAccountAsync(1)).ReturnsAsync((AccountResponse?)null);

            var result = await _controller.GetAccountBalance(1);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAccountBalance_ShouldReturnOk_WhenAccountExists()
        {
            var account = new AccountResponse
            {
                AccountId = 1,
                AccountHolderName = "Anna Queiroz",
                CurrentBalance = 9000
            };

            _accountServiceMock.Setup(s => s.GetAccountAsync(1)).ReturnsAsync(account);

            var result = await _controller.GetAccountBalance(1);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().Be(account.CurrentBalance);
        }

        [Fact]
        public async Task PostAccount_ShouldReturnCreated_WhenAccountIsValid()
        {
            var request = new AccountRequest { AccountHolderName = "Laura Queiroz" };
            _accountServiceMock.Setup(s => s.CreateAccountAsync(request)).ReturnsAsync(new AccountResponse());

            var result = await _controller.Post(request);

            result.Should().BeOfType<CreatedResult>();
        }

        [Fact]
        public async Task PostAccount_ShouldReturnBadRequest_WhenNameIsEmpty()
        {
            var request = new AccountRequest { AccountHolderName = "" };

            var result = await _controller.Post(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateAccount_ShouldReturnNoContent_WhenAccountExists()
        {
            var request = new AccountRequest { AccountHolderName = "Anna Laura Queiroz" };
            _accountServiceMock.Setup(s => s.UpdateAccountAsync(1, request)).ReturnsAsync(true);

            var result = await _controller.PutAccount(1, request);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAccount_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            var request = new AccountRequest { AccountHolderName = "Anna Laura Queiroz" };
            _accountServiceMock.Setup(s => s.UpdateAccountAsync(1, request)).ReturnsAsync((bool?)null);

            var result = await _controller.PutAccount(1, request);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteAccount_ShouldReturnOk_WhenAccountDeleted()
        {
            _accountServiceMock.Setup(s => s.DeleteAccountAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteAccount(1);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteAccount_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            _accountServiceMock.Setup(s => s.DeleteAccountAsync(1)).ReturnsAsync((bool?)null);

            var result = await _controller.DeleteAccount(1);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteAccount_ShouldReturnBadRequest_WhenAccountHasBalance()
        {
            _accountServiceMock.Setup(s => s.DeleteAccountAsync(1)).ReturnsAsync(false);

            var result = await _controller.DeleteAccount(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
