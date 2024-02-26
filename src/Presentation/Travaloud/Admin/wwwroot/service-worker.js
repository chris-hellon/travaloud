self.addEventListener("install", (event) => {
    // The promise that skipWaiting() returns can be safely ignored.
    self.skipWaiting();
});
