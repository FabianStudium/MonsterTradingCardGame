﻿using System;

namespace MonsterTradingCardGame.Exceptions
{
    public class UserDoesntHaveCardExcpt : Exception
    {
        public UserDoesntHaveCardExcpt() { }
        public UserDoesntHaveCardExcpt(string message) : base(message) { }
        public UserDoesntHaveCardExcpt(string message, Exception innerException) : base(message, innerException) { }
    }
}