namespace user.Infrastructure.Helpers
{
    public class UrlBuilder
    {
        private const string ApplicationUrl = "https://localhost:7140";

        public static string EmailConfirmationLink(int userId, string code)
        {
            return $"{ApplicationUrl}/api/Account/ConfirmEmail?userId={userId}&code={code}";
        }

        public static string ResetPasswordCallbackLink(string code)
        {
            return $"{ApplicationUrl}/api/Account/ResetPassword?code={code}";
        }
    }
}