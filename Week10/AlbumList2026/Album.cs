using System.Text.Json.Serialization;

namespace AlbumList
{
    public class Album
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public string CoverUrl { get; set; }
        public List<string> Tracks { get; set; }
        [JsonIgnore]
        public IEnumerable<string> NumberedTracks => Tracks.Select((t, i) => $"{i + 1}. {t}");
    }
}