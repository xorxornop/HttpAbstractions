// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.WebUtilities;
using Microsoft.Framework.Internal;
using System.Linq;

namespace Microsoft.AspNet.Http.Features.Internal
{
    public class QueryFeature : IQueryFeature
    {
        private readonly IFeatureCollection _features;
        private FeatureReference<IHttpRequestFeature> _request = FeatureReference<IHttpRequestFeature>.Default;

        private string _original;
        private IReadableStringCollection _created;
        private IReadableStringCollection _assigned;

        public QueryFeature([NotNull] IFeatureCollection features)
        {
            _features = features;
        }

        public IReadableStringCollection Query
        {
            get
            {
                if (_assigned != null)
                {
                    return _assigned;
                }

                var current = _request.Fetch(_features).QueryString;
                if (_created == null || !string.Equals(_original, current, StringComparison.Ordinal))
                {
                    _original = current;
                    _created = new ReadableStringCollection(QueryHelpers.ParseQuery(current).ToDictionary(kv => kv.Key, kv => (StringValues)kv.Value));
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