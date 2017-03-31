// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http.Features.Authentication
{
    [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
    public interface IAuthenticationHandler
    {
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        void GetDescriptions(DescribeSchemesContext context);

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        Task AuthenticateAsync(AuthenticateContext context);

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        Task ChallengeAsync(ChallengeContext context);

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        Task SignInAsync(SignInContext context);

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        Task SignOutAsync(SignOutContext context);
    }
}
