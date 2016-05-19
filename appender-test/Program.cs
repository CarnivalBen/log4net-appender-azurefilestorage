using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace appender_test {
    class Program {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args) {

            var rand = new Random();

            for (var i = 0; i <= 1000; i++) {
                
                Console.WriteLine($"Log test #{i}");
                switch (rand.Next(1, 5)) {
                case 1:
                    log.Fatal($"Log test #{i}");
                    break;
                case 2:
                    log.Error($"Log test #{i}");
                    break;
                case 3:
                    log.Warn($"Log test #{i}");
                    break;
                case 4:
                    log.Info($"Log test #{i}");
                    break;
                default:
                    log.Debug($"Log test #{i}");
                    break;
                }
                Thread.Sleep(300);

            }
            Console.WriteLine("done");
            Console.ReadKey();

        }
    }
}
