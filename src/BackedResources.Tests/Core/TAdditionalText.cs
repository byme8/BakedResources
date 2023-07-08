using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BackedResources.Tests.Core;

public class TAdditionalText : AdditionalText
{
    private readonly string _path;
    private readonly SourceText _text;

    public TAdditionalText(string path, SourceText text)
    {
        _path = path;
        _text = text;
    }

    public override string Path => _path;

    public override SourceText GetText(CancellationToken cancellationToken = new())
    {
        return _text;
    }
}