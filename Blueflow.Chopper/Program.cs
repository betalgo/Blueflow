using Blueflow.Chopper.Cli;
using Spectre.Console.Cli;

var app = new CommandApp<SplitCommand>();

app.Configure(config =>
{
    config.SetApplicationName("blueflow-chopper");
    config.SetApplicationVersion("1.0.0");
});

return await app.RunAsync(args);