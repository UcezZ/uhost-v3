export default class Common {
    static tokenKey = 'accessToken';

    static transformErrorData(error) {
        if (error?.response?.data?.errors) {
            if (error.response.data.errors instanceof String) {
                return error.response.data.errors;
            } else {
                return Object
                    .keys(error.response.data.errors)
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
        var h = Math.floor(seconds / 3600);
        var m = Math.floor((seconds % 3600) / 60);
        var s = Math.floor(seconds % 30);

        if (h > 0) {
            return `${h}:${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
        } else {
            return `${m}:${s.toString().padStart(2, '0')}`;
        }
    }
}