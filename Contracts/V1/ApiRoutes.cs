namespace Contracts.V1
{
    public static class ApiRoutes
    {
        private const string Root = "api";
        private const string Version = "v1";
        private const string Base = Root + "/" + Version;

       
        public static class Auth
        {
            public const string Login = Base + "/login";

            public const string CreateUser = Base + "/create";
        }
    }
}
