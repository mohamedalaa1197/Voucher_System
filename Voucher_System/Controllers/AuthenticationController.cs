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
            await AddTransaction(1, 18.67, amount, 0, 1, "USD");

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

        public async Task<double> GetUserSalesForThisMonth(int userId, int month)
        {
            var balanceUsd = await _applicationContext.Transactions
                .Where(i => i.UserId == userId && i.Status != TransactionStatus.Rejected && i.CreateDate.Month == month)
                .OrderByDescending(i => i.Id).SumAsync(i => i.SaleUSD);
            return balanceUsd;
        }


        public async Task<bool> AddTransaction(int userId, double usdExchangeRate,
          double credit, double debit, int opsId, string Currency,
          int? BouquetId = null, int? CreatedBy = null,
          TransactionExtended transactionExtended = null)
        {
            var creditUSD = credit * usdExchangeRate;
            var debitUSD = debit * usdExchangeRate;
            var salesForthisMonthInUSD = await GetUserSalesForThisMonth(userId, DateTime.UtcNow.Month); //700
            double commissionUSDValue = 0;

            var differenceTransaction = new Transaction
            {
                UserId = userId,
                BouquetId = BouquetId ?? 0,
                CreatedBy = CreatedBy,
                Type = CommunityTransactionType.difference,
                Credit = credit,
                Debit = debit,
                Currency = Currency,
                ExchangeRate = usdExchangeRate,
                DebitUsd = debitUSD,
                CreditUsd = creditUSD,
                CreateDate = DateTime.UtcNow,
                Note = "",
                TransactionExtended = transactionExtended,
                Status = TransactionStatus.Confirmed
            };
            if (salesForthisMonthInUSD + creditUSD /* 700 + 100*/ <= 1000)
            {
                commissionUSDValue = 0.01 * creditUSD;
            }
            else if (salesForthisMonthInUSD + creditUSD > 1000 && salesForthisMonthInUSD + creditUSD < 4000)
            {
                commissionUSDValue = 0.02 * creditUSD;

                //To Do
                //Add difference transaction with the old values and sales = 0 and commission difference
                differenceTransaction.SaleUSD = 0;

                differenceTransaction.BalanceUsd = salesForthisMonthInUSD * 0.01 + commissionUSDValue;
                await _applicationContext.Transactions.AddAsync(differenceTransaction);
                await _applicationContext.SaveChangesAsync();

            }
            else if (salesForthisMonthInUSD + creditUSD > 4000 && salesForthisMonthInUSD + creditUSD < 7000)
            {
                commissionUSDValue = 0.03 * creditUSD;

                differenceTransaction.SaleUSD = 0;
                differenceTransaction.BalanceUsd = salesForthisMonthInUSD * 0.01 + commissionUSDValue;
                await _applicationContext.Transactions.AddAsync(differenceTransaction);
                await _applicationContext.SaveChangesAsync();


            }
            else if (salesForthisMonthInUSD + creditUSD > 7000 && salesForthisMonthInUSD + creditUSD < 10000)
            {
                commissionUSDValue = 0.04 * creditUSD;

                differenceTransaction.SaleUSD = 0;
                differenceTransaction.BalanceUsd = salesForthisMonthInUSD * 0.01 + commissionUSDValue;
                await _applicationContext.Transactions.AddAsync(differenceTransaction);
                await _applicationContext.SaveChangesAsync();

            }
            else
            {
                commissionUSDValue = 0.05 * creditUSD;

                differenceTransaction.SaleUSD = 0;
                differenceTransaction.BalanceUsd = salesForthisMonthInUSD * 0.01 + commissionUSDValue;
                await _applicationContext.Transactions.AddAsync(differenceTransaction);
                await _applicationContext.SaveChangesAsync();

            }




            var lastBalanceInUSD = await GetUserBalanceInUSD(userId);

            var currentBalanceInUSD = lastBalanceInUSD + commissionUSDValue - debitUSD;

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
                BalanceUsd = currentBalanceInUSD,
                CreateDate = DateTime.UtcNow,
                Note = "",
                TransactionExtended = transactionExtended,
                Status = TransactionStatus.Confirmed,
                SaleUSD = creditUSD
            };
            var trans = await _applicationContext.Transactions.AddAsync(transaction);
            await _applicationContext.SaveChangesAsync();

            return true;
        }
        public async Task<double> GetUserBalanceInUSD(int userId)
        {
            var balanceUsd = await _applicationContext.Transactions
                .Where(i => i.UserId == userId && i.Status != TransactionStatus.Rejected)
                .OrderByDescending(i => i.Id).Select(i => i.BalanceUsd).FirstOrDefaultAsync();
            return balanceUsd;
        }

    }
}
