using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeattleCollectionCalendarWrapper;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            SeattleCollectionCalendarClient client = new SeattleCollectionCalendarClient();
//            Console.WriteLine(client.GetCcAddress("8745 GREENWOOD AVE N").FirstOrDefault());
            var result = client.GetCollectionDaysByAddress("8745 GREENWOOD AVE N");
            Console.WriteLine("");
        }
    }
}
