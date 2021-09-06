using System;
using System.Collections.Generic;

namespace Fantasy.Cfb.Regular.Domain
{
    public class LeagueData
    {
        public int Year { get; set; }
        public SeasonType SeasonType { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<Week> Weeks { get; set; }
        public List<Owner> Owners { get; set; }
        public int CurrentWeek { get; set; }

        public LeagueData(int year, SeasonType type)
        {
            LastUpdated = DateTime.UtcNow;
            Year = year;
            SeasonType = type;
            Weeks = new List<Week>();
            Owners = new List<Owner>();
        }

    }

    public class Week
    {
        public int Number { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Week(string scheduleRec)
        {
            var parts = scheduleRec.Split("|");
            int.TryParse(parts[0].Trim(), out var weekNumber);
            Number = weekNumber;

            DateTime.TryParse(parts[1].Trim(), out var start);
            Start = start;

            DateTime.TryParse(parts[2].Trim(), out var end);
            End = end;
        }
    }

    public class Owner
    {
        public string Name { get; set; }

        public List<Player> Players { get; set; }

        public List<OwnerWeek> Weeks { get; set; }

        public Owner()
        {
            Players = new List<Player>();
            Weeks = new List<OwnerWeek>();
        }

    }

    public class OwnerWeek
    {
        public int Number { get; set; }

        public double Score { get; set; }
    }

    public class Player
    {
        public string Name { get; set; }

        public PositionType Position { get; set; }

        public string Link { get; set; }

        public List<PlayerLog> Logs { get; set; }

        public string School { get; set; }

        public double Score { get; set; }
        public double Avg { get; set; }
    }

    public class PlayerLog
    {
        public int Week { get; set; }

        public DateTime Date { get; set; }

        public int PassYds { get; set; }

        public int RecYds { get; set; }

        public int RushYds { get; set; }

        public int Tds { get; set; }

        public int Rec { get; set; }

        public string Opp { get; set; }
        public string Vs { get; set; }
        public bool IsLog { get; set; }
        
        
        
        

        public double Score
        {
            get
            {
                double rush = Math.Round(RushYds / 10.0, 2);
                double rec = Math.Round(RecYds / 10.0, 2);
                double pass = Math.Round(PassYds / 25.0, 2);
                int rushBonus = RushYds >= 150 ? 10 : RushYds >= 100 ? 5 : 0;
                int recBonus = RecYds >= 150 ? 10 : RecYds >= 100 ? 5 : 0;
                int tdScore = Tds * 6;

                return Math.Round(rush + rec + pass + rushBonus + recBonus + Rec + tdScore, 2);
            }
        }

    }

    public enum PositionType
    {
        none = 0,
        QB = 1,
        WR = 2,
        RB = 3,
        TE = 4
    }

    public enum SeasonType
    {
        Regular = 0,
        Post = 1
    }

}

