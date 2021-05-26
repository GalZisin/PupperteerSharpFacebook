using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PupperteerFacebookV2
{
    class CConfiguration
    {
        public IConfiguration Configuration { get; set; }

        public string User;

        public string Pass;

        public CConfiguration()

        {

            var builder = new ConfigurationBuilder()

            .AddJsonFile("appsettings.json");



            Configuration = builder.Build();

            User = Configuration["Account:User"];

            Pass = Configuration["Account:Pass"];

        }
    }
}
