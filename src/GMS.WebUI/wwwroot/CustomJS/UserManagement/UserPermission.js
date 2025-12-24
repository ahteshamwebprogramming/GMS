$(document).ready(function() {
    // Initialize Select2 for searchable dropdown
    $('#userDropdown').select2({
        placeholder: '-- Select User --',
        allowClear: false,
        width: '100%'
    });
    
    loadUsers();
    
    $('#userDropdown').on('change', function() {
        const userId = $(this).val();
        const selectedOption = $(this).find('option:selected');
        const isUserSelected = userId !== '0' && userId !== null && userId !== '';
        
        // Enable/disable button
        $('#btnEditPermissions').prop('disabled', !isUserSelected);
        
        // Show/hide empty state and info panel
        if (isUserSelected) {
            $('#emptyStateHelper').addClass('hide');
            $('#userInfoPanel').addClass('show');
            
            // Extract role name from option text (format: "UserName (RoleName)")
            const optionText = selectedOption.text();
            const roleMatch = optionText.match(/\(([^)]+)\)/);
            const roleName = roleMatch ? roleMatch[1] : 'N/A';
            $('#infoRoleName').text(roleName);
        } else {
            $('#emptyStateHelper').removeClass('hide');
            $('#userInfoPanel').removeClass('show');
            $('#infoRoleName').text('-');
        }
    });
    
    $('#btnEditPermissions').click(function(e) {
        e.preventDefault();
        const userId = $('#userDropdown').val();
        
        if (!userId || userId === '0' || userId === null || userId === '') {
            alert('Please select a user first.');
            return;
        }
        
        const url = '/UserPermission/Edit/' + userId;
        window.location.href = url;
    });
});

function loadUsers() {
    BlockUI();
    $.ajax({
        url: '/UserPermission/GetUsers',
        type: 'GET',
        success: function(data) {
            UnblockUI();
            const dropdown = $('#userDropdown');
            dropdown.empty();
            dropdown.append('<option value="0">-- Select User --</option>');
            
            if (data && data.length > 0) {
                data.forEach(function(user) {
                    dropdown.append(`<option value="${user.userId}">${user.userName} (${user.roleName})</option>`);
                });
            }
            
            // Trigger Select2 update to refresh the dropdown
            dropdown.trigger('change.select2');
        },
        error: function() {
            UnblockUI();
            alert('Error loading users');
        }
    });
}

