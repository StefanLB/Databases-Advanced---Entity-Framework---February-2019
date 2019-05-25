using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.DataProcessor.ExportDto
{
    public class ExportMoviesDto
    {
        public string MovieName { get; set; }

        public string Rating { get; set; }

        public string TotalIncomes { get; set; }

        public ExportCustomerDto[] Customers { get; set; }

    }

    public class ExportCustomerDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Balance { get; set; }

    }
}
