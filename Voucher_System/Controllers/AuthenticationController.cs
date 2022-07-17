using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Voucher_System.Models;
using Voucher_System.Services.Interfaces;
using Voucher_System.ViewModels;

namespace Voucher_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;
        private readonly ITokenService _tokenService;

        public AuthenticationController(ApplicationContext applicationContext, ITokenService tokenService)
        {
            _applicationContext = applicationContext;
            _tokenService = tokenService;
        }

        [HttpPost("customer/register")]

        // To receive data from the body it should be an object
        public async Task<ActionResult<SignInAndSignUpResponseDTO>> Register([FromBody] CustomerRegisterDTO registerDTO)
        {

            if (await UserExists(registerDTO.Email)) return BadRequest("Name is Taken");

            using var hmac = new HMACSHA512();

            var customer = new Customer()
            {
                Id = Guid.NewGuid(),
                dateOfBirth = registerDTO.dateOfBirth,
                Created = DateTime.UtcNow,
                Email = registerDTO.Email,
                lastActive = DateTime.UtcNow,
                Gender = registerDTO.gender,
                userName = registerDTO.userName.ToLower(),
                passwordHash = hmac.ComputeHash(Encoding.UTF32.GetBytes(registerDTO.password)),
                passwordSalt = hmac.Key
            };

            _applicationContext.Customers.Add(customer);
            await _applicationContext.SaveChangesAsync();

            return new SignInAndSignUpResponseDTO
            {
                userName = customer.userName,
                token = _tokenService.creatToken(customer.userName)
            };
        }

        [HttpPost("test")]
        public async Task<IActionResult> Test([FromBody] test test)
        {
            foreach (var amount in test.amounts)
            {
                await AddTransactionTest(1, 1, amount, 0, 1, "USD");
            }

            return Ok();
        }



        [HttpPost("customer/login")]
        public async Task<ActionResult<SignInAndSignUpResponseDTO>> Login([FromBody] CustomerLogin loginDTO)
        {

            var customer = await _applicationContext.Customers
                     .SingleOrDefaultAsync(u => u.Email == loginDTO.email);

            if (customer == null) return Unauthorized("Invalid user credentials");

            using var hmc = new HMACSHA512(customer.passwordSalt);

            var ComputedHash = hmc.ComputeHash(Encoding.UTF32.GetBytes(loginDTO.password));

            for (int i = 0; i < ComputedHash.Length; i++)
            {
                if (ComputedHash[i] != customer.passwordHash[i]) return Unauthorized("Invalid password!");
            }

            return new SignInAndSignUpResponseDTO
            {
                userName = customer.userName,
                token = _tokenService.creatToken(customer.userName)
            };

        }
        private async Task<bool> UserExists(string email)
        {
            return await _applicationContext.Customers.AnyAsync(u => u.Email == email.ToLower());
        }
        public async Task<bool> AddTransaction(int userId, double usdExchangeRate, double credit, double commissionPrecentage,
             double productSellPrice, double debit, int opsId, string Currency,
             int? BouquetId = null, int? CreatedBy = null,
             TransactionExtended transactionExtended = null)
        {
            var lastBalance = await GetUserBalanceInUSD(userId);
            var creditUSD = credit * usdExchangeRate;
            var debitUSD = debit * usdExchangeRate;
            var currentBalance = lastBalance + creditUSD - debitUSD;

            Transaction transaction = new Transaction
            {
                UserId = userId,
                BouquetId = BouquetId ?? 0,
                CreatedBy = CreatedBy,
                Type = credit > 0 ? CommunityTransactionType.Credit : CommunityTransactionType.Debit,
                Credit = credit,
                Debit = debit,
                Currency = Currency,
                ExchangeRate = usdExchangeRate,
                DebitUsd = debitUSD,
                CreditUsd = creditUSD,
                BalanceUsd = currentBalance,
                CreateDate = DateTime.UtcNow,
                Note = "",
                TransactionExtended = transactionExtended,
                Status = TransactionStatus.Confirmed,
                ProductSellPrice = productSellPrice,
                ProductSellPriceUSD = usdExchangeRate * productSellPrice,
                CommissionPrecentage = commissionPrecentage,
            };
            var trans = await Add(transaction);

            return true;
        }

        public async Task<UserCommissionDataResponseDTO> GetUserCommissionAndAddDifferenceTransaction(int userId, double usdExchangeRate,
            double productSellPrice, double debit, int opsId, string Currency,
            int? bouquetId = null, int? createdBy = null,
            TransactionExtended transactionExtended = null)
        {
            var productSellPriceUSD = productSellPrice * usdExchangeRate;
            var debitUSD = debit * usdExchangeRate;

            var userSellesForthisMonth = await GetUserProductSellsForThisMonth(userId, DateTime.UtcNow.Month);
            var userSellesForThisMonthUSD = userSellesForthisMonth * usdExchangeRate;

            double userCommissionValue = 0;
            double userCommissionPrecentage = 0;
            var userBalanceUSD = await GetUserBalanceInUSD(userId);

            var lastTransactionPrecentageValue = await GetLastTransactionCommissionPrecentage(userId);


            var differenceTransaction = new Transaction
            {
                UserId = userId,
                BouquetId = bouquetId ?? 0,
                CreatedBy = createdBy,
                Type = CommunityTransactionType.difference,
                Debit = debit,
                Currency = Currency,
                ExchangeRate = usdExchangeRate,
                DebitUsd = debitUSD,
                CreateDate = DateTime.UtcNow,
                Note = "",
                TransactionExtended = transactionExtended,
                Status = TransactionStatus.Confirmed,
                ProductSellPriceUSD = 0,
                ProductSellPrice = 0
            };

            var totalSellAmountInUSD = (userSellesForThisMonthUSD + productSellPriceUSD);
            if (totalSellAmountInUSD <= 1000)
            {
                userCommissionValue = 0.01 * productSellPrice;
                userCommissionPrecentage = 1;
            }
            else if (totalSellAmountInUSD > 1000 && totalSellAmountInUSD <= 4000)
            {
                userCommissionValue = 0.02 * productSellPrice;
                userCommissionPrecentage = 2;
                if (userSellesForthisMonth <= 1000)
                {
                    differenceTransaction.Credit = userSellesForthisMonth * (0.02 - lastTransactionPrecentageValue);
                    differenceTransaction.CreditUsd = userSellesForThisMonthUSD * (0.02 - lastTransactionPrecentageValue);
                    differenceTransaction.CommissionPrecentage = (0.02 - lastTransactionPrecentageValue) * 100;
                    differenceTransaction.BalanceUsd = userBalanceUSD + (userSellesForThisMonthUSD * (0.02 - lastTransactionPrecentageValue));
                    await Add(differenceTransaction);
                }
            }
            else if (totalSellAmountInUSD > 4000 && totalSellAmountInUSD <= 7000)
            {
                userCommissionValue = 0.03 * productSellPrice;
                userCommissionPrecentage = 3;
                if (userSellesForthisMonth <= 4000)
                {
                    differenceTransaction.Credit = userSellesForthisMonth * (0.03 - lastTransactionPrecentageValue);
                    differenceTransaction.CreditUsd = userSellesForThisMonthUSD * (0.03 - lastTransactionPrecentageValue);
                    differenceTransaction.CommissionPrecentage = (0.03 - lastTransactionPrecentageValue) * 100;
                    differenceTransaction.BalanceUsd = userBalanceUSD + (userSellesForThisMonthUSD * (0.03 - lastTransactionPrecentageValue));
                    await Add(differenceTransaction);
                }
            }
            else if (totalSellAmountInUSD > 7000 && totalSellAmountInUSD <= 10000)
            {
                userCommissionValue = 0.04 * productSellPrice;
                userCommissionPrecentage = 4;
                if (userSellesForthisMonth <= 7000)
                {
                    differenceTransaction.Credit = userSellesForthisMonth * (0.04 - lastTransactionPrecentageValue);
                    differenceTransaction.CreditUsd = userSellesForThisMonthUSD * (0.04 - lastTransactionPrecentageValue);
                    differenceTransaction.CommissionPrecentage = (0.04 - lastTransactionPrecentageValue) * 100;
                    differenceTransaction.BalanceUsd = userBalanceUSD + (userSellesForThisMonthUSD * (0.04 - lastTransactionPrecentageValue));
                    await Add(differenceTransaction);
                }
            }
            else
            {
                userCommissionValue = 0.05 * productSellPrice;
                userCommissionPrecentage = 5;
                if (userSellesForthisMonth <= 10000)
                {
                    differenceTransaction.Credit = userSellesForthisMonth * (0.05 - lastTransactionPrecentageValue);
                    differenceTransaction.CreditUsd = userSellesForThisMonthUSD * (0.05 - lastTransactionPrecentageValue);
                    differenceTransaction.CommissionPrecentage = (0.05 - lastTransactionPrecentageValue) * 100;
                    differenceTransaction.BalanceUsd = userBalanceUSD + (userSellesForThisMonthUSD * (0.05 - lastTransactionPrecentageValue));
                    await Add(differenceTransaction);
                }
            }

            return new UserCommissionDataResponseDTO
            {
                UserCommissionValue = userCommissionValue,
                UserCommissionPrecentage = userCommissionPrecentage
            };
        }

        public async Task<bool> AddTransactionTest(int userId, double usdExchangeRate,
            double productSellPrice, double debit, int opsId, string Currency,
            int? BouquetId = null, int? CreatedBy = null,
            TransactionExtended transactionExtended = null)
        {
            var userCommissionData = await GetUserCommissionAndAddDifferenceTransaction(userId, usdExchangeRate, productSellPrice, debit, opsId,
              Currency, BouquetId, CreatedBy, transactionExtended);

            return await AddTransaction(userId, usdExchangeRate, userCommissionData.UserCommissionValue, userCommissionData.UserCommissionPrecentage, productSellPrice, debit, opsId,
                Currency, BouquetId, CreatedBy, transactionExtended);
        }

        public async Task<double> GetLastTransactionCommissionPrecentage(int userId)
        {
            return await _applicationContext.Transactions
               .Where(i => i.UserId == userId && i.Status != TransactionStatus.Rejected)
               .OrderByDescending(i => i.Id).Select(i => (i.CreditUsd / i.ProductSellPriceUSD)).FirstOrDefaultAsync();
        }
        public async Task<double> GetUserBalanceInUSD(int userId)
        {
            var balanceUsd = await _applicationContext.Transactions
                .Where(i => i.UserId == userId && i.Status != TransactionStatus.Rejected)
                .OrderByDescending(i => i.Id).Select(i => i.BalanceUsd).FirstOrDefaultAsync();
            return balanceUsd;
        }
        public async Task<bool> Add(Transaction transaction)
        {
            await _applicationContext.AddAsync(transaction);
            _applicationContext.Entry(transaction).State = EntityState.Added;
            return await _applicationContext.SaveChangesAsync() > 0;
        }
        public async Task<double> GetUserProductSellsForThisMonth(int userId, int month)
        {
            var balanceUsd = await _applicationContext.Transactions
                .Where(i => i.UserId == userId && i.Status != TransactionStatus.Rejected && i.CreateDate.Month == month)
                .OrderByDescending(i => i.Id).SumAsync(i => i.ProductSellPrice);
            return balanceUsd;
        }
    }
}
