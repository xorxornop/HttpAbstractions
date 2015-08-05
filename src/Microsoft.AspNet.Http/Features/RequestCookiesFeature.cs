// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http.Internal;
using Microsoft.Framework.Internal;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNet.Http.Features.Internal
{
    public class RequestCookiesFeature : IRequestCookiesFeature
    {
        private readonly IFeatureCollection _features;
        private readonly FeatureReference<IHttpRequestFeature> _request = FeatureReference<IHttpRequestFeature>.Default;

        private StringValues _original;
        private RequestCookiesCollection _created;
        private IReadableStringCollection _assigned;

        public RequestCookiesFeature([NotNull] IFeatureCollection features)
        {
            _features = features;
        }

        public IReadableStringCollection Cookies
        {
            get
            {
                if (_assigned != null)
                {
                    return _assigned;
                }

                var headers = _request.Fetch(_features).Headers;
                StringValues current;
                if (!headers.TryGetValue(HeaderNames.Cookie, out current))
                {
                    current = StringValues.Empty;
                }

                if (_created == null || !Enumerable.SequenceEqual(_original, current, StringComparer.Ordinal))
                {
                    _original = current;
                    if (_created == null)
                    {
                        _created = new RequestCookiesCollection();
                    }
                    _created.Reparse(current);
                }

                return _created;
            }
            set
            {
                if (ReferenceEquals(_created, value))
                {
                    _assigned = null;
                }
                else
                {
                    _assigned = value;
                }
            }
        }
    }
}