using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsaDatabaseService.Implements
{
    public class StudentImageRepository : GenericRepository<StudentImage>, IStudentImageRepository
    {
        public StudentImageRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy danh sách các hình ảnh sinh viên
        /// </summary>
        /// <param name="func">Cột sắp xếp</param>
        /// <param name="pageIndex">Trang</param>
        /// <param name="take">Số lượng</param>
        /// <returns></returns>
        public IEnumerable<StudentImage> Get(
            Func<StudentImage, object> func
            , int pageIndex = 1
            , int take = 50
            )
        {
            int skip = (pageIndex - 1) * take;
            return _context.StudentImages
                .OrderBy(func)
                .Skip(skip)
                .Take(take);
        }
    }
}
