using System;
using System.Collections.Generic;
using System.Text;

namespace BetterCRM.Core.Constants
{
    public static class Roles
    {
        public const string OrganizationHead = "OrganizationHead";
        public const string DepartmentHead = "DepartmentHead";
        public const string Employee = "Employee";
        public const string Managers = $"{OrganizationHead},{DepartmentHead}";
    }
}
