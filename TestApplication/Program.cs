using System;
using System.Linq;
using Kirinnee.Helper;
using narwhal;

namespace TestApplication
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var type = args[0];
            var volume = args[1];
            var target = args.Length > 2 ? args[2] : "data";
            var location = args.Length > 3 ? args[3] : "./";
            var narwhal = new Narwhal(false);
            if (type == "save")
            {
                var errors = narwhal.Save(volume, target, location);
                var enumerable = errors as string[] ?? errors.ToArray();
                Console.WriteLine(!enumerable.Any() ? "Done!" : enumerable.JoinBy("\n"));
            }
            else
            {
                var errors = narwhal.Load(volume, target);
                var enumerable = errors as string[] ?? errors.ToArray();
                Console.WriteLine(!enumerable.Any() ? "Done!" : enumerable.JoinBy("\n"));
            }
        }
    }
}