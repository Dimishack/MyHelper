using MyHelper.DAL.Entyties;
using MyHelper.Interfaces;
using MyHelper.ViewModels.Base;

namespace MyHelper.ViewModels
{
    internal sealed class MovieUCViewModel(IRepository<Movie> movieRepository,
                                           IRepository<Genre> genreRepository,
                                           IRepository<MovieGenre> movieGenreRepository) : MainFunctionsViewModel<Movie>(movieRepository)
    {
        private readonly IRepository<Genre> _genreRepository = genreRepository;
        private readonly IRepository<MovieGenre> _movieGenreRepository = movieGenreRepository;

        protected override Task OnAddElementCommandExecuted(object? p)
        {
            throw new NotImplementedException();
        }

        protected override void OnClosedCommandExecuted(object? p)
        {
            throw new NotImplementedException();
        }

        protected override Task OnDeleteElementCommandExecuted(object? p)
        {
            throw new NotImplementedException();
        }

        protected override Task OnEditElementCommandExecuted(object? p)
        {
            throw new NotImplementedException();
        }

        protected override void OnLoadedCommandExecuted(object? p)
        {
            throw new NotImplementedException();
        }
    }
}
