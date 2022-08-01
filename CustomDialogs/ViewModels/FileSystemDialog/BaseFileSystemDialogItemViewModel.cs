using System.IO;
using CustomDialogs.Models.Imaging;
using Prism.Events;
using Prism.Mvvm;

namespace CustomDialogs.ViewModels.FileSystemDialog
{
    public abstract class BaseFileSystemDialogItemViewModel : BindableBase
    {
        public IEventAggregator EventAggregator { get; set; }

        private string? _SourcePath;
        public virtual string? SourcePath
        {
            get => _SourcePath;
            set
            {
                if (SetProperty(ref _SourcePath, value))
                {
                    RaisePropertyChanged(nameof(SourceDirectoryDisplayName));
                    DisplayName = Path.GetFileName(value);
                }
            }
        }

        private string? _DisplayName;
        public virtual string? DisplayName
        {
            get => _DisplayName;
            set => SetProperty(ref _DisplayName, value);
        }

        private ImageModel? _ItemIcon;
        public ImageModel? ItemIcon
        {
            get => _ItemIcon;
            set => SetProperty(ref _ItemIcon, value);
        }

        public virtual string? SourceDirectoryDisplayName
        {
            get => Path.GetFileName(Path.GetDirectoryName(SourcePath));
        }
    }
}