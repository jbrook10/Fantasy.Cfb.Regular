using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Fantasy.Cfb.Regular.Domain;
using HtmlAgilityPack;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;

namespace Fantasy.Cfb.Regular.Business
{
    public class PlayerReader
    {
        private const string playerUrl = "https://widgets.sports-reference.com/wg.fcgi?site=cfb&url=%2Fcfb%2Fplayers%2F{0}%2Fgamelog%2F{1}%2F&div=div_gamelog&cx={2}";
        static readonly HttpClient client = new HttpClient();
        private AmazonS3Client _s3Client;

        public async Task GetAllStats(int year, SeasonType seasonType)
        {
              _s3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            var rosterFileResponse = await _s3Client.GetObjectAsync("cfb-regular", $"data/{year}.Roster.{seasonType}.txt");
            StreamReader rosterReader = new StreamReader(rosterFileResponse.ResponseStream);
            string rosterContent = rosterReader.ReadToEnd();

            var scheduleFileResponse = await _s3Client.GetObjectAsync("cfb-regular", $"data/{year}.Schedule.{seasonType}.txt");
            StreamReader reader = new StreamReader(scheduleFileResponse.ResponseStream);
            string scheduleContent = reader.ReadToEnd();

            var splitChar = rosterContent.Contains("\r\n") ? "\r\n" : "\n";

            var leagueData = new LeagueData(year, seasonType);

            foreach (var scheduleRec in scheduleContent.Split(splitChar, StringSplitOptions.RemoveEmptyEntries))
            {
                leagueData.Weeks.Add(new Week(scheduleRec));
            }

            foreach (var rosterRec in rosterContent.Split(splitChar, StringSplitOptions.RemoveEmptyEntries))
            {
                Console.WriteLine(rosterRec);
                var rosterEntry = new RosterEntry(rosterRec);

                // if (!rosterEntry.Name.Contains("Tahj W")) {
                //     continue;
                // }

                var player = await GetPlayerDataAsync(leagueData, rosterEntry, year, seasonType);

                var owner = leagueData.Owners.FirstOrDefault(o => o.Name == rosterEntry.Owner);
                if (owner == null)
                {
                    owner = new Owner()
                    {
                        Name = rosterEntry.Owner,
                        Players = new List<Player>()
                    };
                    leagueData.Owners.Add(owner);
                }
                owner.Players.Add(player);
                // System.Threading.Thread.Sleep(100);
            }

            // add schedule
            await GetSchedule(leagueData, year, seasonType);

            // add weekly scores
            foreach (var owner in leagueData.Owners)
            {
                for (var i = 1; i <= leagueData.Weeks.Count(); i++)
                {
                    owner.Weeks.Add(new OwnerWeek()
                    {
                        Number = i,
                        Score = Math.Round(owner.Players.SelectMany(p => p.Logs).Where(p => p.Week == i).OrderByDescending(p => p.Score).Take(10).Sum(p => p.Score), 2)
                    });
                }
            }

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Converters = {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            string jsonString = JsonSerializer.Serialize(leagueData, options);

            var putObjectRequest = new Amazon.S3.Model.PutObjectRequest()
            {
                BucketName = "cfb-regular",
                Key = $"assets/{year}.Regular.Data.json",
                ContentBody = jsonString,
                ContentType = "application/json"
            };
            await _s3Client.PutObjectAsync(putObjectRequest);
            //File.WriteAllText($@"D:\jerem\Documents\code\fantasy\Fantasy.Cfb.Regular\CfbRegular\src\assets\{year}.Regular.Data.json", jsonString);


            Console.WriteLine("done");
        }

        private async Task GetSchedule(LeagueData leagueData, int year, SeasonType seasonType)
        {
            var scheduleUrl = $"https://widgets.sports-reference.com/wg.fcgi?site=cfb&url=%2Fcfb%2Fyears%2F{year}-schedule.html&div=div_schedule";
            var content = await client.GetStringAsync(scheduleUrl);
            var startContent = content.IndexOf("<table");
            var endContent = content.IndexOf("</table>") + 8;

            var html = $"<html><body>{content.Substring(startContent, endContent - startContent)}</body></html>";
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var scheduleList = new List<ScheduleRecord>();
            var rows = doc.DocumentNode.SelectNodes("//tr");
            foreach (var row in rows)
            {
                var points = row.SelectSingleNode("td[@data-stat='winner_points']")?.InnerHtml ?? "skip me";
                //if (points == string.Empty)
                //{

                var isWinnerAwayTeam = row.SelectSingleNode($"td[@data-stat='game_location']")?.InnerHtml == "@";
                var hasPoints = points == string.Empty;

                var homeTeam = string.Empty;
                var awayTeam = string.Empty;

                if (hasPoints && !isWinnerAwayTeam)
                {
                    homeTeam = GetSchoolName(row, "winner_school_name");
                    awayTeam = GetSchoolName(row, "loser_school_name");
                }
                else
                {
                    homeTeam = GetSchoolName(row, "loser_school_name");
                    awayTeam = GetSchoolName(row, "winner_school_name");
                }

                var gameDate = GetGameDate(row);
                scheduleList.Add(new ScheduleRecord()
                {
                    HomeTeam = homeTeam,
                    AwayTeam = awayTeam,
                    Date = gameDate,
                    Week = GetWeek(gameDate, leagueData.Weeks)
                });
                //}
            }

            foreach (var owner in leagueData.Owners)
            {
                foreach (var player in owner.Players)
                {
                    foreach (var week in leagueData.Weeks)
                    {
                        var weekGames = scheduleList.Where(s => s.Week == week.Number && (s.HomeTeam == player.School || s.AwayTeam == player.School));
                        foreach (var scheduledGame in weekGames)
                        {
                            var matchingLogs = player.Logs.Where(l => l.Week == week.Number && (l.Opp == scheduledGame.HomeTeam || l.Opp == scheduledGame.AwayTeam));
                            if (!matchingLogs.Any())
                            {
                                var isHomeTeam = scheduledGame.HomeTeam == player.School;
                                player.Logs.Add(new PlayerLog()
                                {
                                    Date = scheduledGame.Date,
                                    Opp = isHomeTeam ? scheduledGame.AwayTeam : scheduledGame.HomeTeam,
                                    Week = week.Number,
                                    Vs = isHomeTeam ? "vs" : "@"
                                });
                            }
                        }
                    }
                }
            }
        }

        private async Task<Player> GetPlayerDataAsync(LeagueData leagueData, RosterEntry rosterEntry, int year, SeasonType seasonType)
        {

            var player = new Player()
            {
                Name = rosterEntry.Name,
                Link = $"https://www.sports-reference.com/cfb/players/{rosterEntry.Link}/gamelog/{year}/",
                Position = rosterEntry.Position,
                Logs = new List<PlayerLog>(),
                School = rosterEntry.School.Trim()
            };

            var playerContent = await client.GetStringAsync(string.Format(playerUrl, rosterEntry.Link, year, DateTime.UtcNow.ToString("o")));
            var startContent = playerContent.IndexOf("<table");
            var endContent = playerContent.IndexOf("</table>") + 8;

            if (startContent < 0)
            {
                return player;
            }

            var html = $"<html><body>{playerContent.Substring(startContent, endContent - startContent)}</body></html>";
            HtmlDocument playerDoc = new HtmlDocument();
            playerDoc.LoadHtml(html);

            var logRows = playerDoc.DocumentNode.SelectNodes("//table/tbody/tr");
            foreach (var logRow in logRows)
            {
                player.Logs.Add(GetPlayerLog(logRow, leagueData.Weeks));
            }

            player.Score = Math.Round(player.Logs.Sum(l => l.Score), 2);

            var logCount = player.Logs.Count(l => l.IsLog);
            if (logCount > 0)
            {
                player.Avg = Math.Round(player.Score / logCount, 2);
            }

            return player;

        }

        private PlayerLog GetPlayerLog(HtmlNode logRow, List<Week> weeks)
        {
            var log = new PlayerLog();

            if (DateTime.TryParse(logRow.SelectSingleNode("td[@data-stat='date_game']/a")?.InnerHtml, out var date))
            {
                log.Date = date;

                var week = weeks.FirstOrDefault(w => date >= w.Start && date <= w.End);
                if (week != null)
                {
                    log.Week = week.Number;
                }
            }

            log.Opp = logRow.SelectSingleNode("td[@data-stat='opp_name']/a")?.InnerHtml ?? logRow.SelectSingleNode("td[@data-stat='opp_name']")?.InnerHtml;
            log.Vs = logRow.SelectSingleNode("td[@data-stat='game_location']")?.InnerHtml == "@" ? "@" : "vs";
            log.PassYds = GetByDataStat(logRow, "pass_yds");
            log.RushYds = GetByDataStat(logRow, "rush_yds");
            log.RecYds = GetByDataStat(logRow, "rec_yds");
            log.Rec = GetByDataStat(logRow, "rec");
            log.Tds = GetByDataStat(logRow, "pass_td") + GetByDataStat(logRow, "rush_td") + GetByDataStat(logRow, "rec_td");
            log.IsLog = true;

            return log;
        }

        private int GetByDataStat(HtmlNode node, string stat)
        {
            var statText = node.SelectSingleNode($"td[@data-stat='{stat}']")?.InnerHtml ?? "";

            statText = statText.Replace("<strong>", "")
                               .Replace("</strong>", "")
                               .Replace("<em>", "")
                               .Replace("</em>", "");

            var preDecimal = statText.Split('.').FirstOrDefault();
            var parsed = int.TryParse(preDecimal, out var parsedValue);
            return parsedValue;
        }

        private DateTime GetGameDate(HtmlNode node)
        {
            var dateText = node.SelectSingleNode($"td[@data-stat='date_game']/a")?.InnerHtml ?? "";
            var timeText = node.SelectSingleNode($"td[@data-stat='time_game']")?.InnerHtml ?? "";

            var validDate = DateTime.TryParse(dateText, out var parsedDate);
            var validDateAndTime = DateTime.TryParse(dateText + " " + timeText, out var parsedDateAndTime);

            TimeZoneInfo zn;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                zn = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
            else
            {
                zn = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            }

            if (validDateAndTime)
            {

                var allTimeParts = timeText.Split(' ');
                var timeNumberParts = allTimeParts[0].Split(':');
                var isPm = allTimeParts[1] == "PM";
                var hour = int.Parse(timeNumberParts[0]) + (isPm ? 12 : 0);

                if (hour == 24)
                {
                    hour = 12;
                }

                var minutes = int.Parse(timeNumberParts[1]);
                DateTimeOffset dateTimeOffset = new DateTimeOffset(new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, hour, minutes, 0, DateTimeKind.Unspecified), zn.BaseUtcOffset);
                parsedDateAndTime = dateTimeOffset.UtcDateTime;

            }

            return validDateAndTime ? parsedDateAndTime : validDate ? parsedDate : DateTime.MinValue;
        }

        private string GetSchoolName(HtmlNode node, string stat)
        {
            var statText = node.SelectSingleNode($"td[@data-stat='{stat}']")?.InnerHtml ?? "";
            if (statText.Contains("<a"))
            {
                statText = node.SelectSingleNode($"td[@data-stat='{stat}']/a")?.InnerHtml ?? "";
            }

            statText = statText.Replace("<strong>", "")
                               .Replace("</strong>", "")
                               .Replace("<em>", "")
                               .Replace("</em>", "");

            return statText;
        }

        private int GetWeek(DateTime gameDate, List<Week> weeks)
        {
            TimeZoneInfo zn;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                zn = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
            else
            {
                zn = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            }

            var timeUtc = gameDate.ToUniversalTime();
            var easterDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, zn);

            var week = weeks.FirstOrDefault(w => easterDate.Date >= w.Start.Date && easterDate.Date <= w.End.Date)?.Number ?? -1;
            return week;
        }
    }
}
