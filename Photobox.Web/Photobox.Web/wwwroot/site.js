window.attachMouseEvents = (dotNetHelper) => {
    document.addEventListener("mousemove", (event) => {
        dotNetHelper.invokeMethodAsync("OnMouseMove", {
            clientX: event.clientX,
            clientY: event.clientY
        });
    });

    document.addEventListener("mouseup", () => {
        dotNetHelper.invokeMethodAsync("StopDrag");
    });
};
