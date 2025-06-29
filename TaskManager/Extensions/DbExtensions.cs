using Microsoft.EntityFrameworkCore;
using TaskManager.EfCore;

namespace TaskManager.Extensions;

public static class DbExtensions
{
    public static void CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ProjectTaskContext>();
        try
        {
            // Create the database if it doesn't exist
            context.Database.SetCommandTimeout(3);
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var t = ex;
        }
    }
}