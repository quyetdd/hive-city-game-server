namespace HiveCity.Server.Proxy.Common
{
    public enum ErrorCode : short
    {
        OperationDenied = -3,
        OperationInvalid = -2,
        InternalServerError = -1,

        Ok = 0,
        UserNameInUse,
        InCorrectUserNameOrPassword,
        UserCurrentlyLoggedIn,
        InvalidCharacter
    }
}
