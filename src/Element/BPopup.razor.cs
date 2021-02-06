﻿
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class BPopup : ComponentBase
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private DialogService DialogService { get; set; }

        [Inject]
        private MessageService MessageService { get; set; }

        [Inject]
        private LoadingService LoadingService { get; set; }

        [Inject]
        private PopupService PopupService { get; set; }

        internal int ShadowCount { get; set; }
        [Inject]
        private Document Document { get; set; }
        private static int ZIndex { get; set; } = 2000;
        internal List<MessageInfo> Messages { get; set; } = new List<MessageInfo>();
        private List<MessageInfo> RemovingMessages = new List<MessageInfo>();

        protected List<LoadingOption> LoadingOptions = new List<LoadingOption>();
        internal List<DialogOption> DialogOptions = new List<DialogOption>();
        internal List<DateTimePickerOption> DateTimePickerOptions = new List<DateTimePickerOption>();
        internal protected List<DropDownOption> SelectDropDownOptions = new List<DropDownOption>();
        internal protected List<DropDownOption> DropDownMenuOptions = new List<DropDownOption>();
        internal protected List<SubMenuOption> SubMenuOptions = new List<SubMenuOption>();
        internal List<PopupLayerOption> PopupOptions = new List<PopupLayerOption>();

        internal async Task CloseDialogAsync(DialogOption option, DialogResult result)
        {
            option.Result = result;
            if (option.Element == null)
            {
                await DialogAnimationEndAsync(option);
            }
            else
            {
                option.ShadowElement.Resume();
                option.Element.Resume();
            }
            await option.TaskCompletionSource.Task;
        }

        private async Task OnPauseAsync(DialogOption option)
        {
            if (option.OnShow == null)
            {
                return;
            }
            await option.OnShow();
        }

        async Task DialogAnimationEndAsync(DialogOption option)
        {
            option.TaskCompletionSource.TrySetResult(option.Result);
            await Task.Delay(100);
            DialogService.Dialogs.Remove(option);
        }

        protected void SelectDay(DateTimePickerOption option, DateTime day)
        {
            option.CurrentMonth = day;
            option.TaskComplietionSource.SetResult(day);
            CloseDateTimePicker(option);
        }

        protected override void OnInitialized()
        {
            MessageService.Messages.CollectionChanged -= Messages_CollectionChanged;
            MessageService.Messages = new ObservableCollection<MessageInfo>();
            MessageService.Messages.CollectionChanged += Messages_CollectionChanged;

            LoadingService.LoadingOptions.CollectionChanged -= LoadingOptions_CollectionChanged;
            LoadingService.LoadingOptions = new ObservableCollection<LoadingOption>();
            LoadingService.LoadingOptions.CollectionChanged += LoadingOptions_CollectionChanged;

            DialogService.Dialogs.CollectionChanged -= Dialogs_CollectionChanged;
            DialogService.Dialogs = new ObservableCollection<DialogOption>();
            DialogService.Dialogs.CollectionChanged += Dialogs_CollectionChanged;

            PopupService.DateTimePickerOptions.CollectionChanged -= DateTimePickerOptions_CollectionChanged;
            PopupService.DateTimePickerOptions = new ObservableCollection<DateTimePickerOption>();
            PopupService.DateTimePickerOptions.CollectionChanged += DateTimePickerOptions_CollectionChanged;

            PopupService.SelectDropDownOptions.CollectionChanged -= DropDownOptions_CollectionChanged;
            PopupService.SelectDropDownOptions = new ObservableCollection<DropDownOption>();
            PopupService.SelectDropDownOptions.CollectionChanged += DropDownOptions_CollectionChanged;

            PopupService.SubMenuOptions.CollectionChanged -= SubMenuOptions_CollectionChanged;
            PopupService.SubMenuOptions = new ObservableCollection<SubMenuOption>();
            PopupService.SubMenuOptions.CollectionChanged += SubMenuOptions_CollectionChanged;

            PopupService.DropDownMenuOptions.CollectionChanged -= DropDownMenuOptions_CollectionChanged;
            PopupService.DropDownMenuOptions = new ObservableCollection<DropDownOption>();
            PopupService.DropDownMenuOptions.CollectionChanged += DropDownMenuOptions_CollectionChanged;

            PopupService.PopupLayerOptions.CollectionChanged -= PopupLayerOptions_CollectionChanged;
            PopupService.PopupLayerOptions = new ObservableCollection<PopupLayerOption>();
            PopupService.PopupLayerOptions.CollectionChanged += PopupLayerOptions_CollectionChanged;

            PopupService.DropDownTreeOptions.CollectionChanged -= DropDownTreeOptions_CollectionChanged;
            PopupService.DropDownTreeOptions = new ObservableCollection<PopupLayerOption>();
            PopupService.DropDownTreeOptions.CollectionChanged += DropDownTreeOptions_CollectionChanged;
        }

        private void DropDownTreeOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var option = e.NewItems.OfType<DropDownOption>().FirstOrDefault();
                option.IsNew = true;
                option.Instance = this;
                option.ShadowZIndex = ZIndex++;
                option.ZIndex = ZIndex++;
                SelectDropDownOptions.Add(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var option = e.OldItems.OfType<DropDownOption>().FirstOrDefault();
                SelectDropDownOptions.Remove(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        private void PopupLayerOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var option = e.NewItems.OfType<PopupLayerOption>().FirstOrDefault();
                option.IsNew = true;
                option.Instance = this;
                option.ShadowZIndex = ZIndex++;
                option.ZIndex = ZIndex++;
                PopupOptions.Add(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var option = e.OldItems.OfType<PopupLayerOption>().FirstOrDefault();
                PopupOptions.Remove(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        private void DropDownMenuOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var option = e.NewItems.OfType<DropDownOption>().FirstOrDefault();
                option.IsNew = true;
                option.Instance = this;
                option.ShadowZIndex = ZIndex++;
                option.ZIndex = ZIndex++;
                DropDownMenuOptions.Add(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var option = e.OldItems.OfType<DropDownOption>().FirstOrDefault();
                DropDownMenuOptions.Remove(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        private void SubMenuOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var option = e.NewItems.OfType<SubMenuOption>().FirstOrDefault();
                option.IsNew = true;
                option.Instance = this;
                option.ShadowZIndex = ZIndex++;
                option.ZIndex = ZIndex++;
                option.Close = CloseSubMenuAsync;
                SubMenuOptions.Add(option);
                InvokeAsync(StateHasChanged);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var option = e.OldItems.OfType<SubMenuOption>().FirstOrDefault();
                if (!SubMenuOptions.Remove(option))
                {
                    Console.WriteLine("SubMenuOptions_CollectionChanged Remove Failure");
                }
                InvokeAsync(StateHasChanged);
            }
        }

        private void DropDownOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var option = e.NewItems.OfType<DropDownOption>().FirstOrDefault();
                option.IsNew = true;
                option.Instance = this;
                option.ShadowZIndex = ZIndex++;
                option.ZIndex = ZIndex++;
                SelectDropDownOptions.Add(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var option = e.OldItems.OfType<DropDownOption>().FirstOrDefault();
                SelectDropDownOptions.Remove(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        private void DateTimePickerOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var option = e.NewItems.OfType<DateTimePickerOption>().FirstOrDefault();
                option.IsNew = true;
                option.Instance = this;
                option.ShadowZIndex = ZIndex++;
                option.ZIndex = ZIndex++;
                DateTimePickerOptions.Add(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var option = e.OldItems.OfType<DateTimePickerOption>().FirstOrDefault();
                DateTimePickerOptions.Remove(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        private void Dialogs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var option = e.NewItems.OfType<DialogOption>().FirstOrDefault();
                option.IsNew = true;
                option.Instance = this;
                option.ShadowZIndex = ZIndex++;
                option.ZIndex = ZIndex++;
                DialogOptions.Add(option);
                StateHasChanged();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var option = e.OldItems.OfType<DialogOption>().FirstOrDefault();
                DialogOptions.Remove(option);
                StateHasChanged();
            }
        }

        protected void GoPrevYear(DateTimePickerOption option)
        {
            if (option.ShowYear > 0 && option.ShowYear > 1900)
            {
                option.ShowYear -= 10;
                return;
            }
            if (option.CurrentMonth.Year < 1900)
            {
                return;
            }
            option.CurrentMonth = option.CurrentMonth.AddYears(-1);
        }


        protected void GoNextYear(DateTimePickerOption option)
        {
            if (option.ShowYear > 0)
            {
                option.ShowYear += 10;
                return;
            }
            option.CurrentMonth = option.CurrentMonth.AddYears(1);
        }
        protected void OpenMonth(DateTimePickerOption option, int month)
        {
            option.ShowYear = option.ShowMonth = 0;
            option.CurrentMonth = new DateTime(option.CurrentMonth.Year, month, 1);
        }

        private void LoadingOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var loadingOption = e.NewItems.OfType<LoadingOption>().FirstOrDefault();
                loadingOption.IsNew = true;
                loadingOption.ZIndex = ZIndex++;
                LoadingOptions.Add(loadingOption);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var loadingOption = e.OldItems.OfType<LoadingOption>().FirstOrDefault();
                LoadingOptions.Remove(loadingOption);
                if (string.IsNullOrWhiteSpace(loadingOption.Target.Id))
                {
                    _ = CloseFullScreenLoadingAsync(loadingOption);
                }
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var message = e.NewItems.OfType<MessageInfo>().FirstOrDefault();
                message.IsNew = true;
                message.ZIndex = ZIndex++;
                lock (Messages)
                {
                    message.Index = Messages.Count;
                    var movingDistance = 30;
                    if (message.Index > 0)
                    {
                        var prevMessage = Messages.LastOrDefault();
                        message.BeginTop = prevMessage.EndTop + 30;
                        message.EndTop = message.BeginTop + movingDistance;
                    }
                    else
                    {
                        message.BeginTop = 0;
                        message.EndTop = movingDistance;
                    }
                    Messages.Add(message);
                }
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems.OfType<MessageInfo>())
                {
                    RemovingMessages.Add(item);
                }
                if (Messages.Count == RemovingMessages.Count && RemovingMessages.All(x => Messages.Any(y => x == y)))
                {
                    lock (Messages)
                    {
                        foreach (var item in RemovingMessages)
                        {
                            Messages.Remove(item);
                        }
                    }
                    RemovingMessages.Clear();
                    InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await RenderLoadingAsync();
            await RenderDateTimePickerAsync();
            await RenderDropDownAsync(SelectDropDownOptions, true);
            await RenderDropDownAsync(DropDownMenuOptions, false);
            await RenderSubMenuAsync();
        }

        private async Task RenderSubMenuAsync()
        {
            var option = SubMenuOptions.FirstOrDefault(x => x.IsNew);
            if (option == null)
            {
                return;
            }
            option.IsNew = false;
            var targetEl = option.Target.Dom(JSRuntime);
            var rect = await targetEl.GetBoundingClientRectAsync();
            var top = await targetEl.GetTopRelativeBodyAsync();
            option.Left = rect.Left;
            option.Top = top + rect.Height;
            option.IsShow = true;
            option.ShowStatus = AnimationStatus.Begin;
            StateHasChanged();
            await Task.Delay(10);
            option.ShowStatus = AnimationStatus.End;
            StateHasChanged();
        }
        private async Task RenderDropDownAsync(List<DropDownOption> dropDownOptions, bool autoWidth)
        {
            var option = dropDownOptions.FirstOrDefault(x => x.IsNew);
            if (option == null)
            {
                return;
            }
            option.IsNew = false;
            var targetEl = option.Target.Dom(JSRuntime);
            var rect = await targetEl.GetBoundingClientRectAsync();
            var top = await targetEl.GetTopRelativeBodyAsync();
            option.Left = rect.Left;
            option.Top = top + rect.Height;
            if (autoWidth)
            {
                option.Width = rect.Width;
            }
            var dropDownEl = option.Element.Dom(JSRuntime);
            var width = await dropDownEl.GetClientWidthAsync();
            var documentWidth = await Document.GetClientWidthAsync();
            if (option.Left + width >= documentWidth)
            {
                option.Left = documentWidth - width - 10;
            }
            option.IsShow = true;
            if (GlobalElementSettings.DisableAnimation)
            {
                StateHasChanged();
                return;
            }
            option.ShowStatus = AnimationStatus.Begin;
            StateHasChanged();
            await Task.Delay(10);
            option.ShowStatus = AnimationStatus.End;
            StateHasChanged();
        }

        internal async Task CloseDropDownAsync(DropDownOption option)
        {
            option.IsShow = false;
            if (GlobalElementSettings.DisableAnimation)
            {
                PopupService.SelectDropDownOptions.Remove(option);
                PopupService.DropDownMenuOptions.Remove(option);
                option.Refresh?.Invoke();
                return;
            }
            option.HideStatus = AnimationStatus.Begin;
            StateHasChanged();
            await Task.Delay(10);
            option.HideStatus = AnimationStatus.End;
            StateHasChanged();
            await Task.Delay(200);
            PopupService.SelectDropDownOptions.Remove(option);
            PopupService.DropDownMenuOptions.Remove(option);
            option.Refresh?.Invoke();
        }

        internal async Task CloseSubMenuAsync(SubMenuOption option)
        {
            await option.SubMenu.TopMenu.SemaphoreSlim.WaitAsync();
            try
            {
                if (!option.IsShow)
                {
                    return;
                }
                option.IsShow = false;
                if (GlobalElementSettings.DisableAnimation)
                {
                    await InvokeAsync(option.SubMenu.DeActivate);
                    PopupService.SubMenuOptions.Remove(option);
                    return;
                }
                await InvokeAsync(StateHasChanged);
                await Task.Delay(200);
                await InvokeAsync(option.SubMenu.DeActivate);
                PopupService.SubMenuOptions.Remove(option);
            }
            finally
            {
                option.SubMenu.TopMenu.SemaphoreSlim.Release();
            }
        }

        internal void KeepShowSubMenu(SubMenuOption option)
        {
            CancelTokenSource(option.ClosingTaskCancellationTokenSource);
        }
        void CancelTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource == null)
            {
                return;
            }
            try
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }
                cancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException)
            {

            }
        }
        void DisposeTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource == null)
            {
                return;
            }
            try
            {
                cancellationTokenSource.Dispose();
            }
            catch (ObjectDisposedException)
            {

            }
        }

        internal async Task ReadyCloseSubMenuAsync(SubMenuOption option)
        {
            option.ClosingTaskCancellationTokenSource = new System.Threading.CancellationTokenSource();
            var task = Task.Delay(200, option.ClosingTaskCancellationTokenSource.Token).ContinueWith(async task =>
              {
                  if (task.IsCanceled)
                  {
                      DisposeTokenSource(option.ClosingTaskCancellationTokenSource);
                      return;
                  }
                  DisposeTokenSource(option.ClosingTaskCancellationTokenSource);
                  await CloseSubMenuAsync(option);
              });
            option.ClosingTask = await task;
        }

        async Task RenderDateTimePickerAsync()
        {
            var option = DateTimePickerOptions.FirstOrDefault(x => x.IsNew);
            if (option == null)
            {
                return;
            }
            option.IsNew = false;
            var targetEl = option.Target.Dom(JSRuntime);
            var rect = await targetEl.GetBoundingClientRectAsync();
            var top = await targetEl.GetTopRelativeBodyAsync();
            option.Left = rect.Left;
            option.Top = top + rect.Height;
            var style = option.Element.Dom(JSRuntime).Style;
            await style.SetAsync("left", $"{rect.Left}px");
            if (!GlobalElementSettings.DisableAnimation)
            {
                await style.SetAsync("top", $"{option.Top + 10}px");
                await style.ClearAsync("display");
                await Task.Delay(10);
            }
            else
            {
                await style.ClearAsync("display");
            }
            await style.SetAsync("top", $"{option.Top}px");
            await style.SetAsync("opacity", $"1");
        }

        private void DisposeMessage(MessageInfo item)
        {
            Console.WriteLine("disp:" + item.Message);
            item.IsNew = false;
            lock (MessageService.Messages)
            {
                MessageService.Messages.Remove(item);
            }
        }

        protected void CloseDateTimePicker(DateTimePickerOption option)
        {
            PopupService.DateTimePickerOptions.Remove(option);
        }

        async Task ShowFullScreenLoadingAsync(LoadingOption option)
        {
            await Document.AppendAsync(option.Element);
            await Document.DisableScrollAsync();
            var style = option.Element.Dom(JSRuntime).Style;
            await style.SetAsync("display", "none");
            await style.SetAsync("width", "100%");
            await style.SetAsync("height", "100%");
            await style.SetAsync("position", "fixed");
            await style.SetAsync("top", "0");
            await style.SetAsync("bottom", "0");
            await style.SetAsync("display", "block");
        }


        async Task CloseFullScreenLoadingAsync(LoadingOption option)
        {
            await Document.EnableYScrollAsync();
        }
        async Task RenderLoadingAsync()
        {
            var option = LoadingOptions.FirstOrDefault(x => x.IsNew);
            if (option == null)
            {
                return;
            }
            option.IsNew = false;
            if (string.IsNullOrWhiteSpace(option.Target.Id))
            {
                await ShowFullScreenLoadingAsync(option);
                return;
            }
            await option.Target.Dom(JSRuntime).AppendAsync(option.Element);
            await option.Element.Dom(JSRuntime).Style.SetAsync("display", "block");
        }
    }
}
