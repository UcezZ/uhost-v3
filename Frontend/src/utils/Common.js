import Hls from 'hls.js';
import * as Sentry from '@sentry/browser';
import lang from '../lang.json';
import Bowser from 'bowser';

const COLOR_EMPTY = { r: 0, g: 0, b: 0, a: 0 };

const LOCALE_RU = 'ru';
const LOCALE_EN = 'en';

const LOCALES = [
    LOCALE_RU,
    LOCALE_EN
];

const THEME_LIGHT = 'light';
const THEME_DARK = 'dark';

const THEMES = [
    THEME_LIGHT,
    THEME_DARK
];

const SUPPORT_STATES = [
    'probably',
    'maybe'
];

const IS_MP4_SUPPORTED = SUPPORT_STATES.some(e => e === document.createElement('video').canPlayType('video/mp4; codecs="avc1.42E01E, mp4a.40.2"'));
const IS_HLS_SUPPORTED = IS_MP4_SUPPORTED && (Hls.isSupported() || Hls.isMSESupported());

const KEY_TOKEN = 'access_token';
const KEY_SAFARIEXPLORERALERT = 'safari_explorer_alert';

const SORT_DIRECTION_LIST = [
    'asc',
    'desc'
];

export default class Common {

    static getTokenKey() {
        return KEY_TOKEN;
    }

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
        } else if (error?.response?.data) {
            return JSON.stringify(error?.response?.data, null, 2);
        } else {
            console.log(error);
            return 'N/A';
        }

    }

    /**
     * Determines if access token is present
     * @returns {Boolean}
     */
    static isTokenPresent() {
        try {
            return Boolean(localStorage.getItem(KEY_TOKEN)) || Boolean(sessionStorage.getItem(KEY_TOKEN));
        }
        catch (err) {
            Sentry.withScope(scope => {
                scope.setExtra('msg', { message: 'failed to get token from storage in Common' });
                Sentry.captureException(err);
            });

            return false;
        }
    }

    static resetToken() {
        try {
            localStorage.removeItem(KEY_TOKEN);
        } catch (err) {
            Sentry.withScope(scope => {
                scope.setExtra('msg', { message: 'failed to reset local token in Common' });
                Sentry.captureException(err);
            });
        }
        try {
            window.sessionStorage.removeItem(KEY_TOKEN);
        } catch (err) {
            Sentry.withScope(scope => {
                scope.setExtra('msg', { message: 'failed to reset session token in Common' });
                Sentry.captureException(err);
            });
        }
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
     * @returns {String}
     */
    static getBrowserTheme() {
        return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches
            ? THEME_DARK
            : THEME_LIGHT;
    }

    /**
     * 
     * @param {String} name 
     */
    static checkThemeName(name) {
        var name = name?.toLowerCase();

        if (THEMES.includes(name)) {
            return name;
        }

        return THEME_DARK;
    }

    /**
     * 
     * @returns {String[]}
     */
    static getThemes() {
        return THEMES;
    }

    /**
     * 
     * @returns {String[]}
     */
    static getLocales() {
        return LOCALES;
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

    /**
     * 
     * @returns {Boolean}
     */
    static isHlsSupported() {
        return IS_HLS_SUPPORTED;
    }

    /**
     * 
     * @returns {Boolean}
     */
    static isMp4Supported() {
        return IS_MP4_SUPPORTED;
    }

    /**
     * 
     * @param {String} id 
     */
    static scrollToElementById(id) {
        var element = document.getElementById(id);

        if (element) {
            element.scrollIntoView({
                behavior: 'smooth',
                block: 'center'
            });
        }
    }

    /**
     * 
     * @returns {String[]}
     */
    static getSortDirections() {
        return SORT_DIRECTION_LIST;
    }

    /**
     * 
     * @param {String} value 
     * @returns {String}
     */
    static filterSortDirection(value) {
        if (!SORT_DIRECTION_LIST.includes(value)) {
            return SORT_DIRECTION_LIST[0];
        }

        return value;
    }

    /**
     * Returns value if it is contained by array, otherwise default item of array
     * @param {String[]} array 
     * @param {String} value 
     * @param {Number} defaultIndex 
     * @returns {String}
     */
    static filterStringArrayValue(array, value, defaultIndex = 0) {
        if (!array.includes(value)) {
            return array[defaultIndex];
        }

        return value;
    }

    /**
     * Crutch to use i18n feature without hooks
     * @param {String} key 
     * @returns {String}
     */
    static i18n(key) {
        var locale = this.getBrowserLocale();

        if (key in lang[locale].translation) {
            return lang[locale].translation[key];
        } else if (key in lang[LOCALE_EN].translation) {
            return lang[LOCALE_EN].translation[key];
        } {
            return key;
        }
    }

    /**
     * Checks if safari/explorer alert should bw shown
     * @returns {Boolean}
     */
    static checkSetSafariExplorerAlert() {
        const bowsr = Bowser.getParser(window.navigator.userAgent);

        if (!bowsr.satisfies({ 'safari': '>0' }) && !bowsr.satisfies({ 'internet explorer': '>0' })) {
            console.log('not');
            return false;
        }

        if (localStorage.getItem(KEY_SAFARIEXPLORERALERT)?.length > 0) {
            return false;
        }

        return true;
    }

    static setSafariExplorerAlert() {
        localStorage.setItem(KEY_SAFARIEXPLORERALERT, 'true');
    }
}