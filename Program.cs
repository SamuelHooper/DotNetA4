using DotNetA4;
using Trivial.CommandLine;

internal class Program
{
    public static void Main(string[] args)
    {
        // Declarations
        string file = $"{Environment.CurrentDirectory}\\data\\movies.csv";
        string menuOption;

        do
        {
            // Gets user input for menu navigation
            Console.WriteLine("1) List all movies.");
            Console.WriteLine("2) Add movie to file.");
            Console.WriteLine("Enter any other key to exit.");
            menuOption = Console.ReadLine();

            if (menuOption == "1" && File.Exists(file))
            {
                DisplayAllMovies(file);
            }
            else if (menuOption == "2" && File.Exists(file))
            {
                AddNewMovie(file);
            }
            else if (!File.Exists(file))
            {
                Console.WriteLine($"{file} does not exist.");
            }
        } while (menuOption == "1" || menuOption == "2");
    }

    public static List<string> AddGenres()
    {
        List<string> genres = new();
        // Loop to add genres
        while (true)
        {
            Console.Write("Enter a genre (or N to exit): ");
            string genre = Console.ReadLine();
            if (genre.ToUpper() != "N" && genre != null)
            {
                genres.Add($"{genre}");
            }
            else
            {
                if (genres.Count == 0)
                {
                    genres.Add($"(no genres listed)");
                }
                return genres;
            }
        }
    }

    public static void AddNewMovie(string file)
    {
        // Loop to add movies
        do
        {
            List<Movie> movies = GetMoviesList(file);
            List<string> lowerCaseMovieTitles = new();
            Movie newMovie = new();
            Console.Write("Enter the movie title: ");
            newMovie.Title = Console.ReadLine();

            // Check for duplicate movie title
            foreach (Movie movie in movies)
            {
                lowerCaseMovieTitles.Add(movie.Title.ToLower());
            }

            if (lowerCaseMovieTitles.Contains(newMovie.Title.ToLower()))
            {
                Console.WriteLine("That movie has already been entered.");
            }
            else
            {
                newMovie.Id = movies[^1].Id + 1;

                newMovie.Genres = AddGenres();

                // Check for comma in title
                newMovie.Title = newMovie.Title.IndexOf(',') != -1 ? $"\"{newMovie.Title}\"" : newMovie.Title;

                // Create newMovie in file and write to console
                Console.WriteLine(newMovie);
                StreamWriter sw = new StreamWriter(file, true);
                sw.WriteLine(newMovie);
                sw.Close();
            }
            Console.Write("\nAdd another movie (Y/N)? ");
        } while (Console.ReadLine().ToUpper() == "Y");
    }

    public static void DisplayAllMovies(string file)
    {
        // nuget Trival.Console (7.0.0) package for paging
        // It does have some bugs with press the down arrow keys too fast
        List<Movie> movies = GetMoviesList(file);
        var col = new Trivial.Collection.SelectionData<string>();
        for (int i = 0; i < movies.Count; i++)
        {
            // Add movie to display
            col.Add(movies[i].DisplayMovie());
        }

        // Create options for display
        var options = new SelectionConsoleOptions
        {
            Tips = "Tips: You can use arrow key to select and press ENTER key to exit.",
            TipsForegroundConsoleColor = ConsoleColor.Yellow,
            SelectedPrefix = "> ",
            Prefix = " ",
            Column = 2,
            MaxRow = 20
        };

        // Pause to let user read and and see full description of selected item
        DefaultConsole.Select(col, options);
        Console.WriteLine(); // Spacer line for returning to main menu
    }

    public static List<Movie> GetMoviesList(string file)
    {
        // Logic to read
        StreamReader sr = new(file);
        List<Movie> movies = new();

        sr.ReadLine(); // Skips header line
        while (!sr.EndOfStream)
        {
            Movie currentMovie = new();
            string line = sr.ReadLine();

            // Check for quotes
            int index = line.IndexOf('"');
            if (index == -1)
            {
                // If no quote act normally
                string[] movieLine = line.Split(',');
                currentMovie.Id = UInt64.Parse(movieLine[0]);
                currentMovie.Title = movieLine[1];
                currentMovie.Genres = new List<string>(movieLine[2].Split('|'));
            }
            else
            {
                // quote = comma in movie title
                // extract the movieId
                currentMovie.Id = UInt64.Parse(line.Substring(0, index - 1));
                // remove movieId and first quote from string
                line = line.Substring(index + 1);
                // find the next quote
                index = line.IndexOf('"');
                // extract the movieTitle
                currentMovie.Title = line.Substring(0, index);
                // remove title and last comma from the string
                line = line.Substring(index + 2);
                // replace the "|" with ", "
                currentMovie.Genres = new List<string>(line.Split('|'));
            }
            // Add movie to list
            movies.Add(currentMovie);
        }
        // close file when done
        sr.Close();
        return movies;
    }

}