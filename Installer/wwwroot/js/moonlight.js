window.moonlight = {
    toasts: {
        success: function(title, message, timeout)
        {
            this.show(title, message, timeout, "success");
        },
        danger: function(title, message, timeout)
        {
            this.show(title, message, timeout, "danger");
        },
        warning: function(title, message, timeout)
        {
            this.show(title, message, timeout, "warning");
        },
        info: function(title, message, timeout)
        {
            this.show(title, message, timeout, "info");
        },
        show: function(title, message, timeout, color)
        {
            var toast = new ToastHelper(title, message, color, timeout);
            toast.show();
        },
        create: function (id, text) {
            var toast = new ToastHelper("Progress", text, "secondary", 0);
            toast.showAlways();

            toast.domElement.setAttribute('data-ml-toast-id', id);
        },
        modify: function (id, text) {
            var toast = document.querySelector('[data-ml-toast-id="' + id + '"]');

            toast.getElementsByClassName("toast-body")[0].innerText = text;
        },
        remove: function (id) {
            var toast = document.querySelector('[data-ml-toast-id="' + id + '"]');
            bootstrap.Toast.getInstance(toast).hide();

            setTimeout(() => {
                toast.remove();
            }, 2);
        }
    },
    modals: {
        show: function (id, focus)
        {
            let modal = new bootstrap.Modal(document.getElementById(id), {
                focus: focus
            });
            
            modal.show();
        },
        hide: function (id)
        {
            let element = document.getElementById(id)
            let modal = bootstrap.Modal.getInstance(element)
            modal.hide()
        }
    },
    alerts: {
        getHelper: function () {
            return Swal.mixin({
                customClass: {
                    confirmButton: 'btn btn-success',
                    cancelButton: 'btn btn-danger',
                    denyButton: 'btn btn-danger',
                    htmlContainer: 'text-white'
                },
                buttonsStyling: false
            });
        },
        info: function (title, description) {
            this.getHelper().fire(
                title,
                description,
                'info'
            )
        },
        success: function (title, description) {
            this.getHelper().fire(
                title,
                description,
                'success'
            )
        },
        warning: function (title, description) {
            this.getHelper().fire(
                title,
                description,
                'warning'
            )
        },
        error: function (title, description) {
            this.getHelper().fire(
                title,
                description,
                'error'
            )
        },
        yesno: function (title, yesText, noText) {
            return this.getHelper().fire({
                title: title,
                showDenyButton: true,
                confirmButtonText: yesText,
                denyButtonText: noText,
            }).then((result) => {
                if (result.isConfirmed) {
                    return true;
                } else if (result.isDenied) {
                    return false;
                }
            })
        },
        text: async function (title, description, initialValue) {
            const {value: text} = await this.getHelper().fire({
                title: title,
                input: 'text',
                inputLabel: description,
                inputValue: initialValue,
                showCancelButton: false
            })

            return text;
        }
    },
    utils: {
        download: async function (fileName, contentStreamReference) {
            const arrayBuffer = await contentStreamReference.arrayBuffer();
            const blob = new Blob([arrayBuffer]);
            const url = URL.createObjectURL(blob);
            const anchorElement = document.createElement('a');
            anchorElement.href = url;
            anchorElement.download = fileName ?? '';
            anchorElement.click();
            anchorElement.remove();
            URL.revokeObjectURL(url);
        }
    },
    clipboard: {
        copy: function (text) {
            if (!navigator.clipboard) {
                var textArea = document.createElement("textarea");
                textArea.value = text;

                // Avoid scrolling to bottom
                textArea.style.top = "0";
                textArea.style.left = "0";
                textArea.style.position = "fixed";

                document.body.appendChild(textArea);
                textArea.focus();
                textArea.select();

                try {
                    var successful = document.execCommand('copy');
                    var msg = successful ? 'successful' : 'unsuccessful';
                } catch (err) {
                    console.error('Fallback: Oops, unable to copy', err);
                }

                document.body.removeChild(textArea);
                return;
            }
            navigator.clipboard.writeText(text).then(function () {
                },
                function (err) {
                    console.error('Async: Could not copy text: ', err);
                }
            );
        }
    }
}