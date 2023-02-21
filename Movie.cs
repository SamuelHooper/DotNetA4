namespace DotNetA4
{
    public class Movie
    {
        public ulong Id { get; set; }
        public string Title { get; set; }
        public List<string> Genres { get; set; }

        public Movie()
        {
            Genres = new List<string>();
        }

        public string DisplayMovie()
        {
            string info = $"{Id}. \"{Title}\" | ";
            for (int i = 0; i < Genres.Count; i++)
            {
                // Add proper punctuation and grammer
                switch (Genres.Count)
                {
                    case 1: info += $"{Genres[0]}"; break;
                    case 2: info += $"{Genres[0]} and {Genres[1]}"; i++; break;
                    default:
                        if (i == Genres.Count - 1)
                        {
                            info += $"and {Genres[i]}";
                        }
                        else
                        {
                            info += $"{Genres[i]}, ";
                        }
                        break;
                }
            }
            return info;
        }

        public override string ToString()
        {
            return $"{Id},{Title},{String.Join('|', Genres.ToArray())}";
        }
    }
}