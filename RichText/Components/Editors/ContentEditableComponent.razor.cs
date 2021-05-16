using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RichText.Enums;
using RichText.EventArgs;
using RichText.Models;

namespace RichText.Components.Editors
{
    //TODO:
    //- improve casing
    //- support focussing editor
    //- support list of "banned" keys that must be prevented

    public partial class ContentEditableComponent
    {
        private int _id { get; set; } = new Random().Next();
        
        private readonly Dictionary<string, object> _attributesList = new();

        private ElementReference _elementRef;

        [Parameter] public string? Value { get; set; }

        [Parameter] public ListElement Element { get; set; } = default!;

        [Parameter] public bool Enabled { get; set; } = true;

        [Parameter] public string? CSSClass { get; set; }

        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        [Parameter] public EventCallback<KeyEventArgs> KeyPress { get; set; }

        [Inject] private IJSRuntime _JSRuntime { get; set; } = default!;

        protected override void OnInitialized()
        {
            //Enabled=false add disabled attribute to AttributesList
            if (!Enabled)
            {
                _attributesList.Add("disabled", "disabled");
            }

            //add CSSClass if supplied
            if (!string.IsNullOrWhiteSpace(CSSClass))
            {
                _attributesList.Add("class", CSSClass);
            }
        }

        //send initial text (if supplied) to javascript to place in the div
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _JSRuntime.InvokeAsync<string>("BlazorContentEditable", _id, DotNetObjectReference.Create(this), Value);
            }

            if (Element.Focus)
            {
                Element.Focus = false;

                await _elementRef.FocusAsync();
            }
        }

        [JSInvokable]
        public void OnInput(string value)
        {
            ValueChanged.InvokeAsync(value);
        }

        [JSInvokable]
        public void OnKeyPress(int keyCode, bool controlKey, bool altKey, bool shiftKey)
        {
            KeyPress.InvokeAsync(new KeyEventArgs(
                (Keys)keyCode, 
                (controlKey ? ModifierKeys.Control : 0) | 
                    (altKey ? ModifierKeys.Alt : 0) |
                    (shiftKey ? ModifierKeys.Shift: 0)));
        }
    }
}
