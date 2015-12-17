// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.Http.Features
{
    public struct FeatureReferences<TCache>
    {
        public FeatureReferences(IFeatureCollection collection)
        {
            Collection = collection;
            Cache = default(TCache);
            Revision = collection.Revision;
        }

        public readonly IFeatureCollection Collection;
        public int Revision;
        public TCache Cache;

        public TFeature Fetch<TFeature, TState>(
            ref TFeature cached,
            TState state,
            Func<TState, TFeature> factory)
        {
            var cleared = false;
            if (Revision != Collection.Revision)
            {
                cleared = true;
                Cache = default(TCache);
                Revision = Collection.Revision;
            }

            var feature = cached;
            if (feature == null)
            {
                feature = Collection.Get<TFeature>();
                if (feature == null)
                {
                    feature = factory(state);

                    Collection.Set(feature);
                    if (!cleared)
                    {
                        Cache = default(TCache);
                    }
                    Revision = Collection.Revision;
                }
                cached = feature;
            }
            return feature;
        }

        public TFeature Fetch<TFeature>(ref TFeature cached, Func<IFeatureCollection, TFeature> factory) =>
            Fetch(ref cached, Collection, factory);
    }
}