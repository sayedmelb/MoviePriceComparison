using Castle.DynamicProxy;
using MoviePriceComparison.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviePriceComparison.API.Test.Helpers
{
    internal class TestData
    {
        public static MovieSummary GetSampleMovies(string provider)
        {
            if (provider == "filmworld")
                return GetFilmWorldMovies();
            else
                return GetCinemaWorldMovies();
        }
        public static MovieSummary GetCinemaWorldMovies()
        {
            var movies = new List<Movie>
            {
                new() {
                    Title = "Star Wars: Episode IV - A New Hope",
                    Year = "1977",
                    ID= "cw0076759",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTcwOTk@._V1_SX300.jpg"
                },
                new() {
                    Title = "Star Wars: Episode V - The Empire Strikes Back",
                    Year = "1980",
                    ID= "cw0080684",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMjE2MzQwMTgxN15BMl5BanBnXkFtZTcwMDQzNjk2OQ@@._V1_SX300.jpg"
                },
                new() {
                    Title ="Star Wars: Episode VI - Return of the Jedi",
                    Year = "1983",
                    ID= "cw0086190",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMTQ0MzI1NjYwOF5BMl5BanBnXkFtZTgwODU3NDU2MTE@._V1._CR93,97,1209,1861_SX89_AL_.jpg_V1_SX300.jpg"
                },
                new()
                {
                    Title = "Star Wars: The Force Awakens",
                    Year = "2015",
                    ID= "cw2488496",
                    Type=  "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BOTAzODEzNDAzMl5BMl5BanBnXkFtZTgwMDU1MTgzNzE@._V1_SX300.jpg"
                },
                new() {
                    Title = "Star Wars: Episode I - The Phantom Menace",
                    Year = "1999",
                    ID= "cw0120915",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMTQ4NjEwNDA2Nl5BMl5BanBnXkFtZTcwNDUyNDQzNw@@._V1_SX300.jpg"
                },
                new() {
                    Title = "Star Wars: Episode III - Revenge of the Sith",
                    Year = "2005",
                    ID= "cw0121766",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BNTc4MTc3NTQ5OF5BMl5BanBnXkFtZTcwOTg0NjI4NA@@._V1_SX300.jpg"
                },
                 new() {
                    Title = "Star Wars: Episode II - Attack of the Clones",
                    Year = "2002",
                    ID= "cw0121765",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMTY5MjI5NTIwNl5BMl5BanBnXkFtZTYwMTM1Njg2._V1_SX300.jpg"
                }
            };

            return new MovieSummary { Movies = movies };
        }

        public static MovieSummary GetFilmWorldMovies()
        {
            var movies = new List<Movie>
            {
                new() {
                    Title = "Star Wars: Episode IV - A New Hope",
                    Year = "1977",
                    ID= "fw0076759",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTcwOTk@._V1_SX300.jpg"
                },
                new() {
                    Title = "Star Wars: Episode V - The Empire Strikes Back",
                    Year = "1980",
                    ID= "fw0080684",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMjE2MzQwMTgxN15BMl5BanBnXkFtZTcwMDQzNjk2OQ@@._V1_SX300.jpg"
                },
                new() {
                    Title ="Star Wars: Episode VI - Return of the Jedi",
                    Year = "1983",
                    ID= "fw0086190",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMTQ0MzI1NjYwOF5BMl5BanBnXkFtZTgwODU3NDU2MTE@._V1._CR93,97,1209,1861_SX89_AL_.jpg_V1_SX300.jpg"
                },
                new() {
                    Title = "Star Wars: Episode I - The Phantom Menace",
                    Year = "1999",
                    ID= "fw0120915",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMTQ4NjEwNDA2Nl5BMl5BanBnXkFtZTcwNDUyNDQzNw@@._V1_SX300.jpg"
                },
                new() {
                    Title = "Star Wars: Episode III - Revenge of the Sith",
                    Year = "2005",
                    ID= "fw0121766",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BNTc4MTc3NTQ5OF5BMl5BanBnXkFtZTcwOTg0NjI4NA@@._V1_SX300.jpg"
                },
                 new() {
                    Title = "Star Wars: Episode II - Attack of the Clones",
                    Year = "2002",
                    ID= "fw0121765",
                    Type= "movie",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMTY5MjI5NTIwNl5BMl5BanBnXkFtZTYwMTM1Njg2._V1_SX300.jpg"
                }
            };

            return new MovieSummary { Movies = movies };
        }


        public static MovieDetail GetSampleMovieDetails(string provider, string movieId)
        {
            var movieDetails = new List<MovieDetail>
            {
                new()
                {
                    Title="Star Wars: Episode II - Attack of the Clones",
                    Year="2002",
                    Rated="PG",
                    Released="16 May 2002",
                    Runtime="142 min",
                    Genre="Action, Adventure, Fantasy",
                    Director="George Lucas",
                    Writer="George Lucas (screenplay), Jonathan Hales (screenplay), George Lucas (story by)",
                    Actors="Ewan McGregor, Natalie Portman, Hayden Christensen, Christopher Lee",
                    Plot="Ten years after initially meeting, Anakin Skywalker shares a forbidden romance with Padmé, while Obi-Wan investigates an assassination attempt on the Senator and discovers a secret clone army crafted for the Jedi.",
                    Language="English",
                    Country="USA",
                    Poster="https://m.media-amazon.com/images/M/MV5BNDRkYzA4OGYtOTBjYy00YzFiLThhYmYtMWUzMDBmMmZkM2M3XkEyXkFqcGdeQXVyNDYyMDk5MTU@._V1_SX300.jpg",
                    Metascore="54",
                    Rating="6.7",
                    Votes="469,134",
                    ID="fw0121765",
                    Type="movie",
                    Price="1249.5",
                    Provider = "filmworld",
                    MovieId="fw0121765"
                },
                 new()
                {
                    Title= "Star Wars: Episode IV - A New Hope",
                    Year= "1977",
                    Rated= "PG",
                    Released= "25 May 1977",
                    Runtime= "121 min",
                    Genre= "Action, Adventure, Fantasy",
                    Director= "George Lucas",
                    Writer= "George Lucas",
                    Actors= "Mark Hamill, Harrison Ford, Carrie Fisher, Peter Cushing",
                    Plot= "Luke Skywalker joins forces with a Jedi Knight, a cocky pilot, a wookiee and two droids to save the galaxy from the Empire's world-destroying battle-station, while also attempting to rescue Princess Leia from the evil Darth Vader.",
                    Language= "English",
                    Country= "USA",
                    Poster= "https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTfwOTk@._V1_SX300.jpg",
                    Metascore= "92",
                    Rating= "8.7",
                    Votes= "915,459",
                    ID= "fw0076759",
                    Type= "movie",
                    Price= "29.5",
                    Provider = "filmworld",
                    MovieId="fw0076759"
                },
                new()
                 {
                    Title= "Star Wars: Episode VI - Return of the Jedi",
                    Year= "1983",
                    Rated= "PG",
                    Released= "25 May 1983",
                    Runtime= "131 min",
                    Genre= "Action, Adventure, Fantasy",
                    Director= "Richard Marquand",
                    Writer= "Lawrence Kasdan (screenplay), George Lucas (screenplay), George Lucas (story by)",
                    Actors= "Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams",
                    Plot= "After rescuing Han Solo from the palace of Jabba the Hutt, the rebels attempt to destroy the second Death Star, while Luke struggles to make Vader return from the dark side of the Force.",
                    Language= "English",
                    Country= "USA",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMTQ0MzI1NjYwOF5BMl5BanBnXkFtZTgwODU3NDU2MTE@._V1._CR93,97,1209,1861_SX89_AL_.jpg_V1_SX300.jpg",
                    Metascore= "53",
                    Rating= "8.4",
                    Votes= "686,479",
                    ID= "fw0086190",
                    Type= "movie",
                    Price= "69.5",
                    Provider = "filmworld",
                    MovieId="fw0086190"
                },
                new()
                {
                    Title= "Star Wars: Episode V - The Empire Strikes Back",
                    Year= "1980",
                    Rated= "PG",
                    Released= "20 Jun 1980",
                    Runtime= "124 min",
                    Genre= "Action, Adventure, Fantasy",
                    Director= "Irvin Kershner",
                    Writer= "Leigh Brackett (screenplay), Lawrence Kasdan (screenplay), George Lucas (story by)",
                    Actors= "Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams",
                    Plot= "After the Rebel base on the icy planet Hoth is taken over by the Empire, Han, Leia, Chewbacca, and C-3PO flee across the galaxy from the Empire. Luke travels to the forgotten planet of Dagobah to receive training from the Jedi master Yoda, while Vader endlessly pursues him.",
                    Language= "English",
                    Country= "USA",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMjE2MzQwMTgxN15BMl5BanBnXkFtZTfwMDQzNjk2OQ@@._V1_SX300.jpg",
                    Metascore= "80",
                    Rating= "8.8",
                    Votes= "842,451",
                    ID= "fw0080684",
                    Type= "movie",
                    Price= "1295.0",
                    Provider = "filmworld",
                    MovieId="fw0080684"
                },
                new()
                {
                    Year= "1999",
                    Title= "Star Wars: Episode I - The Phantom Menace",
                    Rated= "PG",
                    Released= "19 May 1999",
                    Runtime= "136 min",
                    Genre= "Action, Adventure, Fantasy",
                    Director= "George Lucas",
                    Writer= "George Lucas",
                    Actors= "Liam Neeson, Ewan McGregor, Natalie Portman, Jake Lloyd",
                    Plot= "Two Jedi Knights escape a hostile blockade to find allies and come across a young boy who may bring balance to the Force, but the long dormant Sith resurface to reclaim their old glory.",
                    Language= "English",
                    Country= "USA",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMTQ4NjEwNDA2Nl5BMl5BanBnXkFtZTfwNDUyNDQzNw@@._V1_SX300.jpg",
                    Metascore= "51",
                    Rating= "6.5",
                    Votes= "537,242",
                    ID= "fw0120915",
                    Type= "movie",
                    Price= "900.5",
                    Provider = "filmworld",
                    MovieId="fw0120915"
                },
                new()
                {
                    Title= "Star Wars: Episode III - Revenge of the Sith",
                    Year= "2005",
                    Rated= "PG-13",
                    Released= "19 May 2005",
                    Runtime= "140 min",
                    Genre= "Action, Adventure, Fantasy",
                    Director= "George Lucas",
                    Writer= "George Lucas",
                    Actors= "Ewan McGregor, Natalie Portman, Hayden Christensen, Ian McDiarmid",
                    Plot= "During the near end of the clone wars, Darth Sidious has revealed himself and is ready to execute the last part of his plan to rule the Galaxy. Sidious is ready for his new apprentice, Lord...",
                    Language= "English",
                    Country= "USA",
                    Poster= "https://m.media-amazon.com/images/M/MV5BNTc4MTc3NTQ5OF5BMl5BanBnXkFtZTfwOTg0NjI4NA@@._V1_SX300.jpg",
                    Metascore= "68",
                    Rating= "7.6",
                    Votes= "522,705",
                    ID= "fw0121766",
                    Type= "movie",
                    Price= "129.9",
                    Provider = "filmworld",
                    MovieId="fw0121766"
                },
                new()
                {
                    Title="Star Wars: Episode II - Attack of the Clones",
                    Year="2002",
                    Rated="PG",
                    Released="16 May 2002",
                    Runtime="142 min",
                    Genre="Action, Adventure, Fantasy",
                    Director="George Lucas",
                    Writer="George Lucas (screenplay), Jonathan Hales (screenplay), George Lucas (story by)",
                    Actors="Ewan McGregor, Natalie Portman, Hayden Christensen, Christopher Lee",
                    Plot="Ten years after initially meeting, Anakin Skywalker shares a forbidden romance with Padmé, while Obi-Wan investigates an assassination attempt on the Senator and discovers a secret clone army crafted for the Jedi.",
                    Language="English",
                    Country="USA",
                    Poster="https://m.media-amazon.com/images/M/MV5BNDRkYzA4OGYtOTBjYy00YzFiLThhYmYtMWUzMDBmMmZkM2M3XkEyXkFqcGdeQXVyNDYyMDk5MTU@._V1_SX300.jpg",
                    Metascore="54",
                    Rating="6.7",
                    Votes="469,134",
                    ID="cw0121765",
                    Type="movie",
                    Price="1259.5",
                    Provider = "cinemaworld",
                    MovieId="cw0121765"
                },
                new()
                {
                    Title= "Star Wars: Episode IV - A New Hope",
                    Year= "1977",
                    Rated= "PG",
                    Released= "25 May 1977",
                    Runtime= "121 min",
                    Genre= "Action, Adventure, Fantasy",
                    Director= "George Lucas",
                    Writer= "George Lucas",
                    Actors= "Mark Hamill, Harrison Ford, Carrie Fisher, Peter Cushing",
                    Plot= "Luke Skywalker joins forces with a Jedi Knight, a cocky pilot, a wookiee and two droids to save the galaxy from the Empire's world-destroying battle-station, while also attempting to rescue Princess Leia from the evil Darth Vader.",
                    Language= "English",
                    Country= "USA",
                    Poster= "https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTfwOTk@._V1_SX300.jpg",
                    Metascore= "92",
                    Rating= "8.7",
                    Votes= "915,459",
                    ID= "cw0076759",
                    Type= "movie",
                    Price= "19.5",
                    Provider = "cinemaworld",
                    MovieId="cw0076759"
                },
                new()
                {
                    Title= "Star Wars: Episode VI - Return of the Jedi",
                    Year= "1983",
                    Rated= "PG",
                    Released= "25 May 1983",
                    Runtime= "131 min",
                    Genre= "Action, Adventure, Fantasy",
                    Director= "Richard Marquand",
                    Writer= "Lawrence Kasdan (screenplay), George Lucas (screenplay), George Lucas (story by)",
                    Actors= "Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams",
                    Plot= "After rescuing Han Solo from the palace of Jabba the Hutt, the rebels attempt to destroy the second Death Star, while Luke struggles to make Vader return from the dark side of the Force.",
                    Language= "English",
                    Country= "USA",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMTQ0MzI1NjYwOF5BMl5BanBnXkFtZTgwODU3NDU2MTE@._V1._CR93,97,1209,1861_SX89_AL_.jpg_V1_SX300.jpg",
                    Metascore= "53",
                    Rating= "8.4",
                    Votes= "686,479",
                    ID= "cw0086190",
                    Type= "movie",
                    Price= "70.5",
                    Provider = "cinemaworld",
                    MovieId="cw0086190"
                },
                new()
                {
                    Title= "Star Wars: Episode V - The Empire Strikes Back",
                    Year= "1980",
                    Rated= "PG",
                    Released= "20 Jun 1980",
                    Runtime= "124 min",
                    Genre= "Action, Adventure, Fantasy",
                    Director= "Irvin Kershner",
                    Writer= "Leigh Brackett (screenplay), Lawrence Kasdan (screenplay), George Lucas (story by)",
                    Actors= "Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams",
                    Plot= "After the Rebel base on the icy planet Hoth is taken over by the Empire, Han, Leia, Chewbacca, and C-3PO flee across the galaxy from the Empire. Luke travels to the forgotten planet of Dagobah to receive training from the Jedi master Yoda, while Vader endlessly pursues him.",
                    Language= "English",
                    Country= "USA",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMjE2MzQwMTgxN15BMl5BanBnXkFtZTfwMDQzNjk2OQ@@._V1_SX300.jpg",
                    Metascore= "80",
                    Rating= "8.8",
                    Votes= "842,451",
                    ID= "cw0080684",
                    Type= "movie",
                    Price= "1285.0",
                    Provider = "cinemaworld",
                    MovieId="cw0080684"
                },
                new()
                {
                    Year= "1999",
                    Title= "Star Wars: Episode I - The Phantom Menace",
                    Rated= "PG",
                    Released= "19 May 1999",
                    Runtime= "136 min",
                    Genre= "Action, Adventure, Fantasy",
                    Director= "George Lucas",
                    Writer= "George Lucas",
                    Actors= "Liam Neeson, Ewan McGregor, Natalie Portman, Jake Lloyd",
                    Plot= "Two Jedi Knights escape a hostile blockade to find allies and come across a young boy who may bring balance to the Force, but the long dormant Sith resurface to reclaim their old glory.",
                    Language= "English",
                    Country= "USA",
                    Poster= "https://m.media-amazon.com/images/M/MV5BMTQ4NjEwNDA2Nl5BMl5BanBnXkFtZTfwNDUyNDQzNw@@._V1_SX300.jpg",
                    Metascore= "51",
                    Rating= "6.5",
                    Votes= "537,242",
                    ID= "cw0120915",
                    Type= "movie",
                    Price= "901.5",
                    Provider = "cinemaworld",
                    MovieId="cw0120915"
                },
                new()
                {
                    Title= "Star Wars: Episode III - Revenge of the Sith",
                    Year= "2005",
                    Rated= "PG-13",
                    Released= "19 May 2005",
                    Runtime= "140 min",
                    Genre= "Action, Adventure, Fantasy",
                    Director= "George Lucas",
                    Writer= "George Lucas",
                    Actors= "Ewan McGregor, Natalie Portman, Hayden Christensen, Ian McDiarmid",
                    Plot= "During the near end of the clone wars, Darth Sidious has revealed himself and is ready to execute the last part of his plan to rule the Galaxy. Sidious is ready for his new apprentice, Lord...",
                    Language= "English",
                    Country= "USA",
                    Poster= "https://m.media-amazon.com/images/M/MV5BNTc4MTc3NTQ5OF5BMl5BanBnXkFtZTfwOTg0NjI4NA@@._V1_SX300.jpg",
                    Metascore= "68",
                    Rating= "7.6",
                    Votes= "522,705",
                    ID= "cw0121766",
                    Type= "movie",
                    Price= "129.9",
                    Provider = "cinemaworld",
                    MovieId="cw0121766"
                }
            };

            return movieDetails.Where(w => w.MovieId == movieId && provider == provider).FirstOrDefault();
        }

        public static List<ProviderStatus> GetSampleProviderStatus()
        {
            return new List<ProviderStatus>
            {
                new() { Provider = "cinemaworld", IsHealthy = true, Status = "healthy", ResponseTime = TimeSpan.FromMilliseconds(250), LastChecked = DateTime.UtcNow },
                new() { Provider = "filmworld", IsHealthy = true, Status = "healthy", ResponseTime = TimeSpan.FromMilliseconds(180), LastChecked = DateTime.UtcNow }
            };
        }
    }
}
