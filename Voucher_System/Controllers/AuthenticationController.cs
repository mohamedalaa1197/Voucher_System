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
        public async Task<IActionResult> Test([FromQuery] double amount)
        {
            await AddTransaction(1, 1, amount, 0, 1, "USD");

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


        public async Task<bool> AddTransaction(int userId, double usdExchangeRate,
            double productSellPrice, double debit, int opsId, string Currency,
            int? BouquetId = null, int? CreatedBy = null,
            TransactionExtended transactionExtended = null)
        {
            var productSellPrice_USD = productSellPrice * usdExchangeRate;
            var debitUSD = debit * usdExchangeRate;
            var userSellesForthisMonth = await GetUserProductSellsForThisMonth(userId, DateTime.UtcNow.Month); //700
            var userSellesForThisMonth_USD = userSellesForthisMonth * usdExchangeRate;
            double userCommissionValue = 0;
            double userCommissionValue_USD = 0;
            var userBalance = await GetUserBalanceInUSD(userId);
            var differenceTransaction = new Transaction
            {
                UserId = userId,
                BouquetId = BouquetId ?? 0,
                CreatedBy = CreatedBy,
                Type = CommunityTransactionType.difference,
                Debit = debit,
                Currency = Currency,
                ExchangeRate = usdExchangeRate,
                DebitUsd = debitUSD,
                CreateDate = DateTime.UtcNow,
                Note = "",
                TransactionExtended = transactionExtended,
                Status = TransactionStatus.Confirmed
            };
            var totalSellAmountInUSD = (userSellesForThisMonth_USD + productSellPrice_USD);
            if (totalSellAmountInUSD <= 1000)
            {
                userCommissionValue = 0.01 * productSellPrice;
                userCommissionValue_USD = userCommissionValue * usdExchangeRate;
            }
            else if (totalSellAmountInUSD > 1000 && totalSellAmountInUSD < 4000)
            {
                userCommissionValue = 0.02 * productSellPrice;
                userCommissionValue_USD = userCommissionValue * usdExchangeRate;
                if (userSellesForthisMonth <= 1000)
                {
                    differenceTransaction.ProductSellPriceUSD = 0;
                    differenceTransaction.ProductSellPrice = 0;
                    differenceTransaction.Credit = userSellesForthisMonth * 0.01;
                    differenceTransaction.CreditUsd = userSellesForThisMonth_USD * 0.01;
                    differenceTransaction.BalanceUsd = userBalance + (userSellesForThisMonth_USD * 0.01) + userCommissionValue;
                    await Add(differenceTransaction);
                }

            }
            else if (totalSellAmountInUSD > 4000 && totalSellAmountInUSD < 7000)
            {
                userCommissionValue = 0.03 * productSellPrice;
                userCommissionValue_USD = userCommissionValue * usdExchangeRate;
                if (userSellesForthisMonth <= 4000)
                {
                    differenceTransaction.ProductSellPriceUSD = 0;
                    differenceTransaction.ProductSellPrice = 0;
                    differenceTransaction.Credit = userSellesForthisMonth * 0.01;
                    differenceTransaction.CreditUsd = userSellesForThisMonth_USD * 0.01;
                    differenceTransaction.BalanceUsd = userBalance + (userSellesForThisMonth_USD * 0.01) + userCommissionValue;
                    await Add(differenceTransaction);
                }
            }
            else if (totalSellAmountInUSD > 7000 && totalSellAmountInUSD < 10000)
            {
                userCommissionValue = 0.04 * productSellPrice;
                userCommissionValue_USD = userCommissionValue * usdExchangeRate;
                if (userSellesForthisMonth <= 7000)
                {
                    differenceTransaction.ProductSellPriceUSD = 0;
                    differenceTransaction.ProductSellPrice = 0;
                    differenceTransaction.Credit = userSellesForthisMonth * 0.01;
                    differenceTransaction.CreditUsd = userSellesForThisMonth_USD * 0.01;
                    differenceTransaction.BalanceUsd = userBalance + (userSellesForThisMonth_USD * 0.01) + userCommissionValue;
                    await Add(differenceTransaction);
                }
            }
            else
            {
                userCommissionValue = 0.05 * productSellPrice;
                userCommissionValue_USD = userCommissionValue * usdExchangeRate;
                if (userSellesForthisMonth <= 10000)
                {
                    differenceTransaction.ProductSellPriceUSD = 0;
                    differenceTransaction.ProductSellPrice = 0;
                    differenceTransaction.Credit = userSellesForthisMonth * 0.01;
                    differenceTransaction.CreditUsd = userSellesForThisMonth_USD * 0.01;
                    differenceTransaction.BalanceUsd = userBalance + (userSellesForThisMonth_USD * 0.01) + userCommissionValue;
                    await Add(differenceTransaction);
                }
            }

            var lastBalance = await GetUserBalanceInUSD(userId);
            var currentBalance = lastBalance + userCommissionValue - debitUSD;

            Transaction transaction = new Transaction
            {
                UserId = userId,
                BouquetId = BouquetId ?? 0,
                CreatedBy = CreatedBy,
                Type = productSellPrice > 0 ? CommunityTransactionType.Credit : CommunityTransactionType.Debit,
                Credit = userCommissionValue,
                Debit = debit,
                Currency = Currency,
                ExchangeRate = usdExchangeRate,
                DebitUsd = debitUSD,
                CreditUsd = userCommissionValue_USD,
                BalanceUsd = currentBalance,
                CreateDate = DateTime.UtcNow,
                Note = "",
                TransactionExtended = transactionExtended,
                Status = TransactionStatus.Confirmed,
                ProductSellPrice = productSellPrice,
                ProductSellPriceUSD = productSellPrice_USD,

            };
            var trans = await Add(transaction);

            return true;
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
