//not being used currently

document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");

    form.addEventListener("submit", function (event) {
        // Prevent form submission
        event.preventDefault();

        // Get values from input fields
        const username = document.getElementById("UserRegisterDto_Username")?.value;
        const password = document.getElementById("UserRegisterDto_Password")?.value;
        const email = document.getElementById("UserRegisterDto_Email")?.value;
        const phoneNumber = document.getElementById("UserRegisterDto_PhoneNumber")?.value;

        // Validation flags
        let valid = true;
        let errorMessage = "";

        // Simple validation logic
        if (!username) {
            valid = false;
            errorMessage += "Username is required.\n";
        }
        if (!password) {
            valid = false;
            errorMessage += "Password is required.\n";
        } else if (password.length < 6) {
            valid = false;
            errorMessage += "Password must be at least 6 characters long.\n";
        }
        if (!email || !validateEmail(email)) {
            valid = false;
            errorMessage += "Valid email is required.\n";
        }
        if (!phoneNumber) {
            valid = false;
            errorMessage += "Phone number is required.\n";
        }

        // Show error messages
        if (!valid) {
            alert(errorMessage);
        } else {
            // If valid, submit the form
            form.submit();
        }
    });

    function validateEmail(email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(String(email).toLowerCase());
    }
});
