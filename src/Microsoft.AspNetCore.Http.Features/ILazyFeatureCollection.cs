using System;

namespace Microsoft.AspNetCore.Http.Features
{
    public interface ILazyFeatureCollection : IFeatureCollection
    {
        void Set<TFeature>(Func<object, TFeature> factory, object state);
    }
}