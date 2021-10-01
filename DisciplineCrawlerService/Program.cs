using Autofac;
using CourseSearchService.Crawlers;
using CourseSearchService.Crawlers.Interfaces;
using Cs4rsaDatabaseService.DataProviders;
using DisciplineCrawlerService.Crawlers;
using HelperService;
using System;

namespace DisciplineCrawlerService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new CourseCrawler()).As<ICourseCrawler>().SingleInstance();
            builder.RegisterInstance(new Cs4rsaDbContext()).AsSelf();
            builder.RegisterType<ColorGenerator>().AsSelf().SingleInstance();
            builder.RegisterType<DisciplineCrawler>().AsSelf();
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var writer = scope.Resolve<DisciplineCrawler>();
                var db = scope.Resolve<Cs4rsaDbContext>();
                db.Database.EnsureCreated();
                writer.GetDisciplineAndKeywordDatabase();
            }
        }
    }
}
