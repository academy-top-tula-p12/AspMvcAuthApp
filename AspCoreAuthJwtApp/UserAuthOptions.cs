using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AspCoreAuthJwtApp
{
    public class UserAuthOptions
    {
        public const string Issuer = "AuthServer";
        public const string Audience = "AuthClient";

        private const string key = "abcdefghkjlhkjhfsjkhfjhksdfjhsdghfjsdkjsdhfjshd";

        public static SymmetricSecurityKey SecurityKey
        {
            get => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }
    }
}
