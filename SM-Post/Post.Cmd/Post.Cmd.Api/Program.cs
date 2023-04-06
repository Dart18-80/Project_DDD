using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Stores;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Dispatchers;
using Post.Cmd.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
builder.Services.AddScoped<ICommandHandler, CommandHandler>();

//Register Command Handler Methods
var CommandHandler = builder.Services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
var dispatcher = new CommandDispatcher();

dispatcher.RegisterHandler<NewPostCommand>(CommandHandler.HandleAsync);
dispatcher.RegisterHandler<EditMessageCommand>(CommandHandler.HandleAsync);
dispatcher.RegisterHandler<LikePostCommand>(CommandHandler.HandleAsync);
dispatcher.RegisterHandler<AddCommentCommand>(CommandHandler.HandleAsync);
dispatcher.RegisterHandler<EditCommentCommand>(CommandHandler.HandleAsync);
dispatcher.RegisterHandler<RemoveCommentCommand>(CommandHandler.HandleAsync);
dispatcher.RegisterHandler<DeletePostCommand>(CommandHandler.HandleAsync);

builder.Services.AddSingleton<ICommandDispacher>( _ => dispatcher);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
