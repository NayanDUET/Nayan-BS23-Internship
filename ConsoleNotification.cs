using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjection
{
    internal class ConsoleNotification : INotificationService
    {
        public void NotifyUsernameChanged(User user)
        {
            Console.WriteLine($"Username Has been Changed: {user.Username}");

        }
    }
}
