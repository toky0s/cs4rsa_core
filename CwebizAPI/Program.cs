using System.Text;
using System.Text.Json.Serialization;
using Algolia.Search.Clients;
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
using CwebizAPI.Crawlers.SubjectCrawlerSvc.Crawlers;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.Crawlers.Interfaces;
using CwebizAPI.Crawlers.TeacherCrawlerSvc.Crawlers;
using CwebizAPI.Crawlers.TeacherCrawlerSvc.Crawlers.Interfaces;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.Jobs.DisciplineJob;
using CwebizAPI.Middlewares;
using CwebizAPI.Services;
using CwebizAPI.Services.Interfaces;
using CwebizAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CwebizAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            ConfigurationManager configuration = builder.Configuration;
            
            #region JWT Authentication

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = configuration["JwtForRequest:Issuer"],
                    ValidAudience = configuration["JwtForRequest:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtForRequest:Key"]!)),
                    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RequireExpirationTime = true
                };
            });
            #endregion
            
            #region Services
            // Add services to the container.
            builder.Services.AddLogging();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IJwtTokenSvc, JwtTokenSvc>();
            builder.Services.AddScoped<IImageStorageSvc, ImageStorageSvc>();
            builder.Services.AddScoped<ISvcSubjectCvt, SvcSubjectCvt>();
            builder.Services.AddScoped<ColorGenerator>();
            #endregion

            #region Crawler
            builder.Services.AddScoped<HtmlWeb>();
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddScoped<CourseCrawler>();
            builder.Services.AddScoped<DisciplineCrawler>();
            builder.Services.AddScoped<ImageDownloader>();
            builder.Services.AddScoped<IDtuStudentInfoCrawler, DtuStudentInfoCrawlerV2>();
            builder.Services.AddScoped<ICurriculumCrawler, CurriculumCrawler>();
            builder.Services.AddScoped<ISpecialStringCrawler, SpecialStringCrawlerV2>();
            builder.Services.AddScoped<ISubjectCrawler, SubjectCrawler>();
            builder.Services.AddScoped<ITeacherCrawler, TeacherCrawler>();
            #endregion
            
            #region Businesses
            builder.Services.AddScoped<BuDiscipline>();
            builder.Services.AddScoped<BuRegister>();
            builder.Services.AddScoped<BuLogin>();
            builder.Services.AddScoped<BuUser>();
            builder.Services.AddScoped<BuSubject>();
            #endregion

            #region CORS

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(Policies.CredizBlazorPolicy, policy =>
                {
                    policy.WithOrigins(configuration.GetSection("Origins:Fe").Value!)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            #endregion

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
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(12, 00))
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

            #region Algolia Search
            
            builder.Services.AddSingleton<ISearchClient>(new SearchClient(
                configuration["AlgoliaSearch:ApplicationID"], 
                configuration["AlgoliaSearch:AdminAPIKey"])
            );

            #endregion
                
            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    JsonStringEnumConverter enumConverter = new();
                    options.JsonSerializerOptions.Converters.Add(enumConverter);
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler("/Error");

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthentication();
            app.UseUserClaims();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}