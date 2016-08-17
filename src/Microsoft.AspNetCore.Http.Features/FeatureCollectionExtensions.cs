using System;

namespace Microsoft.AspNetCore.Http.Features
{
    public static class FeatureCollectionExtensions
    {
        public static void TryAddLazy<T>(this IFeatureCollection featureCollection, Func<object, T> factory, object state)
        {
            var lazyFeatureCollection = featureCollection as ILazyFeatureCollection;
            if (lazyFeatureCollection != null)
            {
                lazyFeatureCollection.Set(factory, state);
            }
            else
            {
                featureCollection.Set(factory(state));
            }
        }
    }
}