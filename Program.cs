using System.Threading.Tasks;
using StreamDeckLib;

namespace SoundFader
{
    class Program
    {

        static async Task Main(string[] args)
        {
            using (var config = StreamDeckLib.Config.ConfigurationBuilder.BuildDefaultConfiguration(args))
            {

                await ConnectionManager.Initialize(args, config.LoggerFactory, new CustomSdProxy())
                                                             .RegisterAllActions(typeof(Program).Assembly)
                                                             .StartAsync();

            }

        }

    }
}
