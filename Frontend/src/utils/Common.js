const COLOR_EMPTY = { r: 0, g: 0, b: 0, a: 0 };

const LOCALE_RU = 'ru';
const LOCALE_EN = 'en';

const LOCALES = [
    LOCALE_RU,
    LOCALE_EN
];

export default class Common {
    static tokenKey = 'accessToken';

    static transformErrorData(error) {
        if (error?.response?.data?.errors) {
            if (error.response.data.errors.toString() === error.response.data.errors) {
                return error.response.data.errors;
            } else if ('message' in error.response.data.errors && 'stackTrace' in error.response.data.errors) {
                console.log(error.response.data.errors.stackTrace);
                return error.response.data.errors.message;
            } else {
                return Object
                    .keys(error.response.data.errors)
                    .filter(e => error.response.data.errors[e])
                    .map(e => `${e}: ${error.response.data.errors[e]?.join(', ')}`);
            }
        }
        else {
            return error?.response?.data ? JSON.stringify(error?.response?.data, null, 2) : 'N/A';
        }
    }

    /**
     * Determines if access token is present
     * @returns {Boolean}
     */
    static isTokenPresent() {
        return !!localStorage.getItem(this.tokenKey) || !!sessionStorage.getItem(this.tokenKey);
    }

    static resetToken() {
        localStorage.removeItem(this.tokenKey);
        sessionStorage.removeItem(this.tokenKey);
    }

    /**
     * Parses time as timestamp
     * @param {String} input 
     */
    static parseTime(input) {
        // hh:mm:ss.ttt
        if (/^\d{1,2}:\d{2}:\d{2}[.,]\d{1,3}$/.test(input)) {
            var [h, m, s, ms] = input.split(':.,').map(Number);
            return h * 3600 + m * 60 + s + ms / 1000;
        }

        // mm:ss.ttt
        if (/^\d{1,2}:\d{2}[.,]\d{1,3}$/.test(input)) {
            var [m, s, ms] = input.split(':.,').map(Number);
            return m * 60 + s + ms / 1000;
        }

        // hh:mm:ss
        if (/^\d{1,2}:\d{2}:\d{2}$/.test(input)) {
            var [h, m, s] = input.split(':').map(Number);
            return h * 3600 + m * 60 + s;
        }

        // mm:ss
        if (/^\d{1,2}:\d{2}$/.test(input)) {
            var [m, s] = input.split(':').map(Number);
            return m * 60 + s;
        }

        return NaN;
    }

    /**
     * 
     * @param {Number} seconds 
     */
    static timeToHuman(seconds) {
        var negate = seconds < 0;

        if (negate) {
            seconds = -seconds;
        }

        var h = Math.floor(seconds / 3600);
        var m = Math.floor((seconds % 3600) / 60);
        var s = Math.floor(seconds % 60);

        var formatted = h > 0
            ? `${h}:${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`
            : `${m}:${s.toString().padStart(2, '0')}`;

        return negate
            ? `-${formatted}`
            : formatted;
    }

    /**
     * Starts downloading of url in tew tab
     * @param {String} url URL to download
     */
    static openDownloadUrl(url) {
        var link = document.createElement('a');
        link.style.display = 'none';
        link.setAttribute('href', url);
        link.setAttribute('download', '');
        link.setAttribute('target', '_blank');
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

    /**
     * 
     * @param {String} name 
     */
    static checkThemeName(name) {
        var name = name?.toLowerCase();

        if (name === 'light') {
            return 'light';
        }

        return 'dark';
    }

    /**
     * 
     * @param {Number} size 
     * @returns 
     */
    static sizeToHuman(size) {
        let sizeStr, unit;
        let negate = size < 0;

        if (negate) {
            size = -size;
        }

        if (size < (1 << 10)) {
            sizeStr = size.toString();
            unit = "B";
        } else if (size < (1 << 20)) {
            sizeStr = (size / (1 << 10)).toFixed(2);
            unit = "KiB";
        } else if (size < (1 << 30)) {
            sizeStr = (size / (1 << 20)).toFixed(2);
            unit = "MiB";
        } else {
            sizeStr = (size / (1 << 30)).toFixed(2);
            unit = "GiB";
        }

        if (sizeStr.length > 4) {
            sizeStr = sizeStr.slice(0, 4).replace(/[,\.]$/, "");
        }

        return negate
            ? `-${sizeStr} ${unit}`
            : `${sizeStr} ${unit}`;
    }

    /**
     * 
     * @param {Number} ms 
     * @returns 
     */
    static async sleep(ms) {
        return new Promise(
            resolve => setTimeout(resolve, ms)
        );
    }

    /**
     * 
     * @param {String} colorHex 
     * @returns {{r: Number, g: Number, b: Number, a: Number}}
     */
    static parseColor(colorHex) {
        var colorRegexResult = /[0-9a-fA-F]+/.exec(colorHex);

        if (!colorRegexResult?.length) {
            return COLOR_EMPTY;
        }

        var color = colorRegexResult[0];

        switch (color.length) {
            case 3:
                var [r, g, b] = Array.from(color).map(e => parseInt(`${e}${e}`, 16));
                return { r, g, b, a: 0 };
            case 4:
                var [r, g, b, a] = Array.from(color).map(e => parseInt(`${e}${e}`, 16));
                return { r, g, b, a };
            case 6:
                var [r, g, b] = Array.from(color.matchAll('[0-9a-fA-F]{2}')).flat().map(e => parseInt(e, 16));
                return { r, g, b, a: 0 };
            case 8:
                var [r, g, b, a] = Array.from(color.matchAll('[0-9a-fA-F]{2}')).flat().map(e => parseInt(e, 16));
                return { r, g, b, a };
            default:
                return COLOR_EMPTY;
        }
    }

    /**
     * 
     * @param {Number} r 
     * @param {Number} g 
     * @param {Number} b 
     * @param {Number} a 
     * @returns {String}
     */
    static makeColor(r, g, b, a) {
        r = (r ?? 0) % 256;
        g = (g ?? 0) % 256;
        b = (b ?? 0) % 256;
        a = (a ?? 0) % 256;

        return `#${r.toString(16).padStart(2, '0')}${g.toString(16).padStart(2, '0')}${b.toString(16).padStart(2, '0')}${a > 0 ? a.toString(16).padStart(2, '0') : ''}`;
    }

    /**
     * 
     * @param  {...String} colors 
     * @returns {String}
     */
    static combineColors(...colors) {
        var parsed = colors.map(e => this.parseColor(e));

        return this.makeColor(
            parsed.map(e => e.r).reduce((acc, current) => acc + current, 0),
            parsed.map(e => e.g).reduce((acc, current) => acc + current, 0),
            parsed.map(e => e.b).reduce((acc, current) => acc + current, 0),
            parsed.map(e => e.a).reduce((acc, current) => acc + current, 0)
        )
    }

    /**
     * 
     * @returns {String}
     */
    static getBrowserLocale() {
        return navigator?.languages?.length
            ? navigator.languages.firstOrDefault(e => e?.length)
                .split('-')
                .firstOrDefault()?.toLowerCase()
            : navigator.language.split('-')
                .firstOrDefault()?.toLowerCase();
    }

    /**
     * 
     * @returns {String}
     */
    static getFallbackLocale() {
        return LOCALE_EN;
    }

    /**
     * 
     * @param {String} locale 
     * @returns {String} Supported locale only
     */
    static filterLocale(locale) {
        locale = locale?.toLowerCase();

        if (LOCALES.includes(locale)) {
            return locale;
        } else {
            return this.getFallbackLocale();
        }
    }

    /**
     * 
     * @param {{locale: String}} user 
     * @returns 
     */
    static getLocale(user = null) {
        return this.filterLocale(user?.locale ?? this.getBrowserLocale());
    }
}