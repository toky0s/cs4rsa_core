using CwebizAPI.Businesses;
using CwebizAPI.Crawlers;
using CwebizAPI.Db;

using HtmlAgilityPack;

using Quartz;
using CwebizAPI.Settings;
using CwebizAPI.Crawlers.CourseSearchSvc.Crawlers;
using CwebizAPI.Crawlers.CurriculumCrawlerSvc.Crawlers;
using CwebizAPI.Crawlers.CurriculumCrawlerSvc.Crawlers.Interfaces;
using CwebizAPI.Crawlers.DisciplineCrawlerSvc.Crawlers;
using CwebizAPI.Crawlers.StudentCrawlerSvc.Crawlers;
using CwebizAPI.Crawlers.StudentCrawlerSvc.Crawlers.Interfaces;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.Jobs.DisciplineJob;
using CwebizAPI.Services;
using CwebizAPI.Services.Interfaces;

namespace CwebizAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddLogging();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            #region Crawler
            builder.Services.AddScoped<HtmlWeb>();
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddScoped<CourseCrawler>();
            builder.Services.AddScoped<DisciplineCrawler>();
            builder.Services.AddScoped<ImageDownloader>();
            builder.Services.AddScoped<IDtuStudentInfoCrawler, DtuStudentInfoCrawlerV2>();
            builder.Services.AddScoped<ICurriculumCrawler, CurriculumCrawler>();
            builder.Services.AddScoped<ISpecialStringCrawler, SpecialStringCrawlerV2>();
            #endregion

            #region Services
            builder.Services.AddScoped<IJwtTokenSvc, JwtTokenSvc>();
            #endregion
            
            #region Businesses
            builder.Services.AddScoped<IImageStorageSvc, ImageStorageSvc>();
            builder.Services.AddScoped<BuDiscipline>();
            builder.Services.AddScoped<BuRegister>();
            #endregion

            builder.Services.AddCors(options =>
            {
                ConfigurationManager configuration = builder.Configuration;
                options.AddPolicy(Policies.CredizBlazorPolicy, policy =>
                {
                    policy.WithOrigins(configuration.GetSection("Origins:Fe").Value!)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region Quartz Jobs
            builder.Services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.AddJob<DisciplineJob>(opts => opts.WithIdentity(DisciplineJob.JobKey));
                q.AddTrigger(opts => opts
                    .ForJob(DisciplineJob.JobKey)
                    .WithIdentity(DisciplineJob.Identity)
                    .WithDailyTimeIntervalSchedule(quartzBuilder =>
                    {
                        quartzBuilder
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(23, 00))
                            .OnEveryDay()
                            .Build();
                    })
                );
            });

            builder.Services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
                options.AwaitApplicationStarted = true;
            });
            #endregion

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors();
            
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}