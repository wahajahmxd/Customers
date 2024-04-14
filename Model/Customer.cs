﻿using MongoDB.Bson.Serialization.Attributes;

namespace Bank_Customers_Data_Clone_Project.Model
{
    [BsonIgnoreExtraElements]
    public class Customer
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public string AccountBalance { get; set; }

    }

    public class AccountNumberInput
    {
        public string AccountNumber { get; set; }
    }

    public class Credit
    {
        public int Amount { get; set; }

        public string AccountNumber { get; set; }
        public int Balance { get; set; }

    }
}

