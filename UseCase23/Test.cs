using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bogus;
using Newtonsoft;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Text;

namespace UseCase23
{
    public class Test
    {
        public void Run()
        {
            //Set the randomizer seed if you wish to generate repeatable data sets.
            Randomizer.Seed = new Random(8675309);

            var fruit = new[] { "apple", "banana", "orange", "strawberry", "kiwi" };

            var orderIds = 0;
            var testOrders = new Faker<Order>()
                //Ensure all properties have rules. By default, StrictMode is false
                //Set a global policy by using Faker.DefaultStrictMode
                .StrictMode(true)
                //OrderId is deterministic
                .RuleFor(o => o.OrderId, f => orderIds++)
                //Pick some fruit from a basket
                .RuleFor(o => o.Item, f => f.PickRandom(fruit))
                //A random quantity from 1 to 10
                .RuleFor(o => o.Quantity, f => f.Random.Number(1, 10))
                //A nullable int? with 80% probability of being null.
                //The .OrNull extension is in the Bogus.Extensions namespace.
                .RuleFor(o => o.LotNumber, f => f.Random.Int(0, 100).OrNull(f, .8f));


            var userIds = 0;
            var testUsers = new Faker<User>()
                //Optional: Call for objects that have complex initialization
                .CustomInstantiator(f => new User(userIds++, f.Random.Replace("###-##-####")))

                //Use an enum outside scope.
                .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())


                //Basic rules using built-in generators
                .RuleFor(u => u.FirstName, (f, u) => f.Name.LastName(GetBogusGender(u.Gender)))
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName(GetBogusGender(u.Gender)))
                .RuleFor(u => u.Avatar, f => f.Internet.Avatar())
                .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.SomethingUnique, f => $"Value {f.UniqueIndex}")

                //Use a method outside scope.
                .RuleFor(u => u.CartId, f => Guid.NewGuid())
                //Compound property with context, use the first/last name properties
                .RuleFor(u => u.FullName, (f, u) => u.FirstName + " " + u.LastName)
                //And composability of a complex collection.
                .RuleFor(u => u.Orders, f => testOrders.Generate(3).ToList())
                //Optional: After all rules are applied finish with the following action
                .FinishWith((f, u) =>
                {
                    Console.WriteLine("User Created! Id={0}", u.Id);
                });

            //var user = testUsers.Generate();
            //Console.WriteLine(user.DumpAsJson());

            List<User> users = testUsers.Generate(3);

            // serialize and save to CSV
            string csvString = CsvSerializer.SerializeToCsv(users);
            byte[] csvBytes = System.Text.Encoding.Unicode.GetBytes(csvString);
            //var x = File(csvBytes, "text/csv", "foo.csv");
            string fileName = "Output.csv";
            File.AppendAllText(fileName, csvString);
            //File.WriteAllBytes(fileName, csvBytes);
            //var file = File.Create(csvBytes, "text/csv", "foo.csv");
            //file.Write(csvBytes);

        }

        private Bogus.DataSets.Name.Gender GetBogusGender(Gender g)
        {
            var result = (Bogus.DataSets.Name.Gender)Enum.Parse(typeof(Bogus.DataSets.Name.Gender), g.ToString());
            return result;
        }
    }



    public class User
    {
        public Gender Gender { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string SomethingUnique { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public Guid CartId { get; internal set; }
        public string FullName { get; internal set; }

        public List<Order> Orders { get; set; }

        public User(int userId, string somethingUnique)
        {
            Id = userId;
            SomethingUnique = somethingUnique;

            //Orders = new List<Order>();
        }

        public string DumpAsJson()
        {
            var serializedUser = JsonConvert.SerializeObject(this, Formatting.Indented);
            return serializedUser;

            //throw new NotImplementedException();
        }
    }

    public class Order
    {
        public int OrderId { get; set; }

        public string Item { get; set; }

        public int Quantity { get; set; }

        public int? LotNumber { get; set; }

    }


    public enum Gender
    {
        Male,
        Female
    }

}
