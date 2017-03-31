// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace Microsoft.AspNetCore.Http.Authentication
{
    [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
    public abstract class AuthenticationManager
    {
        /// <summary>
        /// Constant used to represent the automatic scheme
        /// </summary>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public const string AutomaticScheme = "Automatic";

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public abstract HttpContext HttpContext { get; }

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public abstract IEnumerable<AuthenticationDescription> GetAuthenticationSchemes();

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public abstract Task<AuthenticateInfo> GetAuthenticateInfoAsync(string authenticationScheme);

        // Will remove once callees have been updated
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public abstract Task AuthenticateAsync(AuthenticateContext context);

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public virtual async Task<ClaimsPrincipal> AuthenticateAsync(string authenticationScheme)
        {
            return (await GetAuthenticateInfoAsync(authenticationScheme))?.Principal;
        }

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public virtual Task ChallengeAsync()
        {
            return ChallengeAsync(properties: null);
        }

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public virtual Task ChallengeAsync(AuthenticationProperties properties)
        {
            return ChallengeAsync(authenticationScheme: AutomaticScheme, properties: properties);
        }

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public virtual Task ChallengeAsync(string authenticationScheme)
        {
            if (string.IsNullOrEmpty(authenticationScheme))
            {
                throw new ArgumentException(nameof(authenticationScheme));
            }

            return ChallengeAsync(authenticationScheme: authenticationScheme, properties: null);
        }

        // Leave it up to authentication handler to do the right thing for the challenge
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public virtual Task ChallengeAsync(string authenticationScheme, AuthenticationProperties properties)
        {
            if (string.IsNullOrEmpty(authenticationScheme))
            {
                throw new ArgumentException(nameof(authenticationScheme));
            }

            return ChallengeAsync(authenticationScheme, properties, ChallengeBehavior.Automatic);
        }

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public virtual Task SignInAsync(string authenticationScheme, ClaimsPrincipal principal)
        {
            if (string.IsNullOrEmpty(authenticationScheme))
            {
                throw new ArgumentException(nameof(authenticationScheme));
            }

            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return SignInAsync(authenticationScheme, principal, properties: null);
        }

        /// <summary>
        /// Creates a challenge for the authentication manager with <see cref="ChallengeBehavior.Forbidden"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous challenge operation.</returns>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public virtual Task ForbidAsync()
            => ForbidAsync(AutomaticScheme, properties: null);

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public virtual Task ForbidAsync(string authenticationScheme)
        {
            if (authenticationScheme == null)
            {
                throw new ArgumentNullException(nameof(authenticationScheme));
            }

            return ForbidAsync(authenticationScheme, properties: null);
        }

        // Deny access (typically a 403)
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public virtual Task ForbidAsync(string authenticationScheme, AuthenticationProperties properties)
        {
            if (authenticationScheme == null)
            {
                throw new ArgumentNullException(nameof(authenticationScheme));
            }

            return ChallengeAsync(authenticationScheme, properties, ChallengeBehavior.Forbidden);
        }

        /// <summary>
        /// Creates a challenge for the authentication manager with <see cref="ChallengeBehavior.Forbidden"/>.
        /// </summary>
        /// <param name="properties">Additional arbitrary values which may be used by particular authentication types.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous challenge operation.</returns>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public virtual Task ForbidAsync(AuthenticationProperties properties)
            => ForbidAsync(AutomaticScheme, properties);

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public abstract Task ChallengeAsync(string authenticationScheme, AuthenticationProperties properties, ChallengeBehavior behavior);

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public abstract Task SignInAsync(string authenticationScheme, ClaimsPrincipal principal, AuthenticationProperties properties);

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public virtual Task SignOutAsync(string authenticationScheme)
        {
            if (authenticationScheme == null)
            {
                throw new ArgumentNullException(nameof(authenticationScheme));
            }

            return SignOutAsync(authenticationScheme, properties: null);
        }

        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470")]
        public abstract Task SignOutAsync(string authenticationScheme, AuthenticationProperties properties);
    }
}
