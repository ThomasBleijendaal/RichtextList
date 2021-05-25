using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RichText.Abstractions;
using RichText.Commands;
using RichText.Entities;
using RichText.Handlers.CommandHandlers;
using RichText.Handlers.QueryHandlers;
using RichText.Queries;
using RichText.Resolvers;
using RichText.Services;
using RichText.State;

namespace RichText
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSingleton<IAppState, AppState>();

            services.AddSingleton<IEntityService, EntityService>();

            services.AddSingleton<IQueryHandler<GetMetaQuery, Meta>, GetMetaQueryHandler>();
            services.AddSingleton<IQueryHandler<GetFieldQuery, Field>, GetFieldQueryHandler>();
            services.AddSingleton<IQueryHandler<GetBoardsQuery, IReadOnlyList<Board>>, GetBoardsQueryHandler>();
            services.AddSingleton<IQueryHandler<GetEpicsQuery, IReadOnlyList<Epic>>, GetEpicsQueryHandler>();
            services.AddSingleton<IQueryHandler<GetUserStoriesQuery, IReadOnlyList<UserStory>>, GetUserStoriesQueryHandler>();

            services.AddSingleton<IResultResolver<Meta>, MetaResolver>();
            services.AddSingleton<IResultsResolver<Board>, BoardResolver>();
            services.AddSingleton<IResultsResolver<Epic>, EpicResolver>();
            services.AddSingleton<IResultsResolver<UserStory>, UserStoryResolver>();

            services.AddSingleton<ICommandHandler<AssignToEpicCommand>, AssignToEpicCommandHandler>();
            services.AddSingleton<ICommandHandler<DeleteEpicCommand>, DeleteCommandHandler>();
            services.AddSingleton<ICommandHandler<DeleteSubTaskCommand>, DeleteCommandHandler>();
            services.AddSingleton<ICommandHandler<DeleteUserStoryCommand>, DeleteCommandHandler>();
            services.AddSingleton<ICommandHandlerWithResponse<DemoteEpicCommand, IEntity>, DemoteEpicCommandHandler>();
            services.AddSingleton<ICommandHandlerWithResponse<DemoteUserStoryCommand, IEntity>, DemoteUserStoryCommandHandler>();
            services.AddSingleton<ICommandHandlerWithResponse<PromoteSubTaskCommand, IEntity>, PromoteSubTaskCommandHandler>();
            services.AddSingleton<ICommandHandlerWithResponse<PromoteUserStoryCommand, IEntity>, PromoteUserStoryCommandHandler>();
            services.AddSingleton<ICommandHandler<UpsertEpicCommand>, UpsertEpicCommandHandler>();
            services.AddSingleton<ICommandHandler<UpsertSubTaskCommand>, UpsertSubTaskCommandHandler>();
            services.AddSingleton<ICommandHandler<UpsertUserStoryCommand>, UpsertUserStoryCommandHandler>();

            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
