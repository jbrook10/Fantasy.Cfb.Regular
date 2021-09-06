using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Fantasy.Cfb.Regular.Domain;
using HtmlAgilityPack;

namespace Fantasy.Cfb.Regular.Business
{
    public class RosterBuilder
    {
        static readonly HttpClient client = new HttpClient();
        private AmazonS3Client _s3Client;



        public async Task BuildRoster(int year)
        {
            _s3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            var rushUri = $"https://widgets.sports-reference.com/wg.fcgi?site=cfb&url=%2Fcfb%2Fyears%2F{year}-rushing.html&div=div_rushing";
            var passUri = $"https://widgets.sports-reference.com/wg.fcgi?site=cfb&url=%2Fcfb%2Fyears%2F{year}-passing.html&div=div_passing";
            var recUri = $"https://widgets.sports-reference.com/wg.fcgi?site=cfb&url=%2Fcfb%2Fyears%2F{year}-receiving.html&div=div_receiving";

            HtmlDocument rushDoc = await GetHtmlDocumentAsync(rushUri);
            HtmlDocument passDoc = await GetHtmlDocumentAsync(passUri);
            HtmlDocument recDoc = await GetHtmlDocumentAsync(recUri);

            var rosterFileResponse = await _s3Client.GetObjectAsync("cfb-regular", $"data/{year}.Roster.Raw.txt");
            StreamReader rosterReader = new StreamReader(rosterFileResponse.ResponseStream);
            string rosterContent = rosterReader.ReadToEnd();

            var splitChar = rosterContent.Contains("\r\n") ? "\r\n" : "\n";
            var rawRosterEntries = rosterContent.Split(splitChar);

            var roster = await MergeRosterAsync(rawRosterEntries, rushDoc, passDoc, recDoc);
            
            var s = new StringBuilder();
            foreach (var entry in roster)
            {
                s.AppendLine(entry.RosterText());
            }
            // var path = $@"D:\jerem\Documents\code\fantasy\Fantasy.Mlb.Lifetime\Fantasy.Mlb.Lifetime.Data\{year}\Roster.Final.txt";
            // File.WriteAllText(path, s.ToString());


            var putObjectRequest = new Amazon.S3.Model.PutObjectRequest() {
                BucketName = "cfb-regular",
                Key = $"data/{year}.Roster.Regular.txt",
                ContentBody = s.ToString(),
                ContentType = "text/plain"
            };
            await _s3Client.PutObjectAsync(putObjectRequest);
        }

        private async Task<List<RosterEntry>> MergeRosterAsync(string[] rawRosterEntries, HtmlDocument rushDoc, HtmlDocument passDoc, HtmlDocument recDoc)
        {
            var entries = new List<RosterEntry>();
            foreach (var rawEntry in rawRosterEntries)
            {
                var rosterEntry = new RosterEntry(rawEntry);
                if (string.IsNullOrEmpty(rosterEntry.Link))
                {
                    var searchPath = $"//td[a=\"{HtmlDocument.HtmlEncode(rosterEntry.Name)}\"]/a";
                    var passPlayer = passDoc.DocumentNode.SelectSingleNode(searchPath);
                    var rushPlayer = rushDoc.DocumentNode.SelectSingleNode(searchPath);
                    var recPlayer = recDoc.DocumentNode.SelectSingleNode(searchPath);

                    var nodeToCheck = passPlayer ?? rushPlayer ?? recPlayer;

                    if (nodeToCheck != null)
                    {
                        rosterEntry.Link = nodeToCheck.ParentNode.GetAttributeValue("data-append-csv", string.Empty);
                        var playerRow = nodeToCheck.ParentNode.ParentNode;
                        var schoolNode = playerRow.SelectSingleNode("td[@data-stat='school_name']/a");
                        rosterEntry.School = schoolNode.InnerHtml;
                    }

                }
                entries.Add(rosterEntry);
            }

            return entries;


        }

        private async Task<HtmlDocument> GetHtmlDocumentAsync(string rushUri)
        {
            var content = await client.GetStringAsync(rushUri);
            var start = content.IndexOf("<table");
            var end = content.IndexOf("</table>") + 8;
            var parsedContent = content.Substring(start, end - start);
            var html = $"<html><body>{parsedContent}</body></html>";

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc;
        }
    }
}