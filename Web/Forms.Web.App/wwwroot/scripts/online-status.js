let handler;

window.OnlineStatus = {
    Initialize: function (interop) {
        handler = function () {
            interop.invokeMethodAsync("OnlineStatus.StatusChanged", navigator.onLine);
        }

        window.addEventListener("online", handler);
        window.addEventListener("offline", handler);

        interop.invokeMethodAsync("OnlineStatus.StatusChanged", navigator.onLine);
        console.log("Initial online status:", navigator.onLine);
    },
    Dispose: function () {
        if (handler) {
            window.removeEventListener("online", handler);
            window.removeEventListener("offline", handler);
        }
    }
};
