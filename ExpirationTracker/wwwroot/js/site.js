document.querySelectorAll(".file-drop-zone").forEach((dropZone) => {
    const input = dropZone.querySelector(".file-drop-input");
    const fileName = dropZone.querySelector(".file-drop-name");

    if (!input || !fileName) {
        return;
    }

    const setFileName = () => {
        fileName.textContent = input.files?.[0]?.name || "No file selected";
    };

    input.addEventListener("change", setFileName);

    dropZone.addEventListener("dragover", (event) => {
        event.preventDefault();
        dropZone.classList.add("is-drag-over");
    });

    dropZone.addEventListener("dragleave", (event) => {
        if (!dropZone.contains(event.relatedTarget)) {
            dropZone.classList.remove("is-drag-over");
        }
    });

    dropZone.addEventListener("drop", (event) => {
        event.preventDefault();
        dropZone.classList.remove("is-drag-over");

        if (event.dataTransfer?.files?.length) {
            input.files = event.dataTransfer.files;
            input.dispatchEvent(new Event("change", { bubbles: true }));
        }
    });
});
