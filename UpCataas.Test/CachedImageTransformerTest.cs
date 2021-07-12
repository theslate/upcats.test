using System;
using FluentAssertions;
using ImageMagick;
using Machine.Fakes;
using Machine.Specifications;
using Microsoft.Extensions.Caching.Memory;

namespace UpCataas.Test.CachedImageTransformerTests
{
    public class When_Flipping_An_Image : WithSubject<CachedImageTransformer>
    {
        private static IMagickImage _image;
        private static IMagickImage _actual;

        Establish content = () =>
        {
            _image = An<IMagickImage>();
            _image.WhenToldTo(i => i.Signature).Return("1");
        };

        Because of = () => _actual = Subject.FlipUpsideDown(_image);
        
        private It should_return_image = () => _actual.Should().Be(_image);

        private It should_call_image_flip = () => _image.WasToldTo(i => i.Flip());

        private It should_cache_image = () => The<IMemoryCache>().WasToldTo(c => c.CreateEntry(_image.Signature));

        public class And_Image_Is_Cached
        {
            private static object _cachedImage;

            private Establish content = () =>
            {
                _cachedImage = An<IMagickImage>();
                The<IMemoryCache>().WhenToldTo(c => c.TryGetValue(_image.Signature, out _cachedImage)).Return(true);
            };
            
            private It should_return_cached = () => _actual.Should().Be(_cachedImage);

            private It should_not_return_source = () => _actual.Should().NotBe(_image);
        }
    }
}
