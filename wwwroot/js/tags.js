let index = 0;


function AddTag() {
    // Get a reference to the TagEntry input element
    var tagEntry = document.getElementById("tagEntry");
    var tagValues = document.getElementById("tagValues");
    var message = document.getElementById("tagMessage");

    // Clear any previous message
    message.textContent = "";

    //Check to make sure tag is not empty
    if (!tagEntry.value.trim()) {
        message.textContent = "Tag cannot be empty.";
        message.style.color = "red";
        return false;
    }
    // Check if the tag already exists in the options
    for (let i = 0; i < tagValues.options.length; i++) {
        if (tagValues.options[i].value === tagEntry.value) {
            // Display a message to indicate a duplicate tag
            message.textContent = `"${tagEntry.value}" has already been included.`;
            message.style.color = "red";

            // Clear out the TagEntry control
            tagEntry.value = "";
            return false;
        }
    }

    // Create a new Select Option
    let newOption = new Option(tagEntry.value, tagEntry.value);
    tagValues.options[index++] = newOption;

    // Clear out the TagEntry control
    tagEntry.value = "";
    return true;
}

function DeleteTag() {
    //get reference to the selected tag in the broswer
    let tagValue = document.getElementById("tagValues").selectedIndex;

    //remove the selected option from the list
    if (index != 0) {
        document.getElementById("tagValues").options[tagValue] = null;
        --index;
    }

}

$('form').on("submit", function () {
    $("#tagValues option").prop("selected", "selected");
})


// Look for allTagValues to see if it has data
if (typeof allTagValues !== "undefined" && allTagValues !== "") {
    let tagArray = allTagValues.split(",");
    for (let i = 0; i < tagArray.length; i++) {
        // Add the tags to the <select> element
        ReplaceTag(tagArray[i], i);
        index++;
    }
}

function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag);
    document.getElementById("tagValues").options[index] = newOption;
}