using System;
using System.Collections.Generic;
using System.Text;

namespace BetterCRM.Core.Interfaces.Services
{
    public interface ICurrentUserProvider
    {
        CurrentUserInfo GetCurrent();
    }
}
