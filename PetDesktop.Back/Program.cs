using Microsoft.Extensions.Configuration;
using Serilog;

namespace PetDesktop.Back;

class Program
{
    static void Main(string[] args)
    {
        // 1. Construir el lector del archivo JSON
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // 2. Configurar Serilog usando ese JSON
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        // 3. ¡Listo para usar en cualquier parte del Back!
        Log.Information("El Backend se ha iniciado correctamente.");

        try
        {
            // Aquí iría el núcleo de tu backend (sockets, lógica, colas, etc.)
            Console.WriteLine("Presiona una tecla para detener el backend...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "El Backend terminó inesperadamente debido a un error crítico.");
        }
        finally
        {
            // Muy importante en aplicaciones de consola para asegurar que todos los logs en memoria se escriban al disco antes de cerrar.
            Log.CloseAndFlush();
        }
    }
}