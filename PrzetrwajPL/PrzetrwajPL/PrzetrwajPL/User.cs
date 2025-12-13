using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL
{
    public class User
    {
        public string id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string role { get; set; }
        public bool twoFactorEnabled { get; set; }
        public bool banned { get; set; }
        public string banReason { get; set; }

    }
}
