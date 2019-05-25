using BillsPaymentSystem.Core;
using BillsPaymentSystem.Core.Contracts;
using BillsPaymentSystem.Core.IO;
using BillsPaymentSystem.Core.IO.Contracts;
using BillsPaymentSystem.Data;
using BillsPaymentSystem.Initializer;
using System;

namespace BillsPaymentSystem.App
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using(var context = new BillsPaymentSystemContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                Initialize.Seed(context);

                IReader reader = new ConsoleReader();
                IWriter writer = new ConsoleWriter();
                IEngine engine = new Engine(reader, writer, context);

                engine.Run();
            }
        }
    }
}
