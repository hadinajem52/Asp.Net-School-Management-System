(() => {
    const content = document.querySelector("#app-content");
    const loadingIndicator = document.querySelector("#loading-indicator");
    const confirmationDialog = document.querySelector("#confirmation-dialog");
    let pendingConfirmation = null;

    if (!content || !loadingIndicator || !confirmationDialog) {
        return;
    }

    const setLoading = (isLoading) => {
        loadingIndicator.hidden = !isLoading;
        content.setAttribute("aria-busy", isLoading.toString());
    };

    const showRequestError = () => {
        const alert = document.createElement("div");
        alert.className = "app-alert app-alert-error";
        alert.setAttribute("role", "alert");
        alert.textContent = "The request could not be completed. Please try again.";
        content.prepend(alert);
    };

    const refreshValidation = () => {
        if (window.jQuery?.validator?.unobtrusive) {
            window.jQuery.validator.unobtrusive.parse(content);
        }
    };

    const renderResponse = (html, responseUrl, addHistoryEntry) => {
        const page = new DOMParser().parseFromString(html, "text/html");
        const updatedContent = page.querySelector("#app-content");

        if (!updatedContent) {
            window.location.assign(responseUrl);
            return;
        }

        content.innerHTML = updatedContent.innerHTML;
        document.title = page.title;

        if (addHistoryEntry) {
            window.history.pushState({}, "", responseUrl);
        } else {
            window.history.replaceState({}, "", responseUrl);
        }

        refreshValidation();
        const pageHeading = content.querySelector("h1");
        if (pageHeading) {
            pageHeading.tabIndex = -1;
            pageHeading.focus({ preventScroll: true });
        }
        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    const requestPage = async (url, options, addHistoryEntry) => {
        setLoading(true);

        try {
            const response = await fetch(url, {
                ...options,
                headers: {
                    ...options?.headers,
                    "X-Requested-With": "XMLHttpRequest"
                }
            });

            if (new URL(response.url).pathname.startsWith("/Account/Login")) {
                window.location.assign(response.url);
                return;
            }

            if (!response.ok) {
                throw new Error(`Request failed with status ${response.status}.`);
            }

            renderResponse(await response.text(), response.url, addHistoryEntry);
        } catch (error) {
            if (error.name !== "AbortError") {
                showRequestError();
            }
        } finally {
            setLoading(false);
        }
    };

    const submitForm = (form) => {
        const method = form.method.toUpperCase();

        if (method === "GET") {
            const url = new URL(form.action);
            new FormData(form).forEach((value, key) => {
                if (value.toString().length > 0) {
                    url.searchParams.set(key, value.toString());
                }
            });
            requestPage(url, { method: "GET" }, true);
            return;
        }

        requestPage(form.action, {
            method,
            body: new FormData(form)
        }, false);
    };

    const requestConfirmation = (form) => {
        pendingConfirmation = form;
        confirmationDialog.querySelector("#confirmation-title").textContent = form.dataset.confirmTitle;
        confirmationDialog.querySelector("#confirmation-message").textContent = form.dataset.confirmMessage;
        confirmationDialog.querySelector("#confirmation-accept").textContent = form.dataset.confirmLabel ?? "Confirm";
        confirmationDialog.showModal();
    };

    document.addEventListener("click", (event) => {
        const link = event.target.closest("a[href]");

        if (!link || event.defaultPrevented || event.button !== 0 || event.metaKey || event.ctrlKey ||
            event.shiftKey || event.altKey || link.target || link.hasAttribute("download") ||
            link.dataset.noAjax !== undefined) {
            return;
        }

        const url = new URL(link.href, window.location.href);
        if (url.origin !== window.location.origin || url.hash || !link.closest("main, .site-navigation")) {
            return;
        }

        event.preventDefault();
        requestPage(url, { method: "GET" }, true);
    });

    document.addEventListener("submit", (event) => {
        const form = event.target;

        if (event.defaultPrevented || !(form instanceof HTMLFormElement) ||
            !form.closest("#app-content") || form.closest(".auth-page") || form.dataset.noAjax !== undefined) {
            return;
        }

        event.preventDefault();

        if (form.dataset.confirmTitle && form.dataset.confirmed !== "true") {
            requestConfirmation(form);
            return;
        }

        delete form.dataset.confirmed;
        submitForm(form);
    });

    confirmationDialog.querySelector("#confirmation-accept").addEventListener("click", () => {
        if (pendingConfirmation) {
            const confirmedForm = pendingConfirmation;
            confirmedForm.dataset.confirmed = "true";
            confirmationDialog.close();
            confirmedForm.requestSubmit();
        }
    });

    confirmationDialog.querySelector("#confirmation-cancel").addEventListener("click", () => {
        confirmationDialog.close();
    });

    confirmationDialog.addEventListener("close", () => {
        pendingConfirmation = null;
    });

    window.addEventListener("popstate", () => {
        requestPage(window.location.href, { method: "GET" }, false);
    });
})();
