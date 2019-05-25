using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Core.IO.Contracts
{
    public interface IWriter
    {
        void WriteLine(string text);
    }
}
