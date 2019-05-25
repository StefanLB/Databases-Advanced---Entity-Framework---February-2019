using System;

namespace P01_HospitalDatabase
{
    using P01_HospitalDatabase.Data;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using(HospitalContext context = new HospitalContext())
            {

                //context.Database.EnsureDeleted();

            }
        }
    }
}
