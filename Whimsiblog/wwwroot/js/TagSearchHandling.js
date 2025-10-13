document.addEventListener("DOMContentLoaded", function () {
    const input = document.getElementById("tagInput");
    const suggestions = document.getElementById("tagSuggestions");
    const checkboxes = document.getElementById("tagCheckboxes");

    if (!input || !suggestions || !checkboxes) return;

    input.addEventListener("input", async function () {
        const query = input.value.trim();
        suggestions.innerHTML = "";
        suggestions.style.display = "none";

        if (!query) return;

        try {
            const response = await fetch(`/tags/search?query=${encodeURIComponent(query)}`);
            const tags = await response.json();

            if (!tags || tags.length === 0) {
                const addButton = document.createElement("button");
                addButton.type = "button";
                addButton.textContent = `Add "${query}" as new tag`;
                addButton.classList.add("btn", "btn-sm", "btn-primary", "w-100", "mt-2");
                addButton.addEventListener("click", () => addNewTag(query));
                suggestions.appendChild(addButton);
                suggestions.style.display = "block";
                return;
            }

            tags.forEach(tag => {
                const tagName = typeof tag === "string" ? tag : (tag.Name || tag.name);
                const tagId = typeof tag === "object" ? (tag.TagID || tag.tagID) : null;

                const div = document.createElement("div");
                div.textContent = tagName ?? "(unnamed tag)";
                div.classList.add("suggestion-item");

                div.addEventListener("click", () => {
                    input.value = "";
                    suggestions.innerHTML = "";
                    suggestions.style.display = "none";

                    const existing = [...checkboxes.querySelectorAll("label")].find(lbl => {
                        const span = lbl.querySelector("span.form-check-label");
                        return (
                            span &&
                            span.textContent.trim().toLowerCase() === tagName.trim().toLowerCase()
                        );
                    });

                    if (existing) {
                        const cb = existing.querySelector("input[type='checkbox']");
                        if (cb) cb.checked = true;
                    } else {
                        appendCheckbox(tagId, tagName);
                    }
                });

                suggestions.appendChild(div);
            });

            suggestions.style.display = "block";
        } catch (error) {
            console.error("Error fetching tags:", error);
        }
    });

    async function addNewTag(tagName) {
        try {
            const response = await fetch("/tags/add", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(tagName)
            });

            if (response.ok) {
                const newTag = await response.json();
                appendCheckbox(newTag.tagID || newTag.TagID, newTag.name || newTag.Name);
                input.value = "";
                suggestions.innerHTML = "";
                suggestions.style.display = "none";
            } else if (response.status === 409) {
                alert("Tag already exists.");
            } else {
                alert("Failed to add tag.");
            }
        } catch (error) {
            console.error("Error adding tag:", error);
        }
    }

    function appendCheckbox(id, name) {
        const label = document.createElement("label");
        label.classList.add("form-check", "d-block", "mb-1");
        label.innerHTML = `
            <input type="checkbox" name="SelectedTagIDs" value="${id ?? ""}" class="form-check-input" checked />
            <span class="form-check-label">${name}</span>
        `;
        checkboxes.appendChild(label);
    }

    // Hide suggestions when clicking outside
    document.addEventListener("click", (e) => {
        if (!suggestions.contains(e.target) && e.target !== input) {
            suggestions.innerHTML = "";
            suggestions.style.display = "none";
        }
    });
});
