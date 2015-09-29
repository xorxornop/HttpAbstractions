// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Features.Authentication;
using Microsoft.AspNet.Http.Features.Authentication.Internal;

namespace Microsoft.AspNet.Http.Authentication.Internal
{
    public class DefaultAuthenticationManager : AuthenticationManager
    {
        private readonly IFeatureCollection _features;
        private FeatureReference<IHttpAuthenticationFeature> _authentication = FeatureReference<IHttpAuthenticationFeature>.Default;
        private FeatureReference<IHttpResponseFeature> _response = FeatureReference<IHttpResponseFeature>.Default;

        public DefaultAuthenticationManager(IFeatureCollection features)
        {
            _features = features;
        }

        private IHttpAuthenticationFeature HttpAuthenticationFeature
        {
            get { return _authentication.Fetch(_features) ?? _authentication.Update(_features, new HttpAuthenticationFeature()); }
        }

        private IHttpResponseFeature HttpResponseFeature
        {
            get { return _response.Fetch(_features); }
        }

        public override IEnumerable<AuthenticationDescription> GetAuthenticationSchemes()
        {
            var handler = HttpAuthenticationFeature.Handler;
            if (handler == null)
            {
                return new AuthenticationDescription[0];
            }

            var describeContext = new DescribeSchemesContext();
            handler.GetDescriptions(describeContext);
            return describeContext.Results.Select(description => new AuthenticationDescription(description));
        }

        public override async Task AuthenticateAsync(AuthenticateContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var handler = HttpAuthenticationFeature.Handler;

            if (handler != null)
            {
                await handler.AuthenticateAsync(context);
            }

            if (!context.Accepted)
            {
                throw new InvalidOperationException($"No authentication handler is configured to authenticate for the scheme: {context.AuthenticationScheme}");
            }
        }

        public override async Task ChallengeAsync(ChallengeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var handler = HttpAuthenticationFeature.Handler;

            if (handler != null)
            {
                await handler.ChallengeAsync(context);
            }

            if (!context.Accepted)
            {
                throw new InvalidOperationException($"No authentication handler is configured to handle a challenge for the scheme: {context.AuthenticationScheme}");
            }
        }

        public override async Task SignInAsync(SignInContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var handler = HttpAuthenticationFeature.Handler;
            if (handler != null)
            {
                await handler.SignInAsync(context);
            }

            if (!context.Accepted)
            {
                throw new InvalidOperationException($"No authentication handler is configured to handle a sign in for the scheme: {context.AuthenticationScheme}");
            }
        }

        public override async Task SignOutAsync(SignOutContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var handler = HttpAuthenticationFeature.Handler;
            if (handler != null)
            {
                await handler.SignOutAsync(context);
            }

            if (!context.Accepted)
            {
                throw new InvalidOperationException($"No authentication handler is configured to handle sign out for the scheme: {context.AuthenticationScheme}");
            }
        }
    }
}
