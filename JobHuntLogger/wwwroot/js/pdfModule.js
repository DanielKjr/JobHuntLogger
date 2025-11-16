
    export async function setIframeSrc(iframeEl, url) {
        if (!iframeEl) return;
        iframeEl.src = `data:application/pdf;base64,${url}`;
    } ;

