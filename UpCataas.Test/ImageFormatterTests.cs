using FluentAssertions;
using ImageMagick;
using Machine.Fakes;
using Machine.Specifications;
using System.IO.Pipelines;
using System.Threading;

namespace UpCataas.Test
{
    public class ImageFormatterTests : WithSubject<ImageFormatter>
    {
        public class When_Checking_Write_Type
        {
            It should_return_true_for_magick_images = () =>
                Subject.CanWriteTypes(typeof(MagickImage))
                    .Should().Be(true);
            It should_return_false_for_other_types = () => 
                Subject.CanWriteTypes(typeof(object))
                    .Should().Be(false);
        }

        public class When_Write_Response_Body_Async
        {
            private Establish that = () =>
            {
                _image = An<IMagickImage>();
                _image.WhenToldTo(i => i.ToByteArray()).Return(new byte[10]);
                _writer = An<PipeWriter>();
            };

            Because of = () => ImageFormatter.WriteResponseBodyAsync(_image, _writer);
            
            private It should_write_to_pipe_writer = () =>
                _writer.WasToldTo(w => w.WriteAsync(_image.ToByteArray(), CancellationToken.None));

            private static IMagickImage _image;
            private static PipeWriter _writer;
        }
    }
}
