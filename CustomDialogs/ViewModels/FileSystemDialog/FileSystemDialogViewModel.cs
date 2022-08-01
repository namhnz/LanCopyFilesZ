using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomDialogs.Enums;
using CustomDialogs.Events;
using CustomDialogs.Extensions;
using CustomDialogs.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;

namespace CustomDialogs.ViewModels.FileSystemDialog
{
    public sealed class FileSystemDialogViewModel : BaseDialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly CancellationTokenSource _dialogClosingCts;

        public ObservableCollection<BaseFileSystemDialogItemViewModel> Items { get; }

        public FileSystemDialogMode FileSystemDialogMode { get; }

        private FileNameConflictResolveOptionType _AggregatedResolveOption;

        public FileNameConflictResolveOptionType AggregatedResolveOption
        {
            get => _AggregatedResolveOption;
            set
            {
                if (SetProperty(ref _AggregatedResolveOption, value))
                {
                    ApplyConflictOptionToAll(value);
                }
            }
        }

        private string? _Description;

        public string? Description
        {
            get => _Description;
            set => SetProperty(ref _Description, value);
        }

        private bool _DeletePermanently;

        public bool DeletePermanently
        {
            get => _DeletePermanently;
            set => SetProperty(ref _DeletePermanently, value);
        }

        private bool _IsDeletePermanentlyEnabled;

        public bool IsDeletePermanentlyEnabled
        {
            get => _IsDeletePermanentlyEnabled;
            set => SetProperty(ref _IsDeletePermanentlyEnabled, value);
        }

        private FileSystemDialogViewModel(FileSystemDialogMode fileSystemDialogMode,
            IEnumerable<BaseFileSystemDialogItemViewModel> items)
        {
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            this.FileSystemDialogMode = fileSystemDialogMode;
            this._dialogClosingCts = new();

            _eventAggregator.GetEvent<FileSystemDialogOptionChangedEvent>().Subscribe(Receive);

            foreach (var item in items)
            {
                item.EventAggregator = _eventAggregator;
            }

            this.Items = new(items);

            SecondaryButtonClickCommand = new DelegateCommand(SecondaryButtonClick);
        }

        public bool IsNameAvailableForItem(BaseFileSystemDialogItemViewModel item, string name)
        {
            return Items.Where(x => !x.SourcePath!.Equals(item.SourcePath))
                .Cast<FileSystemDialogConflictItemViewModel>().All(x => x.DestinationDisplayName != name);
        }

        public void ApplyConflictOptionToAll(FileNameConflictResolveOptionType e)
        {
            if (!FileSystemDialogMode.IsInDeleteMode && e != FileNameConflictResolveOptionType.None)
            {
                foreach (var item in Items)
                {
                    if (item is FileSystemDialogConflictItemViewModel conflictItem)
                    {
                        conflictItem.ConflictResolveOption = e;
                    }
                }

                PrimaryButtonEnabled = true;
            }
        }

        public IEnumerable<IFileSystemDialogConflictItemViewModel> GetItemsResult()
        {
            return Items.Cast<IFileSystemDialogConflictItemViewModel>();
        }

        public void Receive(FileSystemDialogConflictItemViewModel fileSystemDialogConflictItemViewModel)
        {
            if (Items.Count == 1)
            {
                AggregatedResolveOption = fileSystemDialogConflictItemViewModel.ConflictResolveOption;
            }
            else
            {
                // If all items have the same resolve option -- set the aggregated option to that choice
                var first = (Items.First() as FileSystemDialogConflictItemViewModel)!.ConflictResolveOption;
                AggregatedResolveOption =
                    Items.All(x => (x as FileSystemDialogConflictItemViewModel)!.ConflictResolveOption == first)
                        ? first
                        : FileNameConflictResolveOptionType.None;
            }
        }

        public void CancelCts()
        {
            _dialogClosingCts.Cancel();
        }

        private void SecondaryButtonClick()
        {
            ApplyConflictOptionToAll(FileNameConflictResolveOptionType.Skip);
        }

        public static FileSystemDialogViewModel GetDialogViewModel(FileSystemDialogMode dialogMode,
            (bool deletePermanently, bool IsDeletePermanentlyEnabled) deleteOption,
            FilesystemOperationType operationType, List<BaseFileSystemDialogItemViewModel> nonConflictingItems,
            List<BaseFileSystemDialogItemViewModel> conflictingItems)
        {
            var titleText = string.Empty;
            var descriptionText = string.Empty;
            var primaryButtonText = string.Empty;
            var secondaryButtonText = string.Empty;
            var isInDeleteMode = false;

            if (dialogMode.ConflictsExist)
            {
                // Subtitle text
                if (conflictingItems.Count > 1)
                {
                    if (nonConflictingItems.Count > 0)
                    {
                        // There are {0} conflicting file names, and {1} outgoing item(s)
                        descriptionText =
                            string.Format(
                                "ConflictingItemsDialogSubtitleMultipleConflictsMultipleNonConflicts".ToLocalized(),
                                conflictingItems.Count, nonConflictingItems.Count);
                    }
                    else
                    {
                        // There are {0} conflicting file names
                        descriptionText =
                            string.Format("ConflictingItemsDialogSubtitleMultipleConflictsNoNonConflicts".ToLocalized(),
                                conflictingItems.Count);
                    }
                }
                else
                {
                    if (nonConflictingItems.Count > 0)
                    {
                        // There is one conflicting file name, and {0} outgoing item(s)
                        descriptionText =
                            string.Format(
                                "ConflictingItemsDialogSubtitleSingleConflictMultipleNonConflicts".ToLocalized(),
                                nonConflictingItems.Count);
                    }
                    else
                    {
                        // There is one conflicting file name
                        descriptionText =
                            string.Format("ConflictingItemsDialogSubtitleSingleConflictNoNonConflicts".ToLocalized(),
                                conflictingItems.Count);
                    }
                }

                titleText = "ConflictingItemsDialogTitle".ToLocalized();
                primaryButtonText = "ConflictingItemsDialogPrimaryButtonText".ToLocalized();
                secondaryButtonText = "Cancel".ToLocalized();
            }
            else
            {
                switch (operationType)
                {
                    case FilesystemOperationType.Copy:
                    {
                        titleText = "CopyItemsDialogTitle".ToLocalized();
                        descriptionText = nonConflictingItems.Count + conflictingItems.Count == 1
                            ? "CopyItemsDialogSubtitleSingle".ToLocalized()
                            : string.Format("CopyItemsDialogSubtitleMultiple".ToLocalized(),
                                nonConflictingItems.Count + conflictingItems.Count);
                        primaryButtonText = "Copy".ToLocalized();
                        secondaryButtonText = "Cancel".ToLocalized();
                        break;
                    }

                    case FilesystemOperationType.Move:
                    {
                        titleText = "MoveItemsDialogTitle".ToLocalized();
                        descriptionText = nonConflictingItems.Count + conflictingItems.Count == 1
                            ? "MoveItemsDialogSubtitleSingle".ToLocalized()
                            : string.Format("MoveItemsDialogSubtitleMultiple".ToLocalized(),
                                nonConflictingItems.Count + conflictingItems.Count);
                        primaryButtonText = "MoveItemsDialogPrimaryButtonText".ToLocalized();
                        secondaryButtonText = "Cancel".ToLocalized();
                        break;
                    }

                    case FilesystemOperationType.Delete:
                    {
                        titleText = "DeleteItemsDialogTitle".ToLocalized();
                        descriptionText = nonConflictingItems.Count + conflictingItems.Count == 1
                            ? "DeleteItemsDialogSubtitleSingle".ToLocalized()
                            : string.Format("DeleteItemsDialogSubtitleMultiple".ToLocalized(),
                                nonConflictingItems.Count);
                        primaryButtonText = "Delete".ToLocalized();
                        secondaryButtonText = "Cancel".ToLocalized();
                        isInDeleteMode = true;
                        break;
                    }
                }
            }

            var viewModel =
                new FileSystemDialogViewModel(
                    new() { IsInDeleteMode = isInDeleteMode, ConflictsExist = !conflictingItems.Any() },
                    conflictingItems.Concat(nonConflictingItems))
                {
                    Title = titleText,
                    Description = descriptionText,
                    PrimaryButtonText = primaryButtonText,
                    SecondaryButtonText = secondaryButtonText,
                    DeletePermanently = deleteOption.deletePermanently,
                    IsDeletePermanentlyEnabled = deleteOption.IsDeletePermanentlyEnabled
                };

            _ = LoadItemsIcon(viewModel.Items, viewModel._dialogClosingCts.Token);

            return viewModel;
        }

        public static FileSystemDialogViewModel GetDialogViewModel(
            List<BaseFileSystemDialogItemViewModel> nonConflictingItems, string titleText, string descriptionText,
            string primaryButtonText, string secondaryButtonText)
        {
            var viewModel = new FileSystemDialogViewModel(new() { IsInDeleteMode = false, ConflictsExist = false },
                nonConflictingItems)
            {
                Title = titleText,
                Description = descriptionText,
                PrimaryButtonText = primaryButtonText,
                SecondaryButtonText = secondaryButtonText,
                DeletePermanently = false,
                IsDeletePermanentlyEnabled = false
            };

            _ = LoadItemsIcon(viewModel.Items, viewModel._dialogClosingCts.Token);

            return viewModel;
        }

        private static async Task LoadItemsIcon(IEnumerable<BaseFileSystemDialogItemViewModel> items,
            CancellationToken token)
        {
            var imagingService = ContainerLocator.Container.Resolve<IImagingService>();
            var threadingService = ContainerLocator.Container.Resolve<IThreadingService>();

            await items.ParallelForEachAsync(async (item) =>
            {
                try
                {
                    if (token.IsCancellationRequested) return;

                    await threadingService.ExecuteOnUiThreadAsync(async () =>
                    {
                        item.ItemIcon = await imagingService.GetImageModelFromPathAsync(item.SourcePath!, 64u);
                    });
                }
                catch (Exception ex)
                {
                    _ = ex;
                }
            }, 10, token);
        }
    }

    public sealed class FileSystemDialogMode
    {
        /// <summary>
        /// Determines whether to show delete options for the dialog.
        /// </summary>
        public bool IsInDeleteMode { get; init; }

        /// <summary>
        /// Determines whether conflicts are visible.
        /// </summary>
        public bool ConflictsExist { get; init; }
    }
}