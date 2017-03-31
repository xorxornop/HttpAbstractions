// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Http.Features.Authentication
{
    [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
    public enum ChallengeBehavior
    {
        Automatic,
        Unauthorized,
        Forbidden
    }
}