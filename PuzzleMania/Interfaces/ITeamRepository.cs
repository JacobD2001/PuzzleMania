using System.Drawing.Drawing2D;
using PuzzleMania.Models;

namespace PuzzleMania.Interfaces
{
    public interface ITeamRepository
    {
        bool CheckIfUserHasNullTeamId(string currentUserId);
        //nie widzi teamow bo nie ma ich w modelach tu a jest w bazie team model TODO
        Task<IEnumerable<Team>> GetAll();
        Task<IEnumerable<Team>> GetIncompleteTeams();

    }
}
