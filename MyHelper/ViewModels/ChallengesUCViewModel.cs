using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Commands.Base;
using MyHelper.Interfaces;
using MyHelper.Models.Challenges;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal class ChallengesUCViewModel(IRepository<Challenge> challengeRepository, IRepository<Check> checkRepository) : ViewModel
    {
        private readonly IRepository<Challenge> _challengeRepository = challengeRepository;
        private readonly IRepository<Check> _checkRepository = checkRepository;





        public ChallengesUCViewModel() : this(null,null) { }
    }
}
