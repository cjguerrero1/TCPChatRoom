using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public interface ILoggable
    {
        void LogMessages(string message);
        void LogUserActivity(string message);
        void LogExceptions(string message);

      
    }
}
