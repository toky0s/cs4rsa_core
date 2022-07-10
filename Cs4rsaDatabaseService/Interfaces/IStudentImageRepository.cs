using Cs4rsaDatabaseService.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface IStudentImageRepository: IGenericRepository<StudentImage>
    {
        IEnumerable<StudentImage> Get(Func<StudentImage, object> func, int pageIndex = 1, int take = 50);
    }
}
