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
            var volume = args[0];
            var target = args.Length > 1 ? args[1] : "data";
            var location = args.Length > 2 ? args[2] : "./";
            var narwhal = new Narwhal(false);
            var errors = narwhal.Save(volume, target, location);
            var enumerable = errors as string[] ?? errors.ToArray();
            if (!enumerable.Any())
            {
                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine(enumerable.JoinBy("\n"));
            }
        }
    }
}