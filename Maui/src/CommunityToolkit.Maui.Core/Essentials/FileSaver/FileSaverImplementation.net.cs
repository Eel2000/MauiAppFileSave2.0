namespace CommunityToolkit.Maui.Storage;

/// <inheritdoc />
public sealed partial class FileSaverImplementation : IFileSaver
{
	Task<string> InternalSaveAsync(string initialPath, string fileName, Stream stream, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	Task<string> InternalSaveAsync(string initialPath, string fileName, Stream stream, IProgress<double> progress, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	Task<string> InternalSaveAsync(string fileName, Stream stream, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	Task<string> InternalSaveAsync(string fileName, Stream stream, IProgress<double> progress, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	Task<string> InternalBulkSaveAsync(string initialPath, IReadOnlyDictionary<string, Stream> files, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}