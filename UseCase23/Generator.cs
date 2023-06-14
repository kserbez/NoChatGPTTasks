using Bogus;
using Bogus.DataSets;
using ServiceStack.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static Bogus.DataSets.Name;
using static ServiceStack.Diagnostics.Events;

namespace UseCase23
{
    public class Generator
    {
        private const int MAX_TITLE_WORDS_NUMBER = 4;
        private const int EARLIEST_MOVIE_YEAR = 1922;
        private const int MAX_RUNTIME = 250;
        private const int MAX_NUMBER_OF_GENRES = 3;
        private const int MaxNumberOfSeasons = 6;

        private readonly List<string> AGE_CERTIFICATION = new List<string>
        {
            "G", "PG", "PG-13", "R", "NC-17", "U", "U/A", "A", "S", "AL", "6",
            "9", "12", "12A", "15", "18", "18R", "R18", "R21", "M", "MA15+",
            "R16", "R18+", "X18", "T", "E", "E10+", "EC", "C", "CA", "GP",
            "M/PG", "TV-Y", "TV-Y7", "TV-G", "TV-PG", "TV-14", "TV-MA"
        };

        private readonly List<string> GENRES = new List<string>
        {
            "Drama", "Detetive", "Thriller", "Comedy", "Documentary"
        };

        private readonly List<string> CREDITS_ROLES = new List<string>
        {
            "Director", "Producer", "Screenwriter", "Actor", "Actress",
            "Cinematographer", "Film Editor", "Production Designer",
            "Costume Designer", "Music Composer"
        };


        public void Run()
        {
            Randomizer.Seed = new Random(Guid.NewGuid().GetHashCode());

            int titleIds = 0;

            var testTitles = new Faker<Title>()
                .CustomInstantiator(f => new Title(titleIds++))
                .RuleFor(t => t.title, f => f.Lorem.Sentence(f.Random.Number(1, MAX_TITLE_WORDS_NUMBER)))
                .RuleFor(t => t.description, f => f.Lorem.Text())
                .RuleFor(t => t.release_year, f => f.Date.Random.Int(EARLIEST_MOVIE_YEAR, DateTime.Now.Year))
                .RuleFor(t => t.age_certification, f => f.PickRandom(AGE_CERTIFICATION))
                .RuleFor(t => t.runtime, f => f.Random.Number(1, MAX_RUNTIME))
                .RuleFor(t => t.genres, f => f.Make(f.Random.Number(1, MAX_NUMBER_OF_GENRES), () => f.PickRandom(GENRES)))
                .RuleFor(t => t.product_country, f => f.Address.CountryCode(Bogus.DataSets.Iso3166Format.Alpha3))
                .RuleFor(t => t.seasons, f => f.Random.Int(1, MaxNumberOfSeasons).OrNull(f, .7f));

            //var generatedTitle = testTitles.Generate();

            List<Title> generatedTitles = testTitles.Generate(120);

            int creditIds = 0;

            var testCredits = new Faker<Credit>()
            .CustomInstantiator(f => new Credit(creditIds++))
            .RuleFor(c => c.real_name, f => f.Person.FullName)
            .RuleFor(c => c.character_name, f => (f.Person.LastName + " " + f.Person.FirstName))
            .RuleFor(c => c.role, f => f.PickRandom(CREDITS_ROLES))
            .RuleFor(c => c.title_id, f => f.PickRandom(generatedTitles).id);

            List<Credit> generatedCredits = testCredits.Generate(305);

            SaveTitlesToCsv(generatedTitles);
            SaveCreditsToCsv(generatedCredits);
        }

        private void SaveTitlesToCsv(List<Title> titles)
        {
            string csvString = CsvSerializer.SerializeToCsv(titles);
            byte[] csvBytes = System.Text.Encoding.Unicode.GetBytes(csvString);
            string fileName = "Titles.csv";
            File.Delete(fileName);
            File.WriteAllBytes(fileName, csvBytes);
        }

        private void SaveCreditsToCsv(List<Credit> credits)
        {
            string csvString = CsvSerializer.SerializeToCsv(credits);
            byte[] csvBytes = System.Text.Encoding.Unicode.GetBytes(csvString);
            string fileName = "Credits.csv";
            File.Delete(fileName);
            File.WriteAllBytes(fileName, csvBytes);
        }
    }


    public class Title
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int release_year { get; set; }
        public string age_certification { get; set; }
        public int runtime { get; set; }
        public List<string> genres { get; set; }
        public string product_country { get; set; }
        public int? seasons { get; set; }

        public Title(int inputId) => id = inputId;
    }


    public class Credit
    {
        public int id { get; set; }
        public int title_id { get; set; }
        public string real_name { get; set; }
        public string character_name { get; set; }
        public string role { get; set; }

        public Credit(int inputId) => id = inputId;
    }

}
