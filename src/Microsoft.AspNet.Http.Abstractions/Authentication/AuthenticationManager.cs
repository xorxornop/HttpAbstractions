// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Features.Authentication;

namespace Microsoft.AspNet.Http.Authentication
{
    public abstract class AuthenticationManager
    {
        public abstract IEnumerable<AuthenticationDescription> GetAuthenticationSchemes();

        public abstract Task AuthenticateAsync(AuthenticateContext context);

        public abstract Task ChallengeAsync(ChallengeContext context);

        public abstract Task SignInAsync(SignInContext context);

        public virtual async Task<ClaimsPrincipal> AuthenticateAsync(string authenticationScheme)
        {
            if (authenticationScheme == null)
            {
                throw new ArgumentNullException(nameof(authenticationScheme));
            }

            var context = new AuthenticateContext(authenticationScheme);
            await AuthenticateAsync(context);
            return context.Principal;
        }

        public virtual Task ChallengeAsync()
        {
            return ChallengeAsync(string.Empty);
        }

        public virtual Task ChallengeAsync(AuthenticationProperties properties)
        {
            return ChallengeAsync(authenticationScheme: string.Empty, properties: properties);
        }

        public virtual Task ChallengeAsync(string authenticationScheme)
        {
            return ChallengeAsync(authenticationScheme, properties: null);
        }

        // Leave it up to authentication handler to do the right thing for the challenge
        public virtual Task ChallengeAsync(string authenticationScheme, AuthenticationProperties properties)
        {
            return ChallengeAsync(authenticationScheme, properties, ChallengeBehavior.Automatic);
        }

        public virtual Task ChallengeAsync(string authenticationScheme, AuthenticationProperties properties, ChallengeBehavior behavior)
        {
            return ChallengeAsync(new ChallengeContext(authenticationScheme, properties?.Items, behavior));
        }


        public virtual Task SignInAsync(string authenticationScheme, ClaimsPrincipal principal)
        {
            return SignInAsync(authenticationScheme, principal, properties: null);
        }

        public virtual Task SignInAsync(string authenticationScheme, ClaimsPrincipal principal, AuthenticationProperties properties)
        {
            if (authenticationScheme == null)
            {
                throw new ArgumentNullException(nameof(authenticationScheme));
            }

            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return SignInAsync(new SignInContext(authenticationScheme, principal, properties?.Items));
        }

        public virtual Task ForbidAsync(string authenticationScheme)
        {
            return ForbidAsync(authenticationScheme, properties: null);
        }

        // Deny access (typically a 403)
        public virtual Task ForbidAsync(string authenticationScheme, AuthenticationProperties properties)
        {
            if (authenticationScheme == null)
            {
                throw new ArgumentNullException(nameof(authenticationScheme));
            }

            return ChallengeAsync(new ChallengeContext(authenticationScheme, properties?.Items, ChallengeBehavior.Forbidden));
        }

        public virtual Task SignOutAsync(string authenticationScheme)
        {
            return SignOutAsync(authenticationScheme, properties: null);
        }

        public virtual Task SignOutAsync(string authenticationScheme, AuthenticationProperties properties)
        {
            if (authenticationScheme == null)
            {
                throw new ArgumentNullException(nameof(authenticationScheme));
            }

            return SignOutAsync(new SignOutContext(authenticationScheme, properties?.Items));
        }

        public abstract Task SignOutAsync(SignOutContext context);
    }
}
