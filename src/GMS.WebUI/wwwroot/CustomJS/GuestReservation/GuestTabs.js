//function ValidateAndNext() {
//    // First validate the form (you can uncomment your validation logic)
//    // if (!_isValidateForm("GuestInformationTab")) {
//    //     return false;
//    // }

//    // Get the currently active tab
//    const currentTab = $('.nav-link.active');

//    // Find the next tab in the list
//    const nextTabListItem = currentTab.closest('li').next('li');

//    if (nextTabListItem.length > 0) {
//        // Get the tab link element
//        const nextTabLink = nextTabListItem.find('.nav-link');

//        // Trigger the click event to switch tabs (Bootstrap's way)
//        nextTabLink.tab('show');

//        // Alternatively, you can use:
//        // nextTabLink.click();
//    }

//    return false; // Prevent default behavior if needed
//}

function ValidateAndNext() {



    // First validate the form
    if (!_isValidateForm("GuestInformationTab")) {
        return false; // Don't proceed if validation fails
    }

    // If validation passes, switch to next tab
    const currentTab = $('.nav-link.active');
    const nextTabListItem = currentTab.closest('li').next('li');

    if (nextTabListItem.length > 0) {
        const nextTabLink = nextTabListItem.find('.nav-link');
        nextTabLink.tab('show');
    }

    return false;
}

function PreviousTab() {
    // Get the currently active tab
    const currentTab = $('.nav-link.active');

    // Find the previous tab in the list
    const prevTabListItem = currentTab.closest('li').prev('li');

    if (prevTabListItem.length > 0) {
        // Get the tab link element
        const prevTabLink = prevTabListItem.find('.nav-link');

        // Trigger the click event to switch tabs
        prevTabLink.tab('show');
    }

    return false; // Prevent default behavior if needed
}