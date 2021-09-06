using System;
using System.Threading.Tasks;
using Fantasy.Cfb.Regular.Business;
using Fantasy.Cfb.Regular.Domain;

namespace Fantasy.Cfb.Regular.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // var rosterBuilder = new RosterBuilder();
            // await rosterBuilder.BuildRoster(2019);


            var playerReader = new PlayerReader();
            await playerReader.GetAllStats(2021, SeasonType.Regular);
        }
    }
}
