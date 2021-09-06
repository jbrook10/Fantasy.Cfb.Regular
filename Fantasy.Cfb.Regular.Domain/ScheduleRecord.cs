using System;

namespace Fantasy.Cfb.Regular.Domain
{
    public class ScheduleRecord
    {      
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int Week { get; set; }
        public DateTime Date { get; set; }        
    }
}