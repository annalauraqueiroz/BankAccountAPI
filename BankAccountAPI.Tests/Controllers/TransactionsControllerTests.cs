using BankingSystem.Controllers;
using BankingSystem.Domain.Constants;
using BankingSystem.Domain.Enums;
using BankingSystem.DTOs.Requests;
using BankingSystem.DTOs.Responses;
using BankingSystem.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankingSystem.Tests.Controllers
{
    public class TransactionsControllerTests
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly TransactionsController _controller;
        private readonly AccountsController _accountsController;

        public TransactionsControllerTests()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _controller = new TransactionsController(_accountServiceMock.Object);
            _accountsController = new AccountsController(_accountServiceMock.Object);
        }

        #region Deposit tests
        [Fact]
        public async Task Deposit_ShouldReturnBadRequest_WhenAmountIsZeroOrNegative()
        {
            var request = new DepositRequest(accountId: 1, amount: 0);

            var result = await _controller.Deposit(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Deposit_ShouldReturnBadRequest_WhenAccountDoesNotExist()
        {
            var request = new DepositRequest(accountId: 1, amount: 1000);
            _accountServiceMock.Setup(x => x.DepositAsync(request)).ReturnsAsync(false);

            var result = await _controller.Deposit(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Deposit_ShouldReturnOk_WhenSuccessful()
        {
            var request = new DepositRequest(accountId: 1, amount: 1000);
            _accountServiceMock.Setup(x => x.DepositAsync(request)).ReturnsAsync(true);

            var result = await _controller.Deposit(request);

            result.Should().BeOfType<OkObjectResult>();
        }
        [Theory]
        [InlineData(10000)]
        [InlineData(5000)]
        [InlineData(200000)]
        public async Task Deposit_ShouldApplyTransactionPercentFee(long amount)
        {
            var account = new AccountResponse { AccountId = 1 };

            _accountServiceMock.Setup(x => x.GetAccountAsync(1)).ReturnsAsync(account);

            _accountServiceMock
                .Setup(x => x.DepositAsync(It.IsAny<DepositRequest>()))
                .ReturnsAsync(true);

            _accountServiceMock
                .Setup(x => x.GetStatementAsync(1))
                .ReturnsAsync(new List<TransactionResponse>
                {
                    new TransactionResponse
                    {
                        AccountId = 1,
                        Amount = amount,
                        Type = TransactionType.Credit,
                        Category = TransactionCategory.Deposit,
                        Description = "Deposit",
                        Date = DateTime.Now
                    },
                    new() {
                        AccountId = 1,
                        Amount = (long)Math.Ceiling(amount * TransactionFees.DepositPercentage),
                        Type = TransactionType.Debit,
                        Category = TransactionCategory.Fee,
                        Description = "Deposit fee",
                        Date = DateTime.Now
                    }
                });

            var request = new DepositRequest(1, amount);

            var result = await _controller.Deposit(request);

            var transactionsResult = await _accountsController.GetAccountStatement(1) as OkObjectResult;
            var transactions = transactionsResult!.Value as List<TransactionResponse>;

            var totalDeposits = transactions!
                .Where(t => t.Type == TransactionType.Credit)
                .Sum(t => t.Amount);

            var totalFees = transactions!
                .Where(t => t.Category == TransactionCategory.Fee)
                .Sum(t => t.Amount);

            totalDeposits.Should().Be(amount);
            totalFees.Should().Be((long)Math.Ceiling(amount * TransactionFees.DepositPercentage));
            result.Should().BeOfType<OkObjectResult>();
        }

        #endregion

        #region Withdraw Tests
        [Fact]
        public async Task Withdraw_ShouldReturnBadRequest_WhenAmountIsZeroOrNegative()
        {
            var request = new WithdrawRequest(accountId: 1, amount: 0);

            var result = await _controller.Withdraw(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Withdraw_ShouldReturnBadRequest_WhenFailDueToBalanceOrAccount()
        {
            var request = new WithdrawRequest(accountId: 1, amount: 1000);
            _accountServiceMock.Setup(x => x.WithdrawAsync(request)).ReturnsAsync(false);

            var result = await _controller.Withdraw(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Withdraw_ShouldReturnOk_WhenSuccessful()
        {
            var request = new WithdrawRequest(accountId: 1, amount: 1000);
            _accountServiceMock.Setup(x => x.WithdrawAsync(request)).ReturnsAsync(true);

            var result = await _controller.Withdraw(request);

            result.Should().BeOfType<OkObjectResult>();
        }
        [Theory]
        [InlineData(10000, 10000)]
        [InlineData(5000, 4601)]
        [InlineData(200000, 199601)]
        public async Task Withdraw_ShouldReturnBadRequest_WhenBalanceIsNotEnough(long currentBalance, long withdrawAmount)
        {
            var account = new AccountResponse { AccountId = 1, CurrentBalance = currentBalance };

            _accountServiceMock.Setup(x => x.GetAccountAsync(1)).ReturnsAsync(account);

            _accountServiceMock
                .Setup(x => x.WithdrawAsync(It.IsAny<WithdrawRequest>()))
                .ReturnsAsync((WithdrawRequest request) =>
                {
                    var accountBalance = account.CurrentBalance;
                    long totalFee = TransactionFees.WithdrawFixed;
                    long totalAmount = request.Amount + totalFee;

                    return accountBalance >= totalAmount;
                });

            var request = new WithdrawRequest(1, withdrawAmount);

            var result = await _controller.Withdraw(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData(10000, 10000)]
        [InlineData(5000, 4601)]
        [InlineData(200000, 199900)]
        public async Task Withdraw_ShouldReturnBadRequest_WhenBalanceDoesNotCoverAmountPlusFee(long currentBalance, long withdrawAmount)
        {
            var account = new AccountResponse { AccountId = 1, CurrentBalance = currentBalance };

            _accountServiceMock.Setup(x => x.GetAccountAsync(1)).ReturnsAsync(account);

            _accountServiceMock
                .Setup(x => x.WithdrawAsync(It.IsAny<WithdrawRequest>()))
                .ReturnsAsync((WithdrawRequest request) =>
                {
                    var accountBalance = account.CurrentBalance;
                    long totalFee = TransactionFees.WithdrawFixed;
                    long totalAmount = request.Amount + totalFee;

                    return accountBalance >= totalAmount;
                });

            var request = new WithdrawRequest(1, withdrawAmount);

            var result = await _controller.Withdraw(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData(10000, 9600)]
        [InlineData(5000, 2500)]
        [InlineData(200000, 10)]
        public async Task Withdraw_ShouldReturnOk_WhenBalanceCoversAmountPlusFee(long currentBalance, long withdrawAmount)
        {
            var account = new AccountResponse { AccountId = 1, CurrentBalance = currentBalance };

            _accountServiceMock
                .Setup(x => x.WithdrawAsync(It.IsAny<WithdrawRequest>()))
                .ReturnsAsync((WithdrawRequest request) =>
                {
                    var accountBalance = account.CurrentBalance;
                    long totalFee = TransactionFees.WithdrawFixed;
                    long totalAmount = request.Amount + totalFee;

                    return accountBalance >= totalAmount;
                });

            var request = new WithdrawRequest(1, withdrawAmount);

            var result = await _controller.Withdraw(request);

            result.Should().BeOfType<OkObjectResult>();
        }
        #endregion

        #region Transfer Tests
        [Fact]
        public async Task Transfer_ShouldReturnBadRequest_WhenAmountIsZeroOrNegative()
        {
            var request = new TransferRequest
            {
                SourceAccountId = 1,
                DestinationAccountId = 2,
                Amount = 0
            };

            var result = await _controller.Transfer(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Transfer_ShouldReturnBadRequest_WhenSourceAndDestinationAreSame()
        {
            var request = new TransferRequest
            {
                SourceAccountId = 1,
                DestinationAccountId = 1,
                Amount = 1000
            };

            var result = await _controller.Transfer(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Transfer_ShouldReturnBadRequest_WhenFailDueToBalanceOrAccount()
        {
            var request = new TransferRequest
            {
                SourceAccountId = 1,
                DestinationAccountId = 2,
                Amount = 1000
            };

            _accountServiceMock.Setup(x => x.TransferAsync(request)).ReturnsAsync(false);

            var result = await _controller.Transfer(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Transfer_ShouldReturnOk_WhenSuccessful()
        {
            var request = new TransferRequest
            {
                SourceAccountId = 1,
                DestinationAccountId = 2,
                Amount = 1000
            };

            _accountServiceMock.Setup(x => x.TransferAsync(request)).ReturnsAsync(true);

            var result = await _controller.Transfer(request);

            result.Should().BeOfType<OkObjectResult>();
        }
        [Theory]
        [InlineData(1, null)]
        [InlineData(null, 4)]
        public async Task Transfer_ShouldReturnBadRequest_WhenSourceOrDestinationAccountDoesNotExist(int? sourceAccountId, int? destinationAccountId)
        {
            var sourceAccount = sourceAccountId.HasValue ? new AccountResponse { AccountId = sourceAccountId.Value } : null;
            var destinationAccount = destinationAccountId.HasValue ? new AccountResponse { AccountId = destinationAccountId.Value } : null;

            _accountServiceMock.Setup(x => x.GetAccountAsync(1)).ReturnsAsync(sourceAccount);
            _accountServiceMock.Setup(x => x.GetAccountAsync(2)).ReturnsAsync(destinationAccount);

            var request = new TransferRequest
            {
                SourceAccountId = 1,
                DestinationAccountId = 2,
                Amount = 100
            };

            _accountServiceMock
                .Setup(x => x.TransferAsync(It.IsAny<TransferRequest>()))
                .ReturnsAsync((TransferRequest req) =>
                {
                    if (sourceAccount is null || destinationAccount is null)
                        return false;

                    return true;
                });

            var result = await _controller.Transfer(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Theory]
        [InlineData(500, 401)]
        [InlineData(5000, 5000)]
        [InlineData(200000, 19999000)]
        public async Task Transfer_ShouldReturnBadRequest_WhenBalanceIsNotEnoughIncludingFee(long currentBalance, long transferAmount)
        {
            var sourceAccount = new AccountResponse { AccountId = 1, CurrentBalance = currentBalance };
            var destinationAccount = new AccountResponse { AccountId = 2 };

            _accountServiceMock.Setup(x => x.GetAccountAsync(1)).ReturnsAsync(sourceAccount);
            _accountServiceMock.Setup(x => x.GetAccountAsync(2)).ReturnsAsync(destinationAccount);

            _accountServiceMock.Setup(x => x.GetAccountAsync(1)).ReturnsAsync(sourceAccount);

            _accountServiceMock
                .Setup(x => x.TransferAsync(It.IsAny<TransferRequest>()))
                .ReturnsAsync((TransferRequest request) =>
                {
                    var accountBalance = sourceAccount.CurrentBalance;
                    long totalFee = TransactionFees.TransferFixed;
                    long totalAmount = request.Amount + totalFee;

                    return accountBalance >= totalAmount;
                });

            var request = new TransferRequest
            {
                SourceAccountId = 1,
                DestinationAccountId = 2,
                Amount = transferAmount
            };

            var result = await _controller.Transfer(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion
    }
}
