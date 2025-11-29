using System.Reflection;
using Blueflow.Chopper.Cli;
using Spectre.Console.Cli;

var app = new CommandApp<SplitCommand>();

app.Configure(config =>
{
    config.SetApplicationName("blueflow-chopper");
    var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";
    config.SetApplicationVersion(version);
});

return await app.RunAsync(args);