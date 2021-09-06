using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fantasy.Cfb.Regular.Business;
using Fantasy.Cfb.Regular.Domain;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Fantasy.Cfb.Regular.Lambda
{
    public class LeagueRequest 
    {
        public int Year { get; set; }
        public SeasonType SeasonType { get; set; }
    }

    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(LeagueRequest input, ILambdaContext context)
        {
            var playerReader = new PlayerReader();
            Task.Run(() => playerReader.GetAllStats(input.Year, input.SeasonType)).GetAwaiter().GetResult();

            return string.Empty;
        }
    }
}
