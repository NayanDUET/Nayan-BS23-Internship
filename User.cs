using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjection
{
    internal class User
    {
        private INotificationService _notificationService;
        public string Username {  get; set; }
        public User(string username, INotificationService notificationService) { 
         
             Username = username;
            _notificationService = notificationService;
        }
        
        public void ChangeUsername(string newUsername)
        {
            Username = newUsername;
            _notificationService.NotifyUsernameChanged(this);
        }
    }
}
