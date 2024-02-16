using System.Diagnostics;
using Windows.Storage.Pickers;

namespace CommunityToolkit.Maui.Storage;

/// <inheritdoc />
public sealed partial class FileSaverImplementation : IFileSaver
{
	readonly List<string> allFilesExtension = new() { "." };

	async Task<string> InternalSaveAsync(string initialPath, string fileName, Stream stream, CancellationToken cancellationToken)
	{
		var savePicker = new FileSavePicker
		{
			SuggestedStartLocation = PickerLocationId.DocumentsLibrary
		};
		WinRT.Interop.InitializeWithWindow.Initialize(savePicker, Process.GetCurrentProcess().MainWindowHandle);

		var extension = Path.GetExtension(fileName);
		if (!string.IsNullOrEmpty(extension))
		{
			savePicker.FileTypeChoices.Add(extension, new List<string> { extension });
		}

		savePicker.FileTypeChoices.Add("All files", allFilesExtension);
		savePicker.SuggestedFileName = Path.GetFileNameWithoutExtension(fileName);

		var filePickerOperation = savePicker.PickSaveFileAsync();

		await using var _ = cancellationToken.Register(CancelFilePickerOperation);
		var file = await filePickerOperation;
		if (string.IsNullOrEmpty(file?.Path))
		{
			throw new FileSaveException("Operation cancelled or Path doesn't exist.");
		}

		await WriteStream(stream, file.Path, cancellationToken).ConfigureAwait(false);
		return file.Path;

		void CancelFilePickerOperation()
		{
			filePickerOperation.Cancel();
		}
	}

	async Task<string> InternalSaveAsync(string initialPath, string fileName, Stream stream, IProgress<double> progress, CancellationToken cancellationToken)
	{
		var savePicker = new FileSavePicker
		{
			SuggestedStartLocation = PickerLocationId.DocumentsLibrary
		};
		WinRT.Interop.InitializeWithWindow.Initialize(savePicker, Process.GetCurrentProcess().MainWindowHandle);

		var extension = Path.GetExtension(fileName);
		if (!string.IsNullOrEmpty(extension))
		{
			savePicker.FileTypeChoices.Add(extension, new List<string> { extension });
		}

		savePicker.FileTypeChoices.Add("All files", allFilesExtension);
		savePicker.SuggestedFileName = Path.GetFileNameWithoutExtension(fileName);

		var filePickerOperation = savePicker.PickSaveFileAsync();

		await using var _ = cancellationToken.Register(CancelFilePickerOperation);
		var file = await filePickerOperation;
		if (string.IsNullOrEmpty(file?.Path))
		{
			throw new FileSaveException("Operation cancelled or Path doesn't exist.");
		}

		await WriteStream(stream, file.Path, progress, cancellationToken).ConfigureAwait(false);
		return file.Path;

		void CancelFilePickerOperation()
		{
			filePickerOperation.Cancel();
		}
	}

	Task<string> InternalSaveAsync(string fileName, Stream stream, CancellationToken cancellationToken)
	{
		return InternalSaveAsync(string.Empty, fileName, stream, cancellationToken);
	}

	Task<string> InternalBulkSaveAsync(string initialPath, IReadOnlyDictionary<string, Stream> files, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	Task<string> InternalSaveAsync(string fileName, Stream stream, IProgress<double> progress, CancellationToken cancellationToken)
	{
		return InternalSaveAsync(string.Empty, fileName, stream, progress, cancellationToken);
	}

}