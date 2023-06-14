using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCase23
{
    public class Generator
    {
        public void Run()
        {
            //Set the randomizer seed if you wish to generate repeatable data sets.
            Randomizer.Seed = new Random(8675309);



        }
    }



    public class Title
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int release_year { get; set; }
        public string age_certification { get; set;}
        public int runtime { get; set; }
        public List<string> genres { get; set; }
        public string product_country { get; set; }
        public int seasons { get; set;}
    }

    public class Credit
    {
        public int id { get; set; }
        public int title_id { get; set; }
        public string real_name { get; set; }
        public string character_name { get; set; }
        public string role { get; set; }
    }

}
