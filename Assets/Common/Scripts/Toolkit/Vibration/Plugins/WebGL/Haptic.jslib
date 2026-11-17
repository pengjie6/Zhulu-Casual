var HapticPlugin = {
    // 状态挂在 Module 上，不要用 this。
    // 原因：Emscripten 从 wasm 侧导入调用 jslib 函数时不会绑定 this，
    // 在抖音 TTSDK（Closure 压缩 / 严格模式）产出的 webgl.framework.js 里 this 为 undefined，
    // 直接 this.patterns = {} 会抛 "Cannot set properties of undefined (setting 'patterns')"，
    // 异常穿过 wasm 边界后整个引擎 quit: status 1（表现为启动黑屏）。
    _Initialize: function() {
        Module.HapticPatterns = {};
    },

    _Play: function(duration) {
        if (typeof navigator !== "undefined" && navigator.vibrate) {
            navigator.vibrate(duration);
        }
    },

    _RegisterPattern: function(idPtr, idLen, ptr, length) {
        var id = UTF8ToString(idPtr, idLen);

        var pattern = [];
        for (var i = 0; i < length; i++) {
            pattern.push(HEAP32[(ptr >> 2) + i]);
        }

        if (!Module.HapticPatterns) {
            Module.HapticPatterns = {};
        }

        Module.HapticPatterns[id] = pattern;
    },

    _PlayPattern: function(idPtr, idLen) {
        var id = UTF8ToString(idPtr, idLen);

        var pattern = Module.HapticPatterns ? Module.HapticPatterns[id] : undefined;
        if (pattern) {
            if (typeof navigator !== "undefined" && navigator.vibrate) {
                navigator.vibrate(pattern);
            }
        } else {
            console.error("Haptic pattern with ID '" + id + "' can't be found!");
        }
    }
};

mergeInto(LibraryManager.library, HapticPlugin);
