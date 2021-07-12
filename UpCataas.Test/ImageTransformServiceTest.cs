using FluentAssertions;
using ImageMagick;
using Machine.Fakes;
using Machine.Specifications;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UpCataas.Test.ImageTransformServiceTest
{
    public class When_Flipping_An_Image : WithSubject<ImageTransformService>
    {
        private static Uri _uri;
        private static IMagickImage _image;
        private static IMagickImage _actual;

        private Establish content = () =>
        {
            Configure<HttpMessageHandler, FakeHttpHandler>();
            _image = An<IMagickImage>();
            The<IImageTransformer>()
                .WhenToldTo(t => t.FlipUpsideDown(Param.IsAny<IMagickImage>()))
                .Return(_image);
            _uri = new Uri("http://fake.fake");
        };

        Because of = () => _actual = Subject.Flip(_uri).Result;
        
        private It should_return_image = () => _actual.Should().Be(_image);
    }

    public class FakeHttpHandler : HttpMessageHandler
    {
        private readonly byte[] _imageBytes;

        public FakeHttpHandler()
        {
            var image = new MagickImage(new MagickColor("#ffffff"), 16, 16) {Format = MagickFormat.Png};
            _imageBytes = image.ToByteArray();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage {Content = new ByteArrayContent(_imageBytes)});
        }
    }
}
