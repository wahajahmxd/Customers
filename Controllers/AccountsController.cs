using Microsoft.AspNetCore.Mvc;
using Bank_Customers_Data_Clone_Project.Model;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections;

namespace Bank_Customers_Data_Clone_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {

        [HttpPost]
        public IActionResult Post([FromBody] Account customer)
        {
            try
            {
                var dbclient = new MongoClient("mongodb://localhost:27017");
                var db = dbclient.GetDatabase("NewDB");

                var collection = db.GetCollection<Account>("Accounts");

                var accountNumber = customer.AccountNumber;
                if (accountNumber != null && !string.IsNullOrEmpty(accountNumber) )
                {
                    var filterDefinition = Builders<Account>.Filter.Eq(c => c.AccountNumber, accountNumber);
                    var account = collection.Find(filterDefinition).FirstOrDefault();
                    if (account != null)
                    {
                        return BadRequest("This Account Number is already in use");
                    } else
                    {
                    collection.InsertOne(customer);
                    return Ok(customer);
                    }

                    //fetch account/customer
                    //account!=null => already excists
                   // return BadRequest("This Account Number is already in use");
                } else
                {
                    
                    return BadRequest("sorry");
                }

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(customer);
        }   

        [HttpGet]
        public IActionResult Get()
        {
            var accountNumber = "0292029029";

            var dbclient = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase db = dbclient.GetDatabase("NewDB");

            var collectionName = db.GetCollection<Account>("Accounts");
            //var filterDefinition = Builders<Customer>.Filter.Empty; // Empty filter to match all documents

            var filterDefinition = Builders<Account>.Filter.Eq(c => c.AccountNumber, accountNumber);
            var customer = collectionName.Find(filterDefinition).FirstOrDefault();

            /// add 1000 rs in the customer's balance and then return the customer 
            var NewBalance = customer.AccountBalance;
            var NewBalanceInt = long.Parse(NewBalance);

            NewBalanceInt = NewBalanceInt + 1000;
            customer.AccountBalance = NewBalanceInt.ToString();
            return Ok(customer);
        }

        [HttpPost("BalInquiry", Name = "BalInquiry")]
        public IActionResult BalInquiry([FromBody] AccountNumberInput input)
        {
            try
            {
                var accountNumber = input.AccountNumber;


                var dbclient = new MongoClient("mongodb://localhost:27017");
                var db = dbclient.GetDatabase("NewDB");

                var collectionName = db.GetCollection<Account>("Accounts");

                //filtering Database with Account Number
                var filterDefinition = Builders<Account>.Filter.Eq(c => c.AccountNumber, accountNumber);
                var account = collectionName.Find(filterDefinition).FirstOrDefault();

                if (account != null)
                {

                    var Balance = account.AccountBalance;

                    return Ok(Balance);
                }
                else
                {
                    throw new Exception();
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Credit", Name = "Credit")]
        public IActionResult Credit([FromBody] Credit credit)
        {
            try
            {
                var amount = credit.Amount;
                var accountNumber = credit.AccountNumber;

                var dbclient = new MongoClient("mongodb://localhost:27017");
                var db = dbclient.GetDatabase("NewDB");

                var collectionName = db.GetCollection<Account>("Accounts");

                //filtering Database with A/C
                var filterDefinition = Builders<Account>.Filter.Eq(c => c.AccountNumber, accountNumber);
                var customer = collectionName.Find(filterDefinition).FirstOrDefault();
                var balance = 0.0;

                //getting the account number and converting its balance into int first
                // crediting the amount in the account
                //var Balance = customer.AccountBalance;
                //var BalanceInt = long.Parse(Balance);
                if (customer != null)
                {
                    var newBalance = customer.AccountBalance + amount;
                    customer.AccountBalance = newBalance;
                    //customer.AccountBalance = NewBalance.ToString();

                    var updateDefinition = Builders<Account>.Update.Set("AccountBalance", customer.AccountBalance);

                    collectionName.UpdateOne(filterDefinition, updateDefinition);

                    return Ok(customer);
                }
                else
                {
                    return BadRequest("No user is present on your reference");
                }
            } catch (Exception ex)
                {
                return BadRequest(ex.Message);
                }

        }

        [HttpPost("Debit", Name ="Debit")]
        public IActionResult Debit([FromBody]Credit  debit) 
        {
            var amount = debit.Amount;
            var accountNumber = debit.AccountNumber;

            var dbclient = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase db = dbclient.GetDatabase("NewDB");
            var collectionName = db.GetCollection<Account>("Accounts");

            //filtering Database with A/C
            var filterDefinition = Builders<Account>.Filter.Eq(c => c.AccountNumber, accountNumber);

            var customer = collectionName.Find(filterDefinition).FirstOrDefault();

            //getting account number, changing its balance from string into int
            var Balance = customer.AccountBalance;
            var BalanceInt = long.Parse(Balance);

            //debiting the amount
            if (BalanceInt > amount)
            {
                var NewBalance = BalanceInt - amount;
                customer.AccountBalance = NewBalance.ToString();

                var updateDefinition = Builders<Account>.Update.Set("AccountBalance", customer.AccountBalance);

                collectionName.UpdateOne(filterDefinition, updateDefinition);
                return Ok(customer);
            } else
            {
                return NotFound("Not enough funds to process this payment");
            }
        }
        

    }
}


