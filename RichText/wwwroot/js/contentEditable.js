
function BlazorContentEditable(id, Instance, TextToDisplay) {

    const domObject = document.getElementById(id);

    //set prefilled text
    domObject.innerHTML = TextToDisplay;

    //set initial height
    SetHeight(domObject);

    //set event listener

    domObject.addEventListener("input", () => {
        Instance.invokeMethodAsync("OnInput", domObject.value);
        SetHeight(domObject);
    });

    domObject.addEventListener("keydown", (event) => {
        if (event.keyCode === 13 ||
            event.keyCode === 38 ||
            event.keyCode === 40 ||
            event.keyCode === 9)
        {
            event.preventDefault();
        }

        Instance.invokeMethodAsync("OnKeyPress", event.keyCode, event.ctrlKey, event.altKey, event.shiftKey);
    });
}

function SetHeight(domObject) {

    //reset to original height
    domObject.style.height = 'auto';

    //from https://gomakethings.com/automatically-expand-a-textarea-as-the-user-types-using-vanilla-javascript/
    // Get the computed styles for the element
    const computed = window.getComputedStyle(domObject);

    // Calculate the height
    const height = parseInt(computed.getPropertyValue('border-top-width'), 10)
        + domObject.scrollHeight
        + parseInt(computed.getPropertyValue('border-bottom-width'), 10);

    //set the height
    domObject.style.height = (height + 'px');

}