using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Helpers;
using cs4rsa.Models;

namespace cs4rsa.Contexts
{
    class Cs4rsaDBContext: DbContext
    {
        private static readonly string DB_PATH = Helpers.Helpers.GetFilePathAtApp("Sample.db");
        public Cs4rsaDBContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {
            optionbuilder.UseSqlite(@"Data Source="+ DB_PATH);
        }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TeacherDetail> TeacherDetails { get; set; }
        public DbSet<Curriculum> Curriculums { get; set; }
    }
}
