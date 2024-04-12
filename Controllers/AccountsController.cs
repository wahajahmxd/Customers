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

        [HttpPost() ]
        public IActionResult AccountNumber([FromBody]
}
