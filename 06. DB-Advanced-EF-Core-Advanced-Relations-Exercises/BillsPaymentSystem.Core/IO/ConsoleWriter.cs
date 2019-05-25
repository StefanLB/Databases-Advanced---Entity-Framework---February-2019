using BillsPaymentSystem.Core.IO.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Core.IO
{
    public class ConsoleWriter : IWriter
    {
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
    }
}
