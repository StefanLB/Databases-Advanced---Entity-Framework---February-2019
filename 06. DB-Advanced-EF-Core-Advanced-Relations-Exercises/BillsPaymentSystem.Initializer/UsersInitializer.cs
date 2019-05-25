using BillsPaymentSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Initializer
{
    public class UsersInitializer
    {
        public static User[] GetUsers()
        {
            return new User[]
            {
                new User() {FirstName = "Ivan", LastName = "Petrov", Email = "Petrov@abv.bg", Password = "1234" },
                new User() {FirstName = "Bvan", LastName = "Metrov", Email = "Metrov@abv.bg", Password = "4321" },
                new User() {FirstName = "Vvan", LastName = "Ketrov", Email = "Ketrov@abv.bg", Password = "123456" },
                new User() {FirstName = "Gvan", LastName = "Netrov", Email = "Netrov@abv.bg", Password = "654321" },
                new User() {FirstName = "Dvan", LastName = "Setrov", Email = "Setrov@abv.bg", Password = "12345678" },
                new User() {FirstName = "Evan", LastName = "Getrov", Email = "Getrov@abv.bg", Password = "87654321" },
                new User() {FirstName = "Jvan", LastName = "Detrov", Email = "Detrov@abv.bg", Password = "0000" },
                new User() {FirstName = "Zvan", LastName = "Tetrov", Email = "Tetrov@abv.bg", Password = "0000" }
            };
        }
    }
}
