
function BlazorContentEditable(id, Instance, TextToDisplay) {

    const domObject = document.getElementById(id);

    domObject.innerHTML = TextToDisplay;

    domObject.addEventListener("input", () => {
        Instance.invokeMethodAsync("OnInput", domObject.value);
    });

    domObject.addEventListener("keydown", (event) => {
        if (event.keyCode === 13 ||
            event.keyCode === 38 ||
            event.keyCode === 40 ||
            event.keyCode === 9) {
            event.preventDefault();
        }

        Instance.invokeMethodAsync("OnKeyPress", event.keyCode, event.ctrlKey, event.altKey, event.shiftKey);
    });

    domObject.addEventListener("focus", (event) => {
        domObject.selectionStart = domObject.selectionEnd = domObject.value.length;
    });
}
