window.numeralSystems = {
    copyText: async function (text) {
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(text);
            return;
        }

        const textarea = document.createElement("textarea");
        textarea.value = text;
        textarea.setAttribute("readonly", "");
        textarea.style.position = "fixed";
        textarea.style.opacity = "0";
        document.body.appendChild(textarea);
        textarea.select();
        document.execCommand("copy");
        textarea.remove();
    },

    downloadText: function (fileName, content, contentType) {
        const blob = new Blob([content], { type: contentType });
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement("a");
        anchor.href = url;
        anchor.download = fileName;
        document.body.appendChild(anchor);
        anchor.click();
        anchor.remove();
        window.setTimeout(() => URL.revokeObjectURL(url), 0);
    },

    downloadSvg: function (elementId, fileName) {
        const source = document.getElementById(elementId);
        if (!source) {
            throw new Error("The requested SVG element was not found.");
        }

        const clone = source.cloneNode(true);
        clone.setAttribute("xmlns", "http://www.w3.org/2000/svg");
        const content = '<?xml version="1.0" encoding="UTF-8"?>\n' +
            new XMLSerializer().serializeToString(clone);
        window.numeralSystems.downloadText(fileName, content, "image/svg+xml;charset=utf-8");
    }
};
