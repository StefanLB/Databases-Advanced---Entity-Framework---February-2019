using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace P01_StudentSystem.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder builder)
        {
            string studentPath = @"C:\Users\StefanLB\source\repos\EntityRelations\P01_StudentSystem\Files\Students.txt";
            string coursePath = @"C:\Users\StefanLB\source\repos\EntityRelations\P01_StudentSystem\Files\Courses.txt";
            string homeworkPath = @"C:\Users\StefanLB\source\repos\EntityRelations\P01_StudentSystem\Files\Homeworks.txt";
            string resourcePath = @"C:\Users\StefanLB\source\repos\EntityRelations\P01_StudentSystem\Files\Resources.txt";

            GetDataFromFile<Student>(studentPath, builder);
            GetDataFromFile<Course>(coursePath, builder);
            GetDataFromFile<Homework>(homeworkPath, builder);
            GetDataFromFile<Resource>(resourcePath, builder);
        }

        private static void GetDataFromFile<T>(string path, ModelBuilder builder)
            where T : class
        {
            var entities = new List<T>();
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                entities = JsonConvert.DeserializeObject<List<T>>(json);
            }

            builder.Entity<T>().HasData(entities);
        }
    }
}
