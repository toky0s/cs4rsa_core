using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;

using System.Threading.Tasks;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.Interfaces
{
    public interface IStudentProgramCrawler
    {
        Task<ProgramFolder[]> GetProgramFolders(string specialString, int curid);
    }
}
