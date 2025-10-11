document.addEventListener("DOMContentLoaded", function () {
    const input = document.getElementById("tagInput");
    const suggestions = document.getElementById("tagSuggestions");
    const checkboxes = document.getElementById("tagCheckboxes");

    let currentSuggestions = [];

    input.addEventListener("input", async function () {
        const query = input.value.trim();

    if (!query) {
        suggestions.innerHTML = "";
    return;
        }

    try {
            const response = await fetch(`/tags/search?query=${encodeURIComponent(query)}`);
    const tags = await response.json();
    currentSuggestions = tags;

    suggestions.innerHTML = "";

            if (tags.length > 0) {
        tags.forEach(tag => {
            const div = document.createElement("div");
            div.textContent = tag.Name;
            div.classList.add("suggestion-item");
            div.addEventListener("click", () => {
                input.value = tag.Name;
                suggestions.innerHTML = "";
            });
            suggestions.appendChild(div);
        });
            } else {
                const addButton = document.createElement("button");
                addButton.type = "button";
    addButton.textContent = `Add "${query}" as new tag`;
    addButton.classList.add("btn", "btn-sm", "btn-primary", "mt-2");
                addButton.addEventListener("click", () => addNewTag(query));
    suggestions.appendChild(addButton);
            }
        } catch (error) {
        console.error("Error fetching tags:", error);
        }
    });

    async function addNewTag(tagName) {
        try {
            const response = await fetch("/tags/add", {
        method: "POST",
    headers: {"Content-Type": "application/json" },
    body: JSON.stringify(tagName)
            });

    if (response.ok) {
                const newTag = await response.json();
        appendCheckbox(newTag.tagID, newTag.name);
    input.value = "";
    suggestions.innerHTML = "";
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
    label.innerHTML = `
    <input type="checkbox" name="SelectedTagIDs" value="${id}" checked />
    ${name}
    `;
    checkboxes.appendChild(label);
    checkboxes.appendChild(document.createElement("br"));
    }
});