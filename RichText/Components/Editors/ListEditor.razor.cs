using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.Components;
using RichText.Abstractions;
using RichText.Enums;
using RichText.EventArgs;
using RichText.State;

namespace RichText.Components.Editors
{
    /*
     TODO: more flexible editor inside editor + validation 
    + require interface for data object
    
    + add support for:
    - reordering stuff
    - select items in bulk
    - 
     */
    public partial class ListEditor : IDisposable
    {
        [Inject] public IAppState AppState { get; set; } = default!;

        [Parameter] public ListState State { get; set; } = default!;

        private string? Error { get; set; }

        protected override void OnInitialized()
        {
            State.StateHasChanged += StateHasChangedAsync;
        }

        private async void StateHasChangedAsync(object? sender, System.EventArgs e)
        {
            await InvokeAsync(() => StateHasChanged());
        }

        private async Task HandleKeyPressAsync(KeyEventArgs args, IEntity source)
        {
            if (State == null)
            {
                return;
            }

            try
            {
                try
                {
                    await ProcessKeyPressAsync(args, source);
                }
                catch
                {
                    // retry once
                    await ProcessKeyPressAsync(args, source);
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }

        private async Task ProcessKeyPressAsync(KeyEventArgs args, IEntity source)
        {
            var keyPressSuccessful = args.Key switch
            {
                Keys.Enter when source.IsSaveable => State.AddElement(source),
                Keys.Up => State.SelectElementPreviousOf(source, args.Modifiers == ModifierKeys.Control),
                Keys.Down => State.SelectElementNextOf(source, args.Modifiers == ModifierKeys.Control),
                Keys.Tab when args.Modifiers == ModifierKeys.Shift => await State.PromoteAsync(source),
                Keys.Tab => await State.DemoteAsync(source),

                _ => default(bool?)
            };

            if (keyPressSuccessful != null)
            {
                StateHasChanged();
            }

            if (keyPressSuccessful == true)
            {
                await HandleDefocusAsync(source);
            }
        }

        private async Task HandleDefocusAsync(IEntity source)
        {
            // TODO: when making edits while saving leads to chaos
            await State.SaveElementAsync(source);
        }

        public void Dispose()
        {
            if (State == null)
            {
                return;
            }

            State.StateHasChanged -= StateHasChangedAsync;
        }

    }
}
