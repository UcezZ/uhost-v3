using System.Collections.Generic;

namespace Uhost.Console.Models
{
    internal class DefaultDataModel
    {
        public DefaultUserModel SuperAdmin { get; set; }
        public DefaultRoleModel SuperRole { get; set; }
        public IEnumerable<DefaultRightModel> Rights { get; set; }
        public IEnumerable<DefaultUserModel> Users { get; set; }
        public IEnumerable<DefaultRoleModel> Roles { get; set; }
    }
}
