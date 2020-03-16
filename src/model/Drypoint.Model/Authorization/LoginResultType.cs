using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Model.Authorization
{
    public enum LoginResultType : byte
    {
        Success = 1,

        InvalidUserNameOrEmailAddress,

        InvalidPassword,

        UserIsNotActive,

        UserEmailIsNotConfirmed,

        UnknownExternalLogin,

        LockedOut,

        UserPhoneNumberIsNotConfirmed,
    }
}
