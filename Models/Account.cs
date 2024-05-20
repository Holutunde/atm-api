﻿
using ATMAPI.Interfaces;

namespace ATMAPI.Models
{
        public class Account : IAccount
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public long AccountNumber { get; set; }
            public int Pin { get; set; }
            public double Balance { get; set; }

            public DateTime OpeningDate { get; set; }

        }

    
}