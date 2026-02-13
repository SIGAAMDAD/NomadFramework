/* ---------------------------------------------------------
   UE5‑STYLE JS OVERRIDES FOR DOXYGEN-AWESOME
   + METADATA EXTRACTION
--------------------------------------------------------- */

document.addEventListener("DOMContentLoaded", () => {

    /* --- SIDEBAR BEHAVIOR -------------------------------- */
    const active = document.querySelector("#nav-tree .selected");
    if (active) {
        active.scrollIntoView({ block: "center", behavior: "smooth" });
    }

    document.querySelectorAll("#nav-tree .folder").forEach(folder => {
        folder.addEventListener("click", () => {
            const parent = folder.parentElement;
            const siblings = parent.parentElement.querySelectorAll(":scope > li");

            siblings.forEach(sib => {
                if (sib !== parent) {
                    const ul = sib.querySelector("ul");
                    if (ul) ul.style.display = "none";
                }
            });
        });
    });

    /* --- METADATA INJECTION ------------------------------- */
    const title = document.querySelector("h1.title");
    if (!title) return;

    // Doxygen stores metadata in <table class="memberdecls"> or <div class="summary">
    const file = document.querySelector(".location");
    const typeNode = document.querySelector(".compoundname");

    const type = typeNode ? typeNode.textContent.trim() : null;
    const fileName = file ? file.textContent.replace("Definition at", "").trim() : null;

    if (!type && !fileName) return;

    const wrapper = document.createElement("div");
    wrapper.className = "ue5-metadata";

    wrapper.innerHTML = `
        <table>
            ${type ? `<tr><td class="label">Type</td><td>${type}</td></tr>` : ""}
            ${fileName ? `<tr><td class="label">File</td><td>${fileName}</td></tr>` : ""}
        </table>
    `;

    title.insertAdjacentElement("afterend", wrapper);
});