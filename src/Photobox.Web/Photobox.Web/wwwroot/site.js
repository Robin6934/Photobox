window.renderTextAsPng = (text, fontSize, color) => {
    return new Promise((resolve, reject) => {
        try {
            const canvas = document.createElement("canvas");
            const ctx = canvas.getContext("2d");

            // Set the font first to measure text
            ctx.font = `${fontSize}px Georgia`;
            const textMetrics = ctx.measureText(text);

            // Calculate text width and height
            const textWidth = textMetrics.width;
            const textHeight = fontSize; // Approximate height using font size

            // Set canvas dimensions to fit the text
            canvas.width = Math.ceil(textWidth);
            canvas.height = Math.ceil(textHeight);

            // Redefine font and styles since setting canvas width/height resets them
            ctx.font = `${fontSize}px Georgia`;
            ctx.fillStyle = color;
            ctx.textBaseline = "top"; // Align text properly
            ctx.fillText(text, 0, 0);

            // Convert to PNG
            resolve(canvas.toDataURL("image/png"));
        } catch (error) {
            reject(error);
        }
    });
};
// Attach Mouse Events Separately
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
