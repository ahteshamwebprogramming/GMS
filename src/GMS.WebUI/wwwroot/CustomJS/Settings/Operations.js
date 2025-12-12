(function () {
    $(document).ready(function () {
        initializeForms();
        initializeTabs();
    });

    function initializeForms() {
        // Logo Form
        $("#LogoForm").on("submit", function (e) {
            e.preventDefault();
            saveLogo();
        });

        $("#LogoFile").on("change", handleLogoPreview);
        $("#btnResetLogo").on("click", resetLogoForm);

        // Email Form
        $("#EmailForm").on("submit", function (e) {
            e.preventDefault();
            saveEmail();
        });
        $("#btnResetEmail").on("click", resetEmailForm);

        // Feedback Form
        $("#FeedbackForm").on("submit", function (e) {
            e.preventDefault();
            saveFeedback();
        });
        $("#btnResetFeedback").on("click", resetFeedbackForm);
    }

    function initializeTabs() {
        // Load data when switching tabs
        $('#operationsTabs button[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
            const targetTab = $(e.target).data("bs-target");
            // You can load tab-specific data here if needed
        });
    }

    function saveLogo() {
        const formElement = document.getElementById("LogoForm");
        if (!formElement) {
            return;
        }

        const formData = new FormData(formElement);

        BlockUI();
        $.ajax({
            url: "/Settings/Operations/SaveLogo",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                UnblockUI();
                $successalert("", "Logo saved successfully.");

                if (response) {
                    if (response.id) {
                        $("#LogoId").val(response.id);
                        $("#EmailId").val(response.id);
                        $("#FeedbackId").val(response.id);
                    }

                    if (response.logoFilePath) {
                        $("#LogoFilePath").val(response.logoFilePath);
                        toggleLogoPreview(response.logoFilePath);
                    }
                }
            },
            error: function (xhr) {
                UnblockUI();
                const message = xhr && xhr.responseText ? xhr.responseText : "Unable to save logo.";
                $erroralert("Save Failed", message);
            }
        });
    }

    function saveEmail() {
        const formData = {
            Id: $("#EmailId").val(),
            SmtpServer: $("#SmtpServer").val(),
            SmtpPort: parseInt($("#SmtpPort").val()) || null,
            SmtpUsername: $("#SmtpUsername").val(),
            SmtpPassword: $("#SmtpPassword").val(),
            SmtpEnableSsl: $("#SmtpEnableSsl").is(":checked"),
            SmtpFromEmail: $("#SmtpFromEmail").val(),
            SmtpFromName: $("#SmtpFromName").val()
        };

        BlockUI();
        $.ajax({
            url: "/Settings/Operations/SaveEmail",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                UnblockUI();
                $successalert("", "Email settings saved successfully.");

                if (response && response.id) {
                    $("#EmailId").val(response.id);
                    $("#LogoId").val(response.id);
                    $("#FeedbackId").val(response.id);
                }
            },
            error: function (xhr) {
                UnblockUI();
                const message = xhr && xhr.responseText ? xhr.responseText : "Unable to save email settings.";
                $erroralert("Save Failed", message);
            }
        });
    }

    function saveFeedback() {
        const formData = {
            Id: parseInt($("#FeedbackId").val()) || 0,
            FeedbackWelcomeTitle: $("#FeedbackWelcomeTitle").val() || "",
            FeedbackWelcomeMessage1: $("#FeedbackWelcomeMessage1").val() || "",
            FeedbackWelcomeMessage2: $("#FeedbackWelcomeMessage2").val() || "",
            FeedbackWelcomeMessage3: $("#FeedbackWelcomeMessage3").val() || ""
        };

        BlockUI();
        $.ajax({
            url: "/Settings/Operations/SaveFeedback",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                UnblockUI();
                $successalert("", "Feedback content saved successfully.");

                if (response) {
                    if (response.id) {
                        $("#FeedbackId").val(response.id);
                        $("#LogoId").val(response.id);
                        $("#EmailId").val(response.id);
                    }
                    
                    // Update form fields with saved values
                    if (response.feedbackWelcomeTitle !== undefined) {
                        $("#FeedbackWelcomeTitle").val(response.feedbackWelcomeTitle || "");
                    }
                    if (response.feedbackWelcomeMessage1 !== undefined) {
                        $("#FeedbackWelcomeMessage1").val(response.feedbackWelcomeMessage1 || "");
                    }
                    if (response.feedbackWelcomeMessage2 !== undefined) {
                        $("#FeedbackWelcomeMessage2").val(response.feedbackWelcomeMessage2 || "");
                    }
                    if (response.feedbackWelcomeMessage3 !== undefined) {
                        $("#FeedbackWelcomeMessage3").val(response.feedbackWelcomeMessage3 || "");
                    }
                }
            },
            error: function (xhr, status, error) {
                UnblockUI();
                let message = "Unable to save feedback content.";
                
                if (xhr.responseText) {
                    try {
                        const errorObj = JSON.parse(xhr.responseText);
                        message = errorObj.message || errorObj.title || xhr.responseText;
                    } catch (e) {
                        message = xhr.responseText;
                    }
                } else if (xhr.status === 0) {
                    message = "Network error. Please check your connection.";
                } else if (xhr.status === 400) {
                    message = "Invalid request. Please check your input.";
                } else if (xhr.status === 500) {
                    message = "Server error. Please try again later.";
                }
                
                console.error("Save Feedback Error:", {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    responseText: xhr.responseText,
                    error: error
                });
                
                $erroralert("Save Failed", message);
            }
        });
    }

    function handleLogoPreview(event) {
        const file = event.target.files[0];
        if (!file) {
            toggleLogoPreview($("#LogoFilePath").val());
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            toggleLogoPreview(e.target.result, true);
        };
        reader.readAsDataURL(file);
    }

    function toggleLogoPreview(source, isTemporary) {
        const $previewContainer = $("#logoPreview");
        const $previewImage = $("#logoPreviewImg");

        if (!source) {
            $previewContainer.addClass("d-none");
            $previewImage.attr("src", "#");
            return;
        }

        $previewImage.attr("src", source);
        if (isTemporary) {
            $previewImage.addClass("temporary-logo");
        } else {
            $previewImage.removeClass("temporary-logo");
        }
        $previewContainer.removeClass("d-none");
    }

    function resetLogoForm() {
        const formElement = document.getElementById("LogoForm");
        if (formElement) {
            formElement.reset();
        }
        toggleLogoPreview($("#LogoFilePath").val());
    }

    function resetEmailForm() {
        const formElement = document.getElementById("EmailForm");
        if (formElement) {
            formElement.reset();
        }
        // Reload email data
        loadEmailData();
    }

    function resetFeedbackForm() {
        const formElement = document.getElementById("FeedbackForm");
        if (formElement) {
            formElement.reset();
        }
        // Reload feedback data
        loadFeedbackData();
    }

    function loadEmailData() {
        $.ajax({
            url: "/Settings/Operations/GetOperations",
            type: "POST",
            success: function (data) {
                if (data) {
                    $("#EmailId").val(data.id || "");
                    $("#SmtpServer").val(data.smtpServer || "");
                    $("#SmtpPort").val(data.smtpPort || "");
                    $("#SmtpUsername").val(data.smtpUsername || "");
                    $("#SmtpPassword").val(data.smtpPassword || "");
                    $("#SmtpFromEmail").val(data.smtpFromEmail || "");
                    $("#SmtpFromName").val(data.smtpFromName || "");
                    $("#SmtpEnableSsl").prop("checked", data.smtpEnableSsl || false);
                }
            }
        });
    }

    function loadFeedbackData() {
        $.ajax({
            url: "/Settings/Operations/GetOperations",
            type: "POST",
            success: function (data) {
                if (data) {
                    $("#FeedbackId").val(data.id || "");
                    $("#FeedbackWelcomeTitle").val(data.feedbackWelcomeTitle || "");
                    $("#FeedbackWelcomeMessage1").val(data.feedbackWelcomeMessage1 || "");
                    $("#FeedbackWelcomeMessage2").val(data.feedbackWelcomeMessage2 || "");
                    $("#FeedbackWelcomeMessage3").val(data.feedbackWelcomeMessage3 || "");
                }
            }
        });
    }
})();

