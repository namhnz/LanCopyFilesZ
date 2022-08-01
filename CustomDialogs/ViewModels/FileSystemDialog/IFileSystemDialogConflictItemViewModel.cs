namespace CustomDialogs.ViewModels.FileSystemDialog
{
    public interface IFileSystemDialogConflictItemViewModel
    {
        string? SourcePath { get; }

        string? DestinationPath { get; }

        string? CustomName { get; }

        FileNameConflictResolveOptionType ConflictResolveOption { get; }
    }
}
