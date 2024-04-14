using Microsoft.AspNetCore.Mvc;
using Bank_Customers_Data_Clone_Project.Model;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bank_Customers_Data_Clone_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {

        [HttpPost]
        public IActionResult Post([FromBody] Customer customer)
        {
            var dbclient = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase db = dbclient.GetDatabase("NewDB");

            var collection = db.GetCollection<Customer>("Customers");

            collection.InsertOne(customer);
            return Ok(customer);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var accountNumber = "0292029029";

            var dbclient = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase db = dbclient.GetDatabase("NewDB");

            var collectionName = db.GetCollection<Customer>("Customers");
            //var filterDefinition = Builders<Customer>.Filter.Empty; // Empty filter to match all documents

            var filterDefinition = Builders<Customer>.Filter.Eq(c => c.AccountNumber, accountNumber);
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
            var accountNumber = $"{input.AccountNumber}";
            

            var dbclient = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase db = dbclient.GetDatabase("NewDB");

            var collectionName = db.GetCollection<Customer>("Customers");
            
            //filtering Database with Account Number
            var filterDefinition = Builders<Customer>.Filter.Eq(c => c.AccountNumber, accountNumber);
            var user = collectionName.Find(filterDefinition).FirstOrDefault();

            var Balance = user.AccountBalance;
            return Ok(Balance);
        }

        [HttpPost("Credit", Name ="Credit")]
        public IActionResult Credit([FromBody] Credit credit)
        {
            var amount = credit.Amount;
            var accountNumber = credit.AccountNumber;

            var dbclient = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase db = dbclient.GetDatabase("NewDB");

            var collectionName = db.GetCollection<Customer>("Customers");

            //filtering Database with A/C
            var filterDefinition = Builders<Customer>.Filter.Eq(c => c.AccountNumber, accountNumber);
            var customer = collectionName.Find(filterDefinition).FirstOrDefault();

            //getting the account number and converting its balance into int first
            // crediting the amount in the account
            var Balance = customer.AccountBalance;
            var BalanceInt = long.Parse(Balance);

            var NewBalance = BalanceInt + amount;
            customer.AccountBalance = NewBalance.ToString();
  
            return Ok(customer);
        }

        [HttpPost("Debit", Name ="Debit")]
        public IActionResult Debit([FromBody]Debit  debit) 
        {
            var amount = debit.Amount;
            var accountNumber = debit.AccountNumber;

            var dbclient = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase db = dbclient.GetDatabase("NewDB");

            var collectionName = db.GetCollection<Customer>("Customers");

            //filtering Database with A/C
            var filterDefinition = Builders<Customer>.Filter.Eq(c => c.AccountNumber, accountNumber);
            var customer = collectionName.Find(filterDefinition).FirstOrDefault();

            //getting account number, changing its balance from string into int
            var Balance = customer.AccountBalance;
            var BalanceInt = long.Parse(Balance);

            //debiting the amount
            if (BalanceInt > amount)
            {
                var NewBalance = BalanceInt - amount;
                customer.AccountBalance = NewBalance.ToString();
                return Ok(customer);
            } else
            {
                return NotFound("Not enough funds to process this payment");
            }
        }
    }
}