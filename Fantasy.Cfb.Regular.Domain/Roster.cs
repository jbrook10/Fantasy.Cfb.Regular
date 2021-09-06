using System;

namespace Fantasy.Cfb.Regular.Domain
{
    public class RosterEntry
    {
        public RosterEntry(string rawEntry)
        {
            var parts = rawEntry.Split('|', System.StringSplitOptions.RemoveEmptyEntries);
            Owner = parts[0].Trim();
            Name = parts[1].Trim();


            if (Enum.TryParse(parts[2].Trim(), out PositionType position))
            {
                Position = position;
            }
            else
            {
                Position = PositionType.none;
            }

            if (parts.Length > 3) {
                School = parts[3];
            }

            Link = parts.Length >= 4 ? parts[4].Trim() : string.Empty;
        }

        public string Owner { get; set; }
        public string Name { get; set; }
        public PositionType Position { get; set; }
        public string School { get; set; }
        public string Link { get; set; }

        public string RosterText()
        {
            return $"{Owner} | {Name} | {PostitionText()} | {School} | {Link}";
        }

        public string PostitionText()
        {
            return Position.ToString();
        }
    }
}