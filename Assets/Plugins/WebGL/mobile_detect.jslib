mergeInto(LibraryManager.library, {
    WebGLDetectMobile: function () {
        var ua = navigator.userAgent || navigator.vendor || window.opera;

        if (/android/i.test(ua)) return true;
        if (/iPad|iPhone|iPod/.test(ua)) return true;
        if (/windows phone/i.test(ua)) return true;

        // Detect tablets
        if (/Mobile|Tablet|Phone/i.test(ua)) return true;

        return false;
    }
});