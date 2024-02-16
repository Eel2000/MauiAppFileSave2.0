using System.Buffers;

namespace CommunityToolkit.Maui.Storage;

public sealed partial class FileSaverImplementation
{
	/// <inheritdoc/>
	public async Task<FileSaverResult> SaveAsync(string initialPath, string fileName, Stream stream, CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			var path = await InternalSaveAsync(initialPath, fileName, stream, cancellationToken);
			return new FileSaverResult(path, null);
		}
		catch (Exception e)
		{
			return new FileSaverResult(null, e);
		}
	}

	/// <inheritdoc/>
	public async Task<FileSaverResult> SaveAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			var path = await InternalSaveAsync(fileName, stream, cancellationToken);
			return new FileSaverResult(path, null);
		}
		catch (Exception e)
		{
			return new FileSaverResult(null, e);
		}
	}

	/// <inheritdoc/>
	public async Task<FileSaverResult> SaveAsync(string fileName, Stream stream, IProgress<double> progress, CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			var path = await InternalSaveAsync(fileName, stream, progress, cancellationToken);
			return new FileSaverResult(path, null);
		}
		catch (Exception e)
		{
			return new FileSaverResult(null, e);
		}
	}

	static async Task WriteStream(Stream stream, string filePath, CancellationToken cancellationToken)
	{
		await using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
		fileStream.SetLength(0);
		if (stream.CanSeek)
		{
			stream.Seek(0, SeekOrigin.Begin);
		}

		await stream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
	}


	static async Task WriteStream(Stream stream, string filePath, IProgress<double> progress, CancellationToken cancellationToken)
	{
		await using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
		fileStream.SetLength(0);
		if (stream.CanSeek)
		{
			stream.Seek(0, SeekOrigin.Begin);
		}

		using var buffer = MemoryPool<byte>.Shared.Rent(81920);
		var streamLength = stream.Length;

		try
		{
			int bytesRead;
			long totalRead = 0;
			while ((bytesRead = await stream.ReadAsync(buffer.Memory, cancellationToken).ConfigureAwait(false)) > 0)
			{
				await fileStream.WriteAsync(buffer.Memory[..bytesRead], cancellationToken).ConfigureAwait(false);
				totalRead += bytesRead;

				progress.Report(1.0 * totalRead / streamLength);
			}
		}
		catch { }

	}
}