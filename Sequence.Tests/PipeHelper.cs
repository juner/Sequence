using System.IO.Pipelines;
using System.Text;

namespace Juner.Sequence;

static class PipeHelper
{
    public static PipeReader CreateReader(params string[] chunks)
    {
        var pipe = new Pipe();

        _ = Task.Run(async () =>
        {
            foreach (var chunk in chunks)
            {
                var bytes = Encoding.UTF8.GetBytes(chunk);
                await pipe.Writer.WriteAsync(bytes);
            }
            await pipe.Writer.CompleteAsync();
        });

        return pipe.Reader;
    }
}