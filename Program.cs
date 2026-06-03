using System;

namespace DependencyInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            var notification = new ConsoleNotification();
            var user1 = new User("nayan", notification);
            user1.ChangeUsername("Barman");

            Console.ReadKey();
        }
    }
}